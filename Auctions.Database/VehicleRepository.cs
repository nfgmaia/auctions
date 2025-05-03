using Auctions.Domain.Entities;
using Auctions.Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Database;

internal class VehicleRepository(AuctionsContext dbContext) : IVehicleRepository
{
    public async Task<Vehicle?> GetAsync(string id)
    {
        var vehicle = await dbContext.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return vehicle?.ToDomainEntity();
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        var entity = Entities.Vehicle.FromDomainEntity(vehicle);
        await dbContext.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(string id)
    {
        return dbContext.Vehicles
            .AsNoTracking()
            .AnyAsync(e => e.Id == id);
    }

    public async Task<IReadOnlyList<Vehicle>> SearchAsync(
        string? cursorId, bool asc, int limit, string? manufacturer, string? model, VehicleType? type, int? year)
    {
        var query = dbContext.Vehicles.AsNoTracking();
        
        if (!string.IsNullOrEmpty(cursorId))
        {
            query = asc
                ? query.Where(v => v.Id.CompareTo(cursorId) < 0)
                : query.Where(v => v.Id.CompareTo(cursorId) > 0);
        }

        if (!string.IsNullOrWhiteSpace(manufacturer))
            query = query.Where(e => EF.Functions.ILike(e.Manufacturer, manufacturer.Trim()));

        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(e => EF.Functions.ILike(e.Model, model.Trim()));
        
        if (type is not null)
            query = query.Where(e => e.Type == type);

        if (year is not null)
            query = query.Where(e => e.Year == year);
        
        query = asc ? 
            query.OrderByDescending(e => e.Id) : 
            query.OrderBy(e => e.Id);

        var vehicles = await query
            .Take(limit)
            .ToListAsync();

        return vehicles.Select(e => e.ToDomainEntity()).ToList();
    }
}
