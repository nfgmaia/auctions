using Auctions.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Database;

internal class AuctionsContext(DbContextOptions<AuctionsContext> options) : DbContext(options)
{
    public DbSet<Auction> Auctions => Set<Auction>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuctionsContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}