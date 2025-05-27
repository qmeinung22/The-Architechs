using System.Text.Json.Serialization;

namespace project_wildfire_web.Models.ArcGis;


public class CountResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
}