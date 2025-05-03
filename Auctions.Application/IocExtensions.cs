using System.Reflection;
using Auctions.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Auctions.Application;

public static class IocExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<Features.AddVehicle.Handler>();
        services.AddScoped<Features.SearchVehicles.Handler>();
        services.AddScoped<Features.StartAuction.Handler>();
        services.AddScoped<Features.EndAuction.Handler>();
        services.AddScoped<Features.BidAuction.Handler>();
    }
}