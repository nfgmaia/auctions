namespace Auctions.Domain.Entities;

public record HatchbackProperties(
    string Manufacturer, string Model, int Year, long StartingBid, int NrDoors, string? Id = null) : 
    VehicleProperties(Manufacturer, Model, Year, StartingBid, Id);

public class Hatchback : Vehicle
{
    public int NrDoors { get; private init; }

    private Hatchback()
    {
    }

    internal static Hatchback Create(HatchbackProperties properties)
    {
        return new()
        {
            Id = properties.Id ?? Guid.CreateVersion7().ToString(),
            Type = VehicleType.Hatchback,
            Manufacturer = properties.Manufacturer,
            Model = properties.Model,
            Year = properties.Year,
            StartingBid = properties.StartingBid,
            NrDoors = properties.NrDoors
        };
    }
}

internal class HatchbackFactory : IVehicleFactory
{
    public Vehicle Create(VehicleProperties properties)
    {
        if (properties is not HatchbackProperties hatchbackProperties)
            throw new ArgumentException($"Requires {nameof(HatchbackProperties)}");

        return Hatchback.Create(hatchbackProperties);
    }
}