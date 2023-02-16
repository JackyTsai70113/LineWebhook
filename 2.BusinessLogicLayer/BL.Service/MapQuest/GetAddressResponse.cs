using System.Text.Json.Serialization;

namespace BL.Service.MapQuest;

public class GetAddressResponse
{
    [JsonPropertyName("results")]
    public List<Result> Results { get; set; }
}

public class Result
{
    [JsonPropertyName("locations")]
    public IEnumerable<Location> Locations { get; set; }
}