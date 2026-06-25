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

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/makes", async (VpicClient vpic, CancellationToken ct) =>
    Results.Ok(await vpic.GetAllMakesAsync(ct)));

app.MapGet("/api/makes/{makeId:int}/vehicle-types", async (int makeId, VpicClient vpic, CancellationToken ct) =>
    Results.Ok(await vpic.GetVehicleTypesAsync(makeId, ct)));

app.MapGet("/api/makes/{makeId:int}/models", async (
    int makeId,
    int year,
    string? vehicleType,
    VpicClient vpic,
    CancellationToken ct) =>
{
    if (year < 1900 || year > DateTime.UtcNow.Year + 2)
        return Results.BadRequest("year out of range");
    return Results.Ok(await vpic.GetModelsAsync(makeId, year, vehicleType, ct));
});

app.MapFallbackToFile("index.html");

app.Run();