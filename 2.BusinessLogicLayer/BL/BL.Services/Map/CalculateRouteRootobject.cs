using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services.Map {

    public class CalculateRouteRootobject {
        public Response response { get; set; }
    }

    public class Response {
        public Metainfo metaInfo { get; set; }
        public Route[] route { get; set; }
        public string language { get; set; }
    }

    public class Metainfo {
        public DateTime timestamp { get; set; }
        public string mapVersion { get; set; }
        public string moduleVersion { get; set; }
        public string interfaceVersion { get; set; }
        public string[] availableMapVersion { get; set; }
    }

    public class Route {
        public Waypoint[] waypoint { get; set; }
        public Mode mode { get; set; }
        public Leg[] leg { get; set; }
        public Summary summary { get; set; }
    }

    public class Mode {
        public string type { get; set; }
        public string[] transportModes { get; set; }
        public string trafficMode { get; set; }
        public object[] feature { get; set; }
    }

    public class Summary {
        public int distance { get; set; }
        public int baseTime { get; set; }
        public string[] flags { get; set; }
        public string text { get; set; }
        public int travelTime { get; set; }
        public string _type { get; set; }
    }

    public class Waypoint {
        public string linkId { get; set; }
        public Mappedposition mappedPosition { get; set; }
        public Originalposition originalPosition { get; set; }
        public string type { get; set; }
        public float spot { get; set; }
        public string sideOfStreet { get; set; }
        public string mappedRoadName { get; set; }
        public string label { get; set; }
        public int shapeIndex { get; set; }
        public string source { get; set; }
    }

    public class Mappedposition {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class Originalposition {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class Leg {
        public Start start { get; set; }
        public End end { get; set; }
        public int length { get; set; }
        public int travelTime { get; set; }
        public Maneuver[] maneuver { get; set; }
    }

    public class Start {
        public string linkId { get; set; }
        public Mappedposition1 mappedPosition { get; set; }
        public Originalposition1 originalPosition { get; set; }
        public string type { get; set; }
        public float spot { get; set; }
        public string sideOfStreet { get; set; }
        public string mappedRoadName { get; set; }
        public string label { get; set; }
        public int shapeIndex { get; set; }
        public string source { get; set; }
    }

    public class Mappedposition1 {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class Originalposition1 {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class End {
        public string linkId { get; set; }
        public Mappedposition2 mappedPosition { get; set; }
        public Originalposition2 originalPosition { get; set; }
        public string type { get; set; }
        public float spot { get; set; }
        public string sideOfStreet { get; set; }
        public string mappedRoadName { get; set; }
        public string label { get; set; }
        public int shapeIndex { get; set; }
        public string source { get; set; }
    }

    public class Mappedposition2 {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class Originalposition2 {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class Maneuver {
        public Position position { get; set; }
        public string instruction { get; set; }
        public int travelTime { get; set; }
        public int length { get; set; }
        public string id { get; set; }
        public string _type { get; set; }
    }

    public class Position {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }
}