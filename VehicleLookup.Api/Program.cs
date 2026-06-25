using VehicleLookup.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<VpicClient>(c =>
{
    c.BaseAddress = new Uri("https://vpic.nhtsa.dot.gov/api/");
    c.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/makes", async (VpicClient vpic, CancellationToken ct) =>
    Results.Ok(await vpic.GetAllMakesAsync(ct)));

app.Run();