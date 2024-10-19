using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class TerrainType : MonoBehaviour
{
    [SerializeField]
    TerrainTypes terrainType;

    [SerializeField]
    bool isSpecificItem = false;

    Light2D terrainLight;

    // int maxMoveSpeedModifier;

    private void Awake()
    {
        if (!isSpecificItem)
            terrainType = (TerrainTypes)Random.Range(0, Enum.GetNames(typeof(TerrainTypes)).Length);
        gameObject.tag = terrainType.GetEnumDescription();
        GetComponent<SpriteRenderer>().color = terrainType switch
        {
            TerrainTypes.NORMAL => new Color(0.68627f, 0.68627f, 0.68627f),
            TerrainTypes.ASPHALT => new Color(0.21176f, 0.21176f, 0.21176f),
            TerrainTypes.FOREST => new Color(0.21569f, 0.28627f, 0.21176f),
            TerrainTypes.SWAMP => new Color(0.21176f, 0.68627f, 0.65490f),
            _ => new Color(1, 1, 1),
        };

        terrainLight = GetComponentInChildren<Light2D>();
        if (terrainType == TerrainTypes.FOREST)
            terrainLight.intensity = 0.25f;
    }

    public TerrainTypes GetTerrainType()
    {
        return terrainType;
    }

    // public int GetMaxMoveSpeedModifier()
    // {
    //     return maxMoveSpeedModifier;
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player Controller
        // Ennemy
        bool isPlayer = other.CompareTag("Player");
        if (isPlayer && PlayerController.isEndingSequence)
            return;

        GameObject go = other.gameObject; // what game object collided?
        if (!go.TryGetComponent<Movement>(out Movement movement))
            return;
        switch (terrainType)
        {
            case TerrainTypes.NORMAL:
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed());
                if (isPlayer)
                    other.GetComponent<PlayerAudio>().SetWalkingSound(TerrainTypes.NORMAL);
                break;
            case TerrainTypes.SWAMP:
                movement.SetMaxMoveSpeed(
                    movement.GetOriginalSpeed() + movement.GetSwampSpeedModifier()
                );
                if (isPlayer)
                    other.GetComponent<PlayerAudio>().SetWalkingSound(TerrainTypes.SWAMP);
                break;
            case TerrainTypes.ASPHALT:
                movement.SetMaxMoveSpeed(
                    movement.GetOriginalSpeed() + movement.GetAsphaltSpeedModifier()
                );
                if (isPlayer)
                    other.GetComponent<PlayerAudio>().SetWalkingSound(TerrainTypes.ASPHALT);
                break;
            case TerrainTypes.FOREST:
                movement.SetMaxMoveSpeed(
                    movement.GetOriginalSpeed() + movement.GetForestSpeedModifier()
                );
                if (isPlayer)
                    other.GetComponent<PlayerAudio>().SetWalkingSound(TerrainTypes.FOREST);
                break;
        }
    }
}
