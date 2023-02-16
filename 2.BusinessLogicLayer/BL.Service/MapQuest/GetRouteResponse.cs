using System.Text.Json.Serialization;

namespace BL.Service.MapQuest;

public class GetRouteResponse
{

    [JsonPropertyName("route")]
    public Route Route { get; set; }
    [JsonPropertyName("info")]
    public Info Info { get; set; }
}

public class Route
{
    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; }
    [JsonPropertyName("realTime")]
    public int RealTime { get; set; }
    [JsonPropertyName("distance")]
    public float Distance { get; set; }
    [JsonPropertyName("time")]
    public int Time { get; set; }
    [JsonPropertyName("formattedTime")]
    public string FormattedTime { get; set; }
    [JsonPropertyName("hasHighway")]
    public bool HasHighway { get; set; }
    [JsonPropertyName("hasTollRoad")]
    public bool HasTollRoad { get; set; }
    [JsonPropertyName("hasBridge")]
    public bool HasBridge { get; set; }
    [JsonPropertyName("hasSeasonalClosure")]
    public bool HasSeasonalClosure { get; set; }
    [JsonPropertyName("hasTunnel")]
    public bool HasTunnel { get; set; }
    [JsonPropertyName("hasFerry")]
    public bool HasFerry { get; set; }
    [JsonPropertyName("hasUnpaved")]
    public bool HasUnpaved { get; set; }
    [JsonPropertyName("hasTimedRestriction")]
    public bool HasTimedRestriction { get; set; }
    [JsonPropertyName("hasCountryCross")]
    public bool HasCountryCross { get; set; }
    [JsonPropertyName("leg")]
    public Leg[] Leg { get; set; }
    [JsonPropertyName("locations")]
    public Location[] Locations { get; set; }
}

public class Leg
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    [JsonPropertyName("hasHighway")]
    public bool HasHighway { get; set; }
    [JsonPropertyName("hasTollRoad")]
    public bool HasTollRoad { get; set; }
    [JsonPropertyName("hasBridge")]
    public bool HasBridge { get; set; }
    [JsonPropertyName("hasSeasonalClosure")]
    public bool HasSeasonalClosure { get; set; }
    [JsonPropertyName("hasTunnel")]
    public bool HasTunnel { get; set; }
    [JsonPropertyName("hasFerry")]
    public bool HasFerry { get; set; }
    [JsonPropertyName("hasUnpaved")]
    public bool HasUnpaved { get; set; }
    [JsonPropertyName("hasTimedRestriction")]
    public bool HasTimedRestriction { get; set; }
    [JsonPropertyName("hasCountryCross")]
    public bool HasCountryCross { get; set; }
    [JsonPropertyName("distance")]
    public float Distance { get; set; }
    [JsonPropertyName("time")]
    public int Time { get; set; }
    [JsonPropertyName("formattedTime")]
    public string FormattedTime { get; set; }
    [JsonPropertyName("origIndex")]
    public int OrigIndex { get; set; }
    [JsonPropertyName("origNarrative")]
    public string OrigNarrative { get; set; }
    [JsonPropertyName("destIndex")]
    public int DestIndex { get; set; }
    [JsonPropertyName("destNarrative")]
    public string DestNarrative { get; set; }
    [JsonPropertyName("maneuver")]
    public Maneuver[] Maneuver { get; set; }
}

public class Maneuver
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    [JsonPropertyName("distance")]
    public float Distance { get; set; }
    [JsonPropertyName("narrative")]
    public string Narrative { get; set; }
    [JsonPropertyName("time")]
    public int Time { get; set; }
    [JsonPropertyName("direction")]
    public int Direction { get; set; }
    [JsonPropertyName("directionName")]
    public int DirectionName { get; set; }
    [JsonPropertyName("formattedTime")]
    public string FormattedTime { get; set; }
    [JsonPropertyName("transportMode")]
    public string TransportMode { get; set; }
    [JsonPropertyName("startPoint")]
    public LatLng StartPoint { get; set; }
    [JsonPropertyName("turnType")]
    public int TurnType { get; set; }
}

public class LatLng
{
    [JsonPropertyName("lat")]
    public float Lat { get; set; }
    [JsonPropertyName("lng")]
    public float Lng { get; set; }
}

public class Location
{
    [JsonPropertyName("street")]
    public string Street { get; set; }
    [JsonPropertyName("adminArea6")]
    public string AdminArea6 { get; set; }
    [JsonPropertyName("adminArea6Type")]
    public string AdminArea6Type { get; set; }
    [JsonPropertyName("adminArea5")]
    public string AdminArea5 { get; set; }
    [JsonPropertyName("adminArea5Type")]
    public string AdminArea5Type { get; set; }
    [JsonPropertyName("adminArea4")]
    public string AdminArea4 { get; set; }
    [JsonPropertyName("adminArea4Type")]
    public string AdminArea4Type { get; set; }
    [JsonPropertyName("adminArea3")]
    public string AdminArea3 { get; set; }
    [JsonPropertyName("adminArea3Type")]
    public string AdminArea3Type { get; set; }
    [JsonPropertyName("adminArea1")]
    public string AdminArea1 { get; set; }
    [JsonPropertyName("adminArea1Type")]
    public string AdminArea1Type { get; set; }
    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; }
    [JsonPropertyName("geocodeQualityCode")]
    public string GeocodeQualityCode { get; set; }
    [JsonPropertyName("geocodeQuality")]
    public string GeocodeQuality { get; set; }
    [JsonPropertyName("dragPoint")]
    public bool DragPoint { get; set; }
    [JsonPropertyName("sideOfStreet")]
    public string SideOfStreet { get; set; }
    [JsonPropertyName("linkId")]
    public string LinkId { get; set; }
    [JsonPropertyName("unknownInput")]
    public string UnknownInput { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("latLng")]
    public LatLng LatLng { get; set; }
    [JsonPropertyName("displayLatLng")]
    public LatLng DisplayLatLng { get; set; }
}

public class Info
{

    [JsonPropertyName("statuscode")]
    public int Statuscode { get; set; }

    [JsonPropertyName("copyright")]
    public Copyright Copyright { get; set; }

    [JsonPropertyName("messages")]
    public string[] Messages { get; set; }
}

public class Copyright
{

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; }

    [JsonPropertyName("imageAltText")]
    public string ImageAltText { get; set; }
}