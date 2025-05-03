using Auctions.Domain.Entities;
using Auctions.Domain.Ports;

namespace Auctions.Application.Features.EndAuction;

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

        var auction = await repository.GetAsync(command.AuctionId, forUpdate: true);
        
        // Check if auction exists
        if (auction == null)
        {
            return Result<Auction>.Failure("Auction not found");
        }
        
        // Check if auction is active
        if (!auction.IsActive)
        {
            return Result<Auction>.Failure("Auction already ended"); 
        }

        auction.EndAuction();
        await repository.UpdateAsync(auction);

        return Result<Auction>.Success(auction);
    }
}