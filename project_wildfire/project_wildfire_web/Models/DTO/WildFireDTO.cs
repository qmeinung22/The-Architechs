using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using project_wildfire_web.Models;
using project_wildfire_web.Models.DTO;



namespace project_wildfire_web.Models.DTO;

public class WildfireDTO
{

    public string? FireId { get; set; }
    public string Name { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal? AcreageBurned { get; set; }
    public decimal? PercentageContained { get; set; }
    public string? POOCounty { get; set; }
    public string? POOState { get; set; }
    public string? FireCause { get; set; }
    public DateTime? StartDate { get; set; }
    public decimal? RadiativePower { get; set; }

    public bool IsAdminFire { get; set; } = false;
}