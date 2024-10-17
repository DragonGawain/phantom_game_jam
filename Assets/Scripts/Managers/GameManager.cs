using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    static readonly Dictionary<TerrainTypes, (int, int, int, int)> spawnCounts =
        new()
        {
            { TerrainTypes.NORMAL, (2, 1, 2, 1) },
            { TerrainTypes.SWAMP, (1, 1, 3, 1) },
            { TerrainTypes.ASPHALT, (3, 1, 1, 1) },
            { TerrainTypes.FOREST, (1, 2, 2, 1) },
        };

    GameObject alienBase;
    GameObject ship;
    GameObject human;

    private void Awake()
    {
        alienBase = Resources.Load<GameObject>("AlienBase");
        ship = Resources.Load<GameObject>("HumanShip");
        human = Resources.Load<GameObject>("HumanEnemy2");
    }

    private void Start()
    {
        spawnInEnemiesInTerrain();
    }

    public void spawnInEnemiesInTerrain()
    {
        Vector2 xBounds;
        Vector2 yBounds;
        int nbHumans;
        int nbAliens;
        Vector3 pos;

        GameObject[] terrains;
        GameObject enemy;
        GameObject eBase;
        Ship temp;
        foreach (TerrainTypes terrainType in Enum.GetValues(typeof(TerrainTypes)))
        {
            terrains = GameObject.FindGameObjectsWithTag(terrainType.GetEnumDescription());
            foreach (GameObject terrain in terrains)
            {
                // determine bounds - all terrains will be rectangles
                xBounds = new(
                    terrain.transform.position.x - terrain.transform.localScale.x / 2f,
                    terrain.transform.position.x + terrain.transform.localScale.x / 2f
                );
                yBounds = new(
                    terrain.transform.position.y - terrain.transform.localScale.y / 2f,
                    terrain.transform.position.y + terrain.transform.localScale.y / 2f
                );

                nbHumans = Mathf.Clamp(
                    spawnCounts[terrainType].Item1
                        + Random.Range(
                            -spawnCounts[terrainType].Item2,
                            spawnCounts[terrainType].Item2
                        ),
                    0,
                    99
                );
                nbAliens = Mathf.Clamp(
                    spawnCounts[terrainType].Item3
                        + Random.Range(
                            -spawnCounts[terrainType].Item4,
                            spawnCounts[terrainType].Item4
                        ),
                    0,
                    99
                );
                for (int i = 0; i < nbHumans; i++)
                {
                    pos = new(
                        Random.Range(xBounds.x, xBounds.y),
                        Random.Range(yBounds.x, yBounds.y),
                        0
                    );
                    enemy = Instantiate(human, pos, Quaternion.identity);
                    eBase = Instantiate(ship, pos, Quaternion.identity);
                    enemy.GetComponentInChildren<Human>().SetShip(eBase.GetComponent<Ship>());
                    temp = eBase.GetComponent<Ship>();
                    for (int j = 0; j < 3; j++)
                        temp.AddPieceToShip(
                            (ShipComponents)
                                Random.Range(0, Enum.GetNames(typeof(ShipComponents)).Length)
                        );
                }
                for (int i = 0; i < nbAliens; i++)
                {
                    Instantiate(
                        alienBase,
                        new(
                            Random.Range(xBounds.x, xBounds.y),
                            Random.Range(yBounds.x, yBounds.y),
                            0
                        ),
                        Quaternion.identity
                    );
                }
            }
        }
    }
}
