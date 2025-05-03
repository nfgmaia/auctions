using System.Text.Json.Serialization;
using Auctions.Api.Endpoints;
using Auctions.Database;
using Auctions.Application;
using Auctions.Domain;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddDomain();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();
app.MapAddVehicle();
app.MapSearchVehicles();
app.MapStartAuction();
app.MapEndAuction();
app.MapBidAuction();
app.Run();
