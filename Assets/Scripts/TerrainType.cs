using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType : MonoBehaviour
{
    public TerrainTypes terrainType;

    int maxMoveSpeedModifier;

    public TerrainTypes GetTerrainType()
    {
        return terrainType;
    }

    public int GetMaxMoveSpeedModifier()
    {
        return maxMoveSpeedModifier;
    }
}
