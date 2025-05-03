namespace Auctions.Domain;

public enum ConcurrencyStrategy
{
    RowVersion,
    RowLevelLocking
}