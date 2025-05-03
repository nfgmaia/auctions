namespace Auctions.Domain.Entities;

public record SuvProperties(string Manufacturer, string Model, int Year, long StartingBid, int NrSeats, string? Id = null) : 
    VehicleProperties(Manufacturer, Model, Year, StartingBid, Id);

public class Suv : Vehicle
{
    public int NrSeats { get; private init; }
    
    private Suv()
    {
    }

    internal static Suv Create(SuvProperties properties)
    {
        return new()
        {
            Id = properties.Id ?? Guid.CreateVersion7().ToString(),
            Type = VehicleType.Suv,
            Manufacturer = properties.Manufacturer,
            Model = properties.Model,
            Year = properties.Year,
            StartingBid = properties.StartingBid,
            NrSeats = properties.NrSeats
        };
    }
}

internal class SuvFactory : IVehicleFactory
{
    public Vehicle Create(VehicleProperties properties)
    {
        if (properties is not SuvProperties suvProperties)
            throw new ArgumentException($"Requires {nameof(SuvProperties)}");

        return Suv.Create(suvProperties);
    }
}