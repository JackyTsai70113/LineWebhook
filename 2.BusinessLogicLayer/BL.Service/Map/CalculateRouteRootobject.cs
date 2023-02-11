namespace BL.Service.Map
{

    public class CalculateRouteRootobject
    {
        public Response Response { get; set; }
    }

    public class Response
    {
        public Metainfo MetaInfo { get; set; }
        public Route[] Route { get; set; }
        public string Language { get; set; }
    }

    public class Metainfo
    {
        public DateTime Timestamp { get; set; }
        public string MapVersion { get; set; }
        public string ModuleVersion { get; set; }
        public string InterfaceVersion { get; set; }
        public string[] AvailableMapVersion { get; set; }
    }

    public class Route
    {
        public Waypoint[] Waypoint { get; set; }
        public Mode Mode { get; set; }
        public Leg[] Leg { get; set; }
        public Summary Summary { get; set; }
    }

    public class Mode
    {
        public string Type { get; set; }
        public string[] TransportModes { get; set; }
        public string TrafficMode { get; set; }
        public object[] Feature { get; set; }
    }

    public class Summary
    {
        public int Distance { get; set; }
        public int BaseTime { get; set; }
        public string[] Flags { get; set; }
        public string Text { get; set; }
        public int TravelTime { get; set; }
        public string Type { get; set; }
    }

    public class Waypoint
    {
        public string LinkId { get; set; }
        public Mappedposition MappedPosition { get; set; }
        public Originalposition OriginalPosition { get; set; }
        public string Type { get; set; }
        public float Spot { get; set; }
        public string SideOfStreet { get; set; }
        public string MappedRoadName { get; set; }
        public string Label { get; set; }
        public int ShapeIndex { get; set; }
        public string Source { get; set; }
    }

    public class Mappedposition
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Originalposition
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Leg
    {
        public Start Start { get; set; }
        public End End { get; set; }
        public int Length { get; set; }
        public int TravelTime { get; set; }
        public Maneuver[] Maneuver { get; set; }
    }

    public class Start
    {
        public string LinkId { get; set; }
        public Mappedposition1 MappedPosition { get; set; }
        public Originalposition1 OriginalPosition { get; set; }
        public string Type { get; set; }
        public float Spot { get; set; }
        public string SideOfStreet { get; set; }
        public string MappedRoadName { get; set; }
        public string Label { get; set; }
        public int ShapeIndex { get; set; }
        public string Source { get; set; }
    }

    public class Mappedposition1
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Originalposition1
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class End
    {
        public string LinkId { get; set; }
        public Mappedposition2 MappedPosition { get; set; }
        public Originalposition2 OriginalPosition { get; set; }
        public string Type { get; set; }
        public float Spot { get; set; }
        public string SideOfStreet { get; set; }
        public string MappedRoadName { get; set; }
        public string Label { get; set; }
        public int ShapeIndex { get; set; }
        public string Source { get; set; }
    }

    public class Mappedposition2
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Originalposition2
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Maneuver
    {
        public Position Position { get; set; }
        public string Instruction { get; set; }
        public int TravelTime { get; set; }
        public int Length { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
    }

    public class Position
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}