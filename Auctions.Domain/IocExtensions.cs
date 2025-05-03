using Auctions.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Auctions.Domain;

public static class IocExtensions
{
    public static void AddDomain(this IServiceCollection services)
    {
        VehicleFactory.RegisterFactory(VehicleType.Hatchback, new HatchbackFactory());
        VehicleFactory.RegisterFactory(VehicleType.Sedan, new SedanFactory());
        VehicleFactory.RegisterFactory(VehicleType.Suv, new SuvFactory());
        VehicleFactory.RegisterFactory(VehicleType.Truck, new TruckFactory());
    }
}