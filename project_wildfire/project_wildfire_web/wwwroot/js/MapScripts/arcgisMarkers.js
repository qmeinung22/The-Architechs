export function addWildfireMarkers(layerGroup, flameIcon) {
  fetch('/api/wildfires/getSavedFires')
    .then(response => response.json())
    .then(data => {
      data.forEach(fire => {
        let popupContent = `
           <strong>${fire.name}</strong><br>
             <b>ID:</b> ${fire.fireId}<br>
             <b>Start</b>: ${new Date(fire.startDate).toLocaleString()}<br>
             <b>Acres burned</b>: ${fire.acreageBurned}<br>
             <b>Contained</b>: ${fire.percentageContained}%<br>
             <b>County/State</b>: ${fire.pooCounty}, ${fire.pooState}<br>
             <b>Cause</b>: ${fire.fireCause}<br>
             <button type="button" class="btn btn-danger subscribe-btn btn-sm" data-fire-id="${fire.fireId}">Subscribe to fire</button>

        `;
        if (fire.isAdminFire) {
          popupContent += `
            <button type="button" class="btn btn-warning delete-admin-fire btn-sm mt-2" data-fire-id="${fire.fireId}">
              🗑️ Delete Fire
            </button>
          `;
        }
        const marker = L.marker([fire.latitude, fire.longitude], { icon: flameIcon });
        marker.bindPopup(popupContent);
        
        marker.on('popupopen', function () {
          const popupEl = marker.getPopup().getElement();
          const btn = popupEl.querySelector(`.subscribe-btn[data-fire-id="${fire.fireId}"]`);
          if (btn) {
            btn.addEventListener('click', async () => {
              try {
                const response = await fetch(`/api/fireSub/${encodeURIComponent(fire.fireId)}`, {
                  method: 'POST',
                  credentials: 'include'  // include cookies/auth header
                });
                if (response.ok) {
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

            const deleteBtn = popupEl.querySelector(`.delete-admin-fire[data-fire-id="${fire.fireId}"]`);
          if (deleteBtn) {
            deleteBtn.addEventListener('click', async () => {
              if (confirm("Are you sure you want to delete this admin fire?")) {
                try {
                  const response = await fetch(`/api/AdminFire/Delete/${fire.fireId}`, {
                    method: 'DELETE'
                  });
                  if (response.ok) {
                    marker.remove();
                    alert(" Admin fire deleted.");
                  } else {
                    alert("Failed to delete fire.");
                  }
                } catch (err) {
                  console.error("Fire delete failed:", err);
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