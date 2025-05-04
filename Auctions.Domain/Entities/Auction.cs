namespace Auctions.Domain.Entities;

public class Auction
{
    public string? Id { get; private init; }
    public string? VehicleId { get; private init; }
    public DateTime StartDate { get; private init; }
    public DateTime? EndDate { get; private set; }
    public long StartingBid { get; private init; }
    public long? CurrentBid { get; private set; }
    public string? LastBidder { get; private set; }
    public DateTime? LastBidAt { get; private set; }
    public DateTime CreatedAt { get; private init; }

    public bool IsActive => EndDate == null;
    
    private Auction()
    {
    }
    
    public static Auction Create(
        string vehicleId,
        DateTime startDate,
        DateTime? endDate,
        long startingBid,
        long? currentBid,
        string? lastBidder,
        DateTime? lastBidDate,
        DateTime createdAt,
        string? id = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(vehicleId, nameof(vehicleId));
        ArgumentOutOfRangeException.ThrowIfLessThan(startingBid, 0, nameof(startingBid));
        ArgumentOutOfRangeException.ThrowIfLessThan(currentBid ?? startingBid, startingBid, nameof(currentBid));
        return new()
        {
            Id = id ?? Guid.CreateVersion7().ToString(),
            VehicleId = vehicleId,
            StartDate = startDate,
            EndDate = endDate,
            StartingBid = startingBid,
            CurrentBid = currentBid,
            LastBidder = lastBidder,
            LastBidAt = lastBidDate,
            CreatedAt = createdAt
        };
    }
    
    public static Auction StartNew(string vehicleId, long startingBid)
    {
        ArgumentException.ThrowIfNullOrEmpty(vehicleId, nameof(vehicleId));
        ArgumentOutOfRangeException.ThrowIfLessThan(startingBid, 0, nameof(startingBid));
        var now = DateTime.UtcNow;
        return new()
        {
            Id = Guid.CreateVersion7().ToString(),
            VehicleId = vehicleId,
            StartingBid = startingBid,
            StartDate = now,
            CreatedAt = now,
        };
    }
    
    public void SetCurrentBid(long bid, string bidder)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(bid, CurrentBid ?? StartingBid, nameof(bid));
        ArgumentException.ThrowIfNullOrEmpty(bidder, nameof(bidder));
        CurrentBid = bid;
        LastBidder = bidder;
        LastBidAt = DateTime.UtcNow;
    }
    
    public void EndAuction()
    {
        if (EndDate != null)
            throw new InvalidOperationException("Auction already ended");
        
        EndDate = DateTime.UtcNow;
    }
}
