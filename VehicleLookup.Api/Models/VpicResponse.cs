namespace VehicleLookup.Api.Models;

public record VpicResponse<T>(int Count, string Message, List<T> Results);

public record Make(int Make_ID, string Make_Name);

public record VehicleType(int VehicleTypeId, string VehicleTypeName);