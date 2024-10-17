using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TerrainType : MonoBehaviour
{
    [SerializeField]
    TerrainTypes terrainType;

    // int maxMoveSpeedModifier;

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
                Debug.Log("N");
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed());
                break;
            case TerrainTypes.SWAMP:
                Debug.Log("S");
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed() - 1);
                break;
            case TerrainTypes.ASPHALT:
                Debug.Log("A");
                movement.SetMaxMoveSpeed(movement.GetOriginalSpeed() + 1);
                break;
            case TerrainTypes.FOREST:
                Debug.Log("F");
                if (go.TryGetComponent<PlayerController>(out PlayerController pc)) // returns a bool value
                {
                    go.transform.Find("Flashlight").gameObject.SetActive(true); // activate light
                    GameObject
                        .FindGameObjectWithTag("GlobalLight")
                        .GetComponent<Light2D>()
                        .intensity = 0.1f;
                }
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject go = other.gameObject; // what game object collided?
        Movement movement = go.GetComponent<Movement>();
        if (terrainType == TerrainTypes.FOREST)
        {
            if (go.TryGetComponent<PlayerController>(out PlayerController pc))
            {
                go.transform.Find("Flashlight").gameObject.SetActive(false);
                GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light2D>().intensity =
                    1;
            }
        }
    }
}
