namespace BL.Service.Map;
public enum RouterMode
{
    Car,
    Truck,
    Pedestrian,
    Bicycle,
    Scooter,
    Taxi,
    Bus,
    PrivateBus
}

public static class RouterModeExtensions
{
    public static string ToLower(this RouterMode routerMode)
    {
        var str = routerMode.ToString();
        return char.ToLowerInvariant(str[0]) + str[1..];
    }
}