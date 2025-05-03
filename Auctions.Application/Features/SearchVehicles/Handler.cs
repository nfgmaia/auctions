using System.Text;
using Auctions.Domain.Entities;
using Auctions.Domain.Ports;

namespace Auctions.Application.Features.SearchVehicles;

public class Handler(Validator validator, IVehicleRepository repository) 
    : IHandler<Command, PaginatedResult<IReadOnlyList<Vehicle>>>
{
    public async Task<PaginatedResult<IReadOnlyList<Vehicle>>> Handle(Command command)
    {
        // Validate command
        var validation = await validator.ValidateAsync(command);
        if (!validation.IsValid)
        {
            return PaginatedResult<IReadOnlyList<Vehicle>>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).First());
        }
        
        VehicleType? type = Enum.TryParse<VehicleType>(command.Type, true, out var val) ? val : null;
        
        // Search vehicles
        var limit = command.Limit ?? 10;
        var (cursorId, isNext) = DecodeCursor(command.Cursor);
        
        var vehicles = await repository.SearchAsync(
            cursorId, isNext, limit + 1, command.Manufacturer, command.Model, type, command.Year);
        
        // Handle pagination
        var hasMore = vehicles.Count > limit;
        var isPrev = cursorId != null && !isNext;
        var isAtStart = cursorId == null || isPrev && !hasMore;

        vehicles = hasMore ? 
            vehicles.SkipLast(1).OrderByDescending(e => e.Id).ToList() : 
            vehicles.OrderByDescending(e => e.Id).ToList();

        var nextCursor = isPrev || hasMore ? EncodeCursor(vehicles[^1].Id, true) : null;
        var previousCursor = !isAtStart ? EncodeCursor(vehicles[0].Id, false): null;
        
        return PaginatedResult<IReadOnlyList<Vehicle>>.Success(vehicles, nextCursor, previousCursor);
    }
    
    private static (string? cursorId, bool isNext) DecodeCursor(string? cursor)
    {
        if (string.IsNullOrEmpty(cursor)) 
            return (null, true);

        var decodedByteArr = Convert.FromBase64String(cursor);
        var decodedString = Encoding.UTF8.GetString(decodedByteArr);
        var parts = decodedString.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid cursor format");
        
        return (parts[0], parts[1] == "next");
    }
    
    private static string? EncodeCursor(string? cursorId, bool isNext)
    {
        if (string.IsNullOrEmpty(cursorId))
            return null;

        var direction = isNext ? "next" : "previous";
        var encodedString = $"{cursorId}:{direction}";
        var encodedBytes = Encoding.UTF8.GetBytes(encodedString);
        return Convert.ToBase64String(encodedBytes);
    }
}