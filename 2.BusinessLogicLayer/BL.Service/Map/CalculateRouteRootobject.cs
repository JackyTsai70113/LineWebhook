using System.Text.Json.Serialization;

namespace BL.Service.Map
{
    public class CalculateRouteRootobject
    {
        [JsonPropertyName("response")]
        public Response Response { get; set; }
    }

    public class Response
    {
        [JsonPropertyName("metaInfo")]
        public Metainfo MetaInfo { get; set; }
        [JsonPropertyName("route")]
        public Route[] Route { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; }
    }

    public class Metainfo
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("mapVersion")]
        public string MapVersion { get; set; }
        [JsonPropertyName("moduleVersion")]
        public string ModuleVersion { get; set; }
        [JsonPropertyName("interfaceVersion")]
        public string InterfaceVersion { get; set; }
        [JsonPropertyName("availableMapVersion")]
        public string[] AvailableMapVersion { get; set; }
    }

    public class Route
    {
        [JsonPropertyName("waypoint")]
        public Waypoint[] Waypoint { get; set; }
        [JsonPropertyName("mode")]
        public Mode Mode { get; set; }
        [JsonPropertyName("leg")]
        public Leg[] Leg { get; set; }
        [JsonPropertyName("summary")]
        public Summary Summary { get; set; }
    }

    public class Mode
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("transportModes")]
        public string[] TransportModes { get; set; }
        [JsonPropertyName("trafficMode")]
        public string TrafficMode { get; set; }
        [JsonPropertyName("feature")]
        public object[] Feature { get; set; }
    }

    public class Summary
    {
        [JsonPropertyName("distance")]
        public int Distance { get; set; }
        [JsonPropertyName("baseTime")]
        public int BaseTime { get; set; }
        [JsonPropertyName("flags")]
        public string[] Flags { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("travelTime")]
        public int TravelTime { get; set; }
        [JsonPropertyName("_type")]
        public string Type { get; set; }
    }

    public class Waypoint
    {
        [JsonPropertyName("linkId")]
        public string LinkId { get; set; }
        [JsonPropertyName("mappedPosition")]
        public Position MappedPosition { get; set; }
        [JsonPropertyName("originalPosition")]
        public Position OriginalPosition { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("spot")]
        public float Spot { get; set; }
        [JsonPropertyName("sideOfStreet")]
        public string SideOfStreet { get; set; }
        [JsonPropertyName("mappedRoadName")]
        public string MappedRoadName { get; set; }
        [JsonPropertyName("label")]
        public string Label { get; set; }
        [JsonPropertyName("shapeIndex")]
        public int ShapeIndex { get; set; }
        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    public class Position
    {
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }
    }

    public class Leg
    {
        [JsonPropertyName("start")]
        public Waypoint Start { get; set; }
        [JsonPropertyName("end")]
        public Waypoint End { get; set; }
        [JsonPropertyName("length")]
        public int Length { get; set; }
        [JsonPropertyName("travelTime")]
        public int TravelTime { get; set; }
        [JsonPropertyName("maneuver")]
        public Maneuver[] Maneuver { get; set; }
    }

    public class Maneuver
    {
        [JsonPropertyName("position")]
        public Position Position { get; set; }
        [JsonPropertyName("instruction")]
        public string Instruction { get; set; }
        [JsonPropertyName("travelTime")]
        public int TravelTime { get; set; }
        [JsonPropertyName("length")]
        public int Length { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("_type")]
        public string Type { get; set; }
    }
}