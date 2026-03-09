namespace MemoryTrave.Maui.Models.Location;

public class Location
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Geohash { get; set; }
}