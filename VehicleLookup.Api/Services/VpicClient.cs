using Microsoft.Extensions.Caching.Memory;
using VehicleLookup.Api.Models;

namespace VehicleLookup.Api.Services;

public class VpicClient
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan MakesTtl = TimeSpan.FromHours(24);

    public VpicClient(HttpClient http, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<List<Make>> GetAllMakesAsync(CancellationToken ct)
    {
        return await _cache.GetOrCreateAsync("vpic:allmakes", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = MakesTtl;
            var resp = await _http.GetFromJsonAsync<VpicResponse<Make>>(
                "vehicles/getallmakes?format=json", ct);
            return resp?.Results ?? new List<Make>();
        }) ?? new List<Make>();
    }

    public async Task<List<VehicleType>> GetVehicleTypesAsync(int makeId, CancellationToken ct)
    {
        var resp = await _http.GetFromJsonAsync<VpicResponse<VehicleType>>(
            $"vehicles/GetVehicleTypesForMakeId/{makeId}?format=json", ct);
        return resp?.Results ?? new List<VehicleType>();
    }

    public async Task<List<Model>> GetModelsAsync(int makeId, int year, string? vehicleType, CancellationToken ct)
    {
        var url = $"vehicles/GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{year}";
        if (!string.IsNullOrWhiteSpace(vehicleType))
            url += $"/vehicleType/{Uri.EscapeDataString(vehicleType)}";
        url += "?format=json";

        var resp = await _http.GetFromJsonAsync<VpicResponse<Model>>(url, ct);
        return resp?.Results ?? new List<Model>();
    }
}