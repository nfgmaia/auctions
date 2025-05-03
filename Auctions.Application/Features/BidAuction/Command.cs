namespace Auctions.Application.Features.BidAuction;

public record Command(string AuctionId, long BidAmount, string Bidder);