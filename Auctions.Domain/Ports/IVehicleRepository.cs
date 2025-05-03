using Auctions.Domain.Entities;

namespace Auctions.Domain.Ports;

public interface IVehicleRepository
{
    Task<Vehicle?> GetAsync(string id);
    Task AddAsync(Vehicle vehicle);
    Task<bool> ExistsAsync(string id);
    Task<IReadOnlyList<Vehicle>> SearchAsync(
        string? cursorId, bool asc, int limit, string? manufacturer, string? model, VehicleType? type, int? year);
}