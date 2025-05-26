export function addWildfireMarkers(layerGroup, flameIcon) {
  fetch('/api/wildfires')
    .then(response => response.json())
    .then(data => {
      data.forEach(fire => {
        const popupContent = `
           <strong>${fire.name}</strong><br>
             <b>ID:</b> ${fire.uniqueFireIdentifier}<br>
             <b>Start</b>: ${new Date(fire.startDate).toLocaleString()}<br>
             <b>Acres burned</b>: ${fire.acreageBurned.toLocaleString()}<br>
             <b>Contained</b>: ${fire.percentageContained}%<br>
             <b>County/State</b>: ${fire.pooCounty}, ${fire.pooState}<br>
             <b>Cause</b>: ${fire.fireCause}<br>
             <button type="button" class="btn btn-danger subscribe-btn btn-sm" data-fire-id="${fire.uniqueFireIdentifier}">Subscribe to fire</button>

        `;
        const marker = L.marker([fire.latitude, fire.longitude], { icon: flameIcon });
        marker.bindPopup(popupContent);
        layerGroup.addLayer(marker);
      });
    })
    .catch(error => console.error('Error loading wildfire data:', error));
}