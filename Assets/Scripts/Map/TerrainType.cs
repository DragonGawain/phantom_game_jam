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
            TerrainTypes.NORMAL => new Color(1, 1, 1, 0.39f),
            TerrainTypes.ASPHALT => new Color(0, 0, 0, 0.39f),
            TerrainTypes.FOREST => new Color(0.17f, 1, 0, 0.39f),
            TerrainTypes.SWAMP => new Color(0, 1, 0.95f, 0.39f),
            _ => new Color(1, 1, 1),
        };

        terrainLight = GetComponentInChildren<Light2D>();
        if (terrainType == TerrainTypes.FOREST)
            terrainLight.intensity = 0.075f;
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

        GameObject go = other.gameObject; // what game object collided?
        if (!go.TryGetComponent<Movement>(out Movement movement))
            return;
        switch (terrainType)
        {
            case TerrainTypes.NORMAL:
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed());
                break;
            case TerrainTypes.SWAMP:
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed() - 2);
                break;
            case TerrainTypes.ASPHALT:
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed() + 1.5f);
                break;
            case TerrainTypes.FOREST:
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed() - 0.8f);
                // if (go.TryGetComponent<PlayerController>(out PlayerController pc)) // returns a bool value
                // {
                //     go.transform.Find("Flashlight").gameObject.SetActive(true); // activate light
                //     GameObject
                //         .FindGameObjectWithTag("GlobalLight")
                //         .GetComponent<Light2D>()
                //         .intensity = 0.1f;
                // }
                break;
        }
    }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     GameObject go = other.gameObject; // what game object collided?
    //     Movement movement = go.GetComponent<Movement>();
    //     if (terrainType == TerrainTypes.FOREST)
    //     {
    //         if (go.TryGetComponent<PlayerController>(out PlayerController pc))
    //         {
    //             go.transform.Find("Flashlight").gameObject.SetActive(false);
    //             GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light2D>().intensity =
    //                 1;
    //             pc.DeactivateAdvancedFlashlight();
    //         }
    //     }
    // }
}
