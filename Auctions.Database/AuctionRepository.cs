using Auctions.Domain;
using Auctions.Domain.Entities;
using Auctions.Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Database;

internal class AuctionRepository(AuctionsContext dbContext) : IAuctionRepository
{
    public async Task<Auction?> GetAsync(string id, bool forUpdate = false)
    {
        var auction = await Get(forUpdate)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        return auction?.ToDomainEntity();
    }
    
    public async Task AddAsync(Auction auction)
    {
        var entity = Entities.Auction.FromDomainEntity(auction);
        await dbContext.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Auction auction)
    {
        try
        {
            var entity = Entities.Auction.FromDomainEntity(auction);
            if (await dbContext.FindAsync(entity.GetType(), entity.Id) is { } current)
            {
                dbContext.Entry(current).CurrentValues.SetValues(entity);
            }
            else
            {
                dbContext.Add(entity);
            }
            return await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await ex.Entries.Single().ReloadAsync();
            throw new RowVersionException("Row version expired", ex);
        }       
    }
    
    public async Task<int> UpdateBidAsync(Auction auction, ConcurrencyStrategy strategy)
    {
        return strategy switch
        {
            ConcurrencyStrategy.RowVersion => await UpdateAsync(auction),
            ConcurrencyStrategy.RowLevelLocking => await UpdateBidRowLevelLockingAsync(auction),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
        };
    }
    
    private async Task<int> UpdateBidRowLevelLockingAsync(Auction auction)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        
        var lockedIds = await dbContext.Auctions
            .FromSqlRaw("SELECT * FROM auction WHERE id = {0} AND current_bid < {1} FOR UPDATE", 
                auction.Id, auction.CurrentBid)
            .Select(e => e.Id)
            .ToListAsync();
            
        var updatedRows = await dbContext.Auctions
            .Where(e => lockedIds.Contains(e.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(e => e.CurrentBid, _ => auction.CurrentBid)
                .SetProperty(e => e.LastBidder, _ => auction.LastBidder)
                .SetProperty(e => e.LastBidAt, _ => auction.LastBidAt)
            );
        
        await transaction.CommitAsync();
        return updatedRows;
    }

    public Task<bool> ExistsActiveAuctionForVehicleAsync(string vehicleId) => 
        dbContext.Auctions.AnyAsync(e => e.VehicleId == vehicleId && e.EndDate == null);
    
    private IQueryable<Database.Entities.Auction> Get(bool forUpdate) => forUpdate ? 
        dbContext.Auctions.AsTracking() : 
        dbContext.Auctions.AsNoTracking();
}