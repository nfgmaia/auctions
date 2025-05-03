namespace Auctions.Domain.Entities;

public record SedanProperties(string Manufacturer, string Model, int Year, long StartingBid, int NrDoors, string? Id = null) : 
    VehicleProperties(Manufacturer, Model, Year, StartingBid, Id);

public class Sedan : Vehicle
{
    public int NrDoors { get; private init; }
    
    private Sedan()
    {
    }

    internal static Sedan Create(SedanProperties properties)
    {
        return new()
        {
            Id = properties.Id ?? Guid.CreateVersion7().ToString(),
            Type = VehicleType.Sedan,
            Manufacturer = properties.Manufacturer,
            Model = properties.Model,
            Year = properties.Year,
            StartingBid = properties.StartingBid,
            NrDoors = properties.NrDoors
        };
    }
}

internal class SedanFactory : IVehicleFactory
{
    public Vehicle Create(VehicleProperties properties)
    {
        if (properties is not SedanProperties sedanProperties)
            throw new ArgumentException($"Requires {nameof(SedanProperties)}");

        return Sedan.Create(sedanProperties);
    }
}