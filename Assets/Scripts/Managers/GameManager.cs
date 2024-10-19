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
            { TerrainTypes.SWAMP, (1, 1, 4, 1) },
            { TerrainTypes.ASPHALT, (3, 1, 1, 1) },
            { TerrainTypes.FOREST, (2, 2, 2, 1) },
        };

    // Doing it this way is making it slightly harder to expand the number of terrains that we have, but it shouldn't be too big of a deal...
    // Just know that this isn't the best way to do this, but it's fast and easy, so I'll take the technical debt.
    static readonly Dictionary<TerrainTypes, (ShipComponents, ShipComponents)> terrainCompPrefs =
        new()
        {
            { TerrainTypes.NORMAL, (ShipComponents.FUEL_TANK, ShipComponents.RCS) },
            { TerrainTypes.SWAMP, (ShipComponents.NOSE_GEAR, ShipComponents.SOLID_BOOSTERS) },
            { TerrainTypes.ASPHALT, (ShipComponents.ENGINES, ShipComponents.LANDING_GEAR) },
            { TerrainTypes.FOREST, (ShipComponents.WINGS, ShipComponents.OXYGEN_TANK) },
        };

    GameObject alienBase;
    GameObject ship;
    GameObject human;
    GameObject shipComponentObject;
    GameObject advGunObject;
    GameObject bootsObject;
    GameObject flashlightObject;

    private void Awake()
    {
        alienBase = Resources.Load<GameObject>("AlienBase");
        ship = Resources.Load<GameObject>("HumanShip");
        human = Resources.Load<GameObject>("HumanEnemy2");
        shipComponentObject = Resources.Load<GameObject>("Items/ShipComponent");
        advGunObject = Resources.Load<GameObject>("Items/AdvGun");
        bootsObject = Resources.Load<GameObject>("Items/Boots");
        flashlightObject = Resources.Load<GameObject>("Items/Flashlight");
    }

    private void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        SpawnInEnemiesInTerrain();
        SpawnShipComponents();
    }

    void SpawnInEnemiesInTerrain()
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

                switch (terrainType)
                {
                    case TerrainTypes.SWAMP:
                        Instantiate(
                            bootsObject,
                            new(terrain.transform.position.x, terrain.transform.position.y, 0),
                            Quaternion.identity
                        );
                        break;
                    case TerrainTypes.ASPHALT:
                        Instantiate(
                            advGunObject,
                            new(terrain.transform.position.x, terrain.transform.position.y, 0),
                            Quaternion.identity
                        );
                        break;
                    case TerrainTypes.FOREST:
                        Instantiate(
                            flashlightObject,
                            new(terrain.transform.position.x, terrain.transform.position.y, 0),
                            Quaternion.identity
                        );
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void SpawnShipComponents()
    {
        // To make this flexible, I'm going to base it off of the size of the biomes
        // For now, there are 4 biomes and 8 types of ship ocmponents, so I'll make each biome favor two types
        // A ship needs a total of 12 components. AI ships start with 3, so every AI ship will need 9
        // Since every biome will have at least 1 of each type of enemy, if a standard sized biome (basing it on 100x100)
        // has 8-12 components, that should be about fine, I think?
        // Therefore, in any given biome, the density will be determined by area/1000 +/- (area/1000)/5, with rounding to nearest int

        // P.S. I'm doing absolutely no checking to ensure that things don't spawn in bad spots (i.e. directly on top of other things)
        // This also applies for the enemies. A human ship and alien base can theoretically spawn in the exact same spot

        // Yes I'm doing this exact same foreach loop again, which means I'm fetching all the terrains a second time.
        // This is suboptimal, but the game is small scale enough that it doesn't really matter, and this makes it more human readable IMO

        Vector2 xBounds;
        Vector2 yBounds;

        float area;
        GameObject[] terrains;
        int nbComps;
        float temp;

        GameObject component;

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

                area = terrain.transform.localScale.x * terrain.transform.localScale.y;
                temp = area / 1000;
                nbComps = Mathf.RoundToInt(temp + Random.Range(-temp / 5, temp / 5));
                for (int i = 0; i < nbComps; i++)
                {
                    // ensure that at least half of the components are of the prefered type
                    if (i < Mathf.CeilToInt(nbComps / 2) + 1)
                    {
                        component = Instantiate(
                            shipComponentObject,
                            new(
                                Random.Range(xBounds.x, xBounds.y),
                                Random.Range(yBounds.x, yBounds.y),
                                0
                            ),
                            Quaternion.identity
                        );
                        if (i % 2 == 0)
                            component
                                .GetComponent<ShipPiece>()
                                .SetSpecificType(terrainCompPrefs[terrainType].Item1);
                        else
                            component
                                .GetComponent<ShipPiece>()
                                .SetSpecificType(terrainCompPrefs[terrainType].Item2);
                    }
                    else
                        Instantiate(
                            shipComponentObject,
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
