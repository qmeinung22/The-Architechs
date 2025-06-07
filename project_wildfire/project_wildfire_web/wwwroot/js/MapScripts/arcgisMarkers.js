export function addWildfireMarkers(layerGroup, flameIcon) {
  fetch('/api/wildfires/getSavedFires')
    .then(response => response.json())
    .then(data => {
      data.forEach(fire => {
        // Fallback radiative power for missing data
        const power = fire.radiativePower ?? fire.frp ?? 10;
        const radius = Math.max(5, Math.min(power / 2, 20));

        const getColor = (p) => {
          if (power < 10) return '#FFD700';      
          if (power < 30) return '#FFA500';      
          return '#DC143C'; 
        };

        const popupContent = `
          <strong>${fire.name}</strong><br>
          <b>ID:</b> ${fire.fireId}<br>
          <b>Start</b>: ${new Date(fire.startDate).toLocaleString()}<br>
          <b>Acres burned</b>: ${fire.acreageBurned}<br>
          <b>Contained</b>: ${fire.percentageContained}%<br>
          <b>County/State</b>: ${fire.pooCounty}, ${fire.pooState}<br>
          <b>Cause</b>: ${fire.fireCause}<br>
          <button type="button" class="btn btn-danger subscribe-btn btn-sm" data-fire-id="${fire.fireId}">Subscribe to fire</button>
          ${fire.isAdminFire ? `
            <br><button type="button" class="btn btn-warning delete-admin-fire btn-sm mt-2" data-fire-id="${fire.fireId}">
              🗑️ Delete Fire
            </button>` : ''}
        `;

        let marker;

        if (fire.isAdminFire) {
          // ✅ Restore admin fire to radiative-power circleMarker style
          marker = L.circle([fire.latitude, fire.longitude], {
            
            radius: radius * 100,
            fillColor: getColor(power),
            color: getColor(power),
            weight: 1,
            //opacity: 1,
            fillOpacity: 0.8,
            className: "wildfire-marker admin-fire"
          });
        } else {
          // 🟡 Teammate’s lighter emoji icon for non-admin fires
          marker = L.marker([fire.latitude, fire.longitude], { icon: flameIcon });
        }

        marker.bindPopup(popupContent);

        marker.on('popupopen', () => {
          const popupEl = marker.getPopup().getElement();

          // Subscribe button
          const btn = popupEl.querySelector(`.subscribe-btn[data-fire-id="${fire.fireId}"]`);
          if (btn) {
            btn.addEventListener('click', async () => {
              try {
                const res = await fetch(`/api/fireSub/${fire.fireId}`, {
                  method: 'POST',
                  credentials: 'include'
                });
                if (res.ok) {
                  btn.textContent = 'Subscribed!';
                  btn.disabled = true;
                } else {
                  alert('Subscription failed.');
                }
              } catch (err) {
                console.error(err);
                alert('An error occurred.');
              }
            });
          }

          // Admin delete button
          const deleteBtn = popupEl.querySelector(`.delete-admin-fire[data-fire-id="${fire.fireId}"]`);
          if (deleteBtn) {
            deleteBtn.addEventListener('click', async () => {
              if (confirm("Are you sure you want to delete this admin fire?")) {
                try {
                  const res = await fetch(`/api/AdminFire/Delete/${fire.fireId}`, {
                    method: 'DELETE'
                  });
                  if (res.ok) {
                    marker.remove();
                    alert("Admin fire deleted.");
                  } else {
                    alert("Failed to delete fire.");
                  }
                } catch (err) {
                  console.error(err);
                  alert("Failed to delete fire. Check console for details.");
                }
              }
            });
          }
        });

        layerGroup.addLayer(marker);
      });
    })
    .catch(error => console.error('Error loading wildfire data:', error));
}
