using System.Reflection;
using Auctions.Domain.Entities;
using Auctions.Domain.Ports;
using DbUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Npgsql.NameTranslation;

namespace Auctions.Database;

public static class IocExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")!;
        DeployScripts(connectionString);
        
        var builder = new NpgsqlDataSourceBuilder(connectionString);
        builder.MapEnum<VehicleType>();
        services.AddSingleton(builder.Build());
        
        services.AddDbContextFactory<AuctionsContext>((sp, options) =>
        {
            var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
            options.UseNpgsql(
                    dataSource, 
                    o => o.MapEnum<VehicleType>("vehicle_type", "public", new NpgsqlSnakeCaseNameTranslator()))
                .UseSnakeCaseNamingConvention();
        });
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
    }
    
    private static void DeployScripts(string connectionString)
    {
        var engine = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetCallingAssembly())
            .LogToConsole()
            .Build();

        var result = engine.PerformUpgrade();
        if (!result.Successful)
        {
            throw new Exception(result.Error.Message);
        }
    }
}