using VeloBid.Services.Auctions.API.Endpoints;
using VeloBid.Services.Auctions.Application;
using VeloBid.Services.Auctions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddAuctionsApplication();
builder.Services.AddAuctionsInfrastructure(builder.Configuration);

var app = builder.Build();

// app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapAuctionEndpoints();
app.MapBidEndpoints();

app.Run();