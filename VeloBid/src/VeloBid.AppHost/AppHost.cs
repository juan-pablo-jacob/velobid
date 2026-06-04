var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter(
    name: "postgres-password",
    value: "postgres");

var postgres = builder
    .AddPostgres(
        name: "postgres",
        password: postgresPassword,
        port: 55432)
    .WithDataVolume("velobid-postgres-data")
    .WithLifetime(ContainerLifetime.Persistent);

var auctionsDb = postgres.AddDatabase("auctions-db", databaseName: "velobid_auctions");

builder.AddProject<Projects.VeloBid_Services_Auctions_API>("auctions-api")
    .WithReference(auctionsDb)
    .WaitFor(auctionsDb);

builder.Build().Run();