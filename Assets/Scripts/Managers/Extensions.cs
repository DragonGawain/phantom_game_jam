using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector3 Rotate(this Vector3 v, float delta)
    {
        delta *= Mathf.Deg2Rad;
        return new Vector3(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta),
            v.z
        );
    }

    public static List<ShipPiece> DeepCopy(this List<ShipPiece> list)
    {
        List<ShipPiece> output = new();
        foreach (var item in list)
            output.Add(item);
        return output;
    }
}

public enum TerrainTypes
{
    NORMAL,
    SWAMP,
    ASPHALT,
    FOREST,
}
