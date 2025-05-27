using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using project_wildfire_web.Models.ArcGis;


namespace project_wildfire_web.Services
{


    public class ArcGisService : IArcGisService
    {
        private readonly HttpClient _httpClient;

        public ArcGisService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        /// <summary>
        /// Retrieves all wildfires from the USA_Wildfires_v1 Feature Service.
        /// </summary>
        public async Task<List<FireEvent>> GetUsaWildfiresAsync()
        {
            //var url1 = "https://services9.arcgis.com/RHVPKKiFTONKtxq3/arcgis/rest/services/USA_Wildfires_v1/FeatureServer/0/query?where=1=1&outFields=IncidentName,InitialLatitude,InitialLongitude,PercentContained&outSR=4326&f=json";
            // var url2 = "https://services9.arcgis.com/RHVPKKiFTONKtxq3/arcgis/rest/services/USA_Wildfires_v1/FeatureServer/0/query?where=FireDiscoveryDateTime%20%3E%20DATE%20'2025-01-01'&outFields=IncidentName,FireDiscoveryDateTime,PercentContained&outSR=4326&f=json";
            var url = "https://services9.arcgis.com/RHVPKKiFTONKtxq3/arcgis/rest/services/USA_Wildfires_v1/FeatureServer/0/query?where=1%3D1&outFields=*&outSR=4326&f=json";
            var raw = await _httpClient.GetStringAsync(url);
            var arcResp = JsonSerializer.Deserialize<ArcGisFeatureResponse>(raw);

            return arcResp?.Features?.Select(f => new FireEvent
            {
                UniqueFireIdentifier = f.Attributes.UniqueFireIdentifier ?? string.Empty,
                Name = f.Attributes.IncidentName,
                Latitude = f.Geometry.Y,
                Longitude = f.Geometry.X,
                StartDate = f.Attributes.FireDiscoveryDateTime.HasValue
                    ? DateTimeOffset.FromUnixTimeMilliseconds(f.Attributes.FireDiscoveryDateTime.Value).UtcDateTime
                    : DateTime.MinValue,
               // AcreageBurned = f.Attributes.DiscoveryAcres ?? 0,
                // ...existing code...
                AcreageBurned = f.Attributes.DiscoveryAcres ?? 0m,
// ...existing code...
                PercentageContained = f.Attributes.PercentContained ?? 0m,
                POOCounty = f.Attributes.POOCounty ?? string.Empty,
                POOState = f.Attributes.POOState ?? string.Empty,
                FireCause = f.Attributes.FireCause ?? string.Empty,
            }).ToList()
            ?? new List<FireEvent>();
        }
        
        public async Task<bool> FireExistsAsync(string uniqueFireId)
        {
            // build the count-only URL
          //  var url2 = $"USA_Wildfires_v1/FeatureServer/0/query?where=UniqueFireIdentifier%3D%27{uniqueFireId}%27&returnCountOnly=true&f=json";
          //  var url1 = $"USA_Wildfires_v1/FeatureServer/0/query?where=UniqueFireIdentifier%3D%27{uniqueFireId}%27&returnCountOnly=true&f=json";
            var url = $"https://services9.arcgis.com/RHVPKKiFTONKtxq3/arcgis/rest/services/USA_Wildfires_v1/FeatureServer/0/query?where=UniqueFireIdentifier%3D%27{uniqueFireId}%27&returnCountOnly=true&f=json";
            // issue the request
            var resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            // parse the simple { "count": X } JSON
            var countResp = await JsonSerializer.DeserializeAsync<CountResponse>(
                await resp.Content.ReadAsStreamAsync()
            );
            return (countResp?.Count ?? 0) > 0;
        }

        
    }
}
