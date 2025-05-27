using System.Text.Json;
using System.Text.Json.Serialization;

namespace project_wildfire_web.Models.ArcGis;

public class ArcGisGeometry
{
    [JsonPropertyName("x")]
    public decimal X { get; set; }

    [JsonPropertyName("y")]
    public decimal Y { get; set; }
}