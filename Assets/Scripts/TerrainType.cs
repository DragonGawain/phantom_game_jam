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
    private void OnTriggerEnter2D(Collider2D other) 
    {
        // Player Controller
        // Ennemy

        GameObject go = other.gameObject;  // what game object collided?
        Movement movement = go.GetComponent<Movement>();
        switch (terrainType)
        {
            case TerrainTypes.NORMAL:
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed());
                break;
            case TerrainTypes.SWAMP:
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed()-1);
                break;
            case TerrainTypes.ASPHALT:
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed() + 1);
                break;
        }
    }
}
