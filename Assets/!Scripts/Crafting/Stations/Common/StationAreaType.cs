using UnityEngine;

public enum StationAreaType : byte
{
    None = 0,
    PrepBoard = 1,
    TemperaturePot = 2,
    ArcaneCircle = 3,
    Cauldron = 4
}

public enum StationType
{
    CuttingBoard = 0,
    Mortar = 1,
    TemperaturePot = 2,
    ArcaneCircle = 3,
    Cauldron = 4,
    Bottler = 5
}

public static class StationUtils
{
    public static int GetStationAreaID(this StationAreaType stationArea)
    {
        return (int) stationArea - 1;
    }

    public static int GetStationID(this StationType station)
    {
        return (int) station;
    }
}
