using System.Collections.Concurrent;

namespace Auctions.Domain.Entities;

public abstract record VehicleProperties(string Manufacturer, string Model, int Year, long StartingBid, string? Id = null);

public abstract class Vehicle
{
    public string? Id { get; protected set; }
    public VehicleType Type { get; protected set; }
    public string? Manufacturer { get; protected set; }
    public string? Model { get; protected set; }
    public int Year { get; protected set; }
    public long StartingBid { get; protected set; }
}

internal interface IVehicleFactory
{
    Vehicle Create(VehicleProperties options);
}

public static class VehicleFactory
{
    private static readonly ConcurrentDictionary<VehicleType, IVehicleFactory> factories = new();

    internal static void RegisterFactory(VehicleType type, IVehicleFactory factory)
    {
        if (!factories.TryAdd(type, factory))
            throw new ArgumentException($"Factory already registered for {type}");
    }
    
    public static Vehicle CreateVehicle(VehicleType type, VehicleProperties options)
    {
        if (!factories.TryGetValue(type, out var factory))
            throw new ArgumentException($"No factory registered for {type}");

        return factory.Create(options);
    }
}

public enum VehicleType
{
    Hatchback,
    Sedan,
    Suv,
    Truck,
}