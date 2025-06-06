using UnityEngine;

public enum StationAreaType : byte
{
    None = 0,
    PrepBoard = 1,
    TemperaturePot = 2,
    ArcaneCircle = 3,
    Cauldron = 4
}

public static class StationUtils
{
    public static int GetStationID(this StationAreaType stationArea)
    {
        return (int) stationArea - 1;
    }
}
