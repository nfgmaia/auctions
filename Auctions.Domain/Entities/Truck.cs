namespace Auctions.Domain.Entities;

public record TruckProperties(string Manufacturer, string Model, int Year, long StartingBid, int LoadCapacity, string? Id = null) : 
    VehicleProperties(Manufacturer, Model, Year, StartingBid, Id);

public class Truck : Vehicle
{
    public int LoadCapacity { get; private init; }
    
    private Truck()
    {
    }

    internal static Truck Create(TruckProperties properties)
    {
        return new()
        {
            Id = properties.Id ?? Guid.CreateVersion7().ToString(),
            Type = VehicleType.Truck,
            Manufacturer = properties.Manufacturer,
            Model = properties.Model,
            Year = properties.Year,
            StartingBid = properties.StartingBid,
            LoadCapacity = properties.LoadCapacity,
        };
    }
}

internal class TruckFactory : IVehicleFactory
{
    public Vehicle Create(VehicleProperties? properties)
    {
        if (properties is not TruckProperties truckProperties)
            throw new ArgumentException($"Requires {nameof(TruckProperties)}");

        return Truck.Create(truckProperties);
    }
}