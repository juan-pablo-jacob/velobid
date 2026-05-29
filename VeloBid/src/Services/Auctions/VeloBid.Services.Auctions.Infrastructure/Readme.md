# Migrations creation

```bash
bash dotnet ef migrations add InitialAuctionsDb
--project src/Services/Auctions/VeloBid.Services.Auctions.Infrastructure/VeloBid.Services.Auctions.Infrastructure.csproj
--context AuctionsDbContext
--output-dir Persistence/Migrations
```

```bash
bash dotnet ef database update
--project src/Services/Auctions/VeloBid.Services.Auctions.Infrastructure/VeloBid.Services.Auctions.Infrastructure.csproj
--context AuctionsDbContext
```