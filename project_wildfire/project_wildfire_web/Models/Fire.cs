using NetTopologySuite.Geometries;

namespace project_wildfire_web.Models;

public partial class Fire
{
    public string FireId { get; set; }
    public string Name { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal AcreageBurned { get; set; }
    public decimal PercentageContained { get; set; }
    public string POOCounty { get; set; }
    public string POOState { get; set; }
    public string FireCause { get; set; }
    public DateTime StartDate { get; set; }
    public decimal? RadiativePower { get; set; }

    public bool IsAdminFire { get; set; }

    //public Geometry? Polygon { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<UserFireSubscription> UserSubscriptions { get; set; } = new List<UserFireSubscription>();
}