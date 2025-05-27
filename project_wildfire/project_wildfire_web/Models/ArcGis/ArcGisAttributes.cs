using System.Text.Json;
using System.Text.Json.Serialization;

namespace project_wildfire_web.Models.ArcGis;

public class ArcGisAttributes
{
    [JsonPropertyName("SourceOID")]
    public int SourceOID { get; set; }

    [JsonPropertyName("IncidentName")]
    public string IncidentName { get; set; } = string.Empty;

    [JsonPropertyName("InitialLatitude")]
    public decimal InitialLatitude { get; set; }

    [JsonPropertyName("InitialLongitude")]
    public decimal InitialLongitude { get; set; }

    [JsonPropertyName("DiscoveryAcres")]
    public decimal? DiscoveryAcres { get; set; }

    [JsonPropertyName("FinalAcres")]
    public decimal? FinalAcres { get; set; }

    [JsonPropertyName("FireDiscoveryDateTime")]
    public long? FireDiscoveryDateTime { get; set; }

    [JsonPropertyName("IncidentSize")]
    public decimal? IncidentSize { get; set; }

    [JsonPropertyName("UniqueFireIdentifier")]
    public string? UniqueFireIdentifier { get; set; }

    [JsonPropertyName("PercentContained")]
    public decimal? PercentContained { get; set; }
}