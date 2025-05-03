using Auctions.Domain;
using Auctions.Domain.Entities;
using Auctions.Domain.Ports;

namespace Auctions.Application.Features.BidAuction;

public class Handler(Validator validator, IAuctionRepository repository) 
    : IHandler<Command, Result<Auction>>
{
    public async Task<Result<Auction>> Handle(Command command)
    {
        // Validate command
        var validation = await validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return Result<Auction>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).First());
        }

        Auction? auction = null;
        var currentStrategy = ConcurrencyStrategy.RowVersion;
        var execute = true;

        while (execute)
        {
            execute = false;
            auction = await repository.GetAsync(command.AuctionId, forUpdate: true);
            
            // Check if auction exists
            if (auction == null)
            {
                return Result<Auction>.Failure("Auction not found");
            }
            
            // Check if auction is active
            if (!auction.IsActive)
            {
                return Result<Auction>.Failure("Auction is not active"); 
            }
            
            // Check if bid is higher than current bid
            if (command.BidAmount <= (auction.CurrentBid ?? auction.StartingBid))
            {
                return Result<Auction>.Failure("'Bid Amount' must be higher.");
            }

            // Update current bid with a fallback concurrency strategy.
            // If the update fails due to an optimistic concurrency issue, retry with a different strategy.
            try
            {
                auction.SetCurrentBid(command.BidAmount, command.Bidder);
                var updatedRows = await repository.UpdateBidAsync(auction, currentStrategy);
                if (updatedRows == 0)
                {
                    return Result<Auction>.Failure("'Bid Amount' must be higher."); 
                }
            }
            catch (RowVersionException)
            {
                currentStrategy = ConcurrencyStrategy.RowLevelLocking;
                execute = true;
            }
        }

        return Result<Auction>.Success(auction!);
    }
}