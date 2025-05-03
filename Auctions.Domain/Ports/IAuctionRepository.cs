using Auctions.Domain.Entities;

namespace Auctions.Domain.Ports;

public interface IAuctionRepository
{
    Task<Auction?> GetAsync(string id, bool forUpdate = false);
    Task AddAsync(Auction auction);
    Task<int> UpdateAsync(Auction auction);
    Task<int> UpdateBidAsync(Auction auction, ConcurrencyStrategy strategy);
    Task<bool> ExistsActiveAuctionForVehicleAsync(string vehicleId);
}