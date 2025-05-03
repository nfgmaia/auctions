using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Database.Entities;

[Table("auction")]
internal class Auction
{
    public required string Id { get; set; }
    public required string VehicleId { get; set; }
    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public required long StartingBid { get; set; }
    public long? CurrentBid { get; set; }
    public string? LastBidder { get; set; }
    public DateTime? LastBidAt { get; set; }
    public required DateTime CreatedAt { get; set; }
    
    [Timestamp]
    public uint Version { get; set; }
    
    public Domain.Entities.Auction ToDomainEntity()
    {
        return Domain.Entities.Auction.Create(
            VehicleId,
            StartDate,
            EndDate,
            StartingBid,
            CurrentBid,
            LastBidder,
            LastBidAt,
            CreatedAt,
            Id);
    }
    
    public static Auction FromDomainEntity(Domain.Entities.Auction auction)
    {
        return new()
        {
            Id = auction.Id!,
            VehicleId = auction.VehicleId!,
            StartDate = auction.StartDate,
            EndDate = auction.EndDate,
            StartingBid = auction.StartingBid,
            CurrentBid = auction.CurrentBid,
            LastBidder = auction.LastBidder,
            LastBidAt = auction.LastBidAt,
            CreatedAt = auction.CreatedAt
        };
    }
}