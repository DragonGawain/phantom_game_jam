using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;

public class Ship : MonoBehaviour
{
    static Dictionary<ShipComponents, int> requiredInventory =
        new()
        {
            { ShipComponents.NOSE_GEAR, 1 },
            { ShipComponents.LANDING_GEAR, 1 },
            { ShipComponents.OXYGEN_TANK, 1 },
            { ShipComponents.FUEL_TANK, 3 },
            { ShipComponents.SOLID_BOOSTERS, 2 },
            { ShipComponents.ENGINES, 1 },
            { ShipComponents.RCS, 1 },
            { ShipComponents.WINGS, 2 }
        };
    static Dictionary<ShipComponents, int> componentHpValue =
        new()
        {
            { ShipComponents.NOSE_GEAR, 7 },
            { ShipComponents.LANDING_GEAR, 5 },
            { ShipComponents.OXYGEN_TANK, 3 },
            { ShipComponents.FUEL_TANK, 3 },
            { ShipComponents.SOLID_BOOSTERS, 4 },
            { ShipComponents.ENGINES, 2 },
            { ShipComponents.RCS, 1 },
            { ShipComponents.WINGS, 8 }
        };

    Dictionary<ShipComponents, int> inventory =
        new()
        {
            { ShipComponents.NOSE_GEAR, 0 },
            { ShipComponents.LANDING_GEAR, 0 },
            { ShipComponents.OXYGEN_TANK, 0 },
            { ShipComponents.FUEL_TANK, 0 },
            { ShipComponents.SOLID_BOOSTERS, 0 },
            { ShipComponents.ENGINES, 0 },
            { ShipComponents.RCS, 0 },
            { ShipComponents.WINGS, 0 }
        };

    [SerializeField]
    List<ShipComponents> initialComps = new();
    Human human = null;
    PlayerController player = null;

    static Sprite humanShip1;
    static Sprite humanShip2;

    public int hp = 25;
    int maxHp = 25;

    int iFrameCounter = 0;

    GameObject shipComponentObject;

    ShipAudio shipAudio;

    public bool spec = false;

    private void Awake()
    {
        shipAudio = GetComponent<ShipAudio>();
        if (humanShip1 == null)
        {
            humanShip1 = Resources.Load<Sprite>("Vehicles/Enemy1Ship");
            humanShip2 = Resources.Load<Sprite>("Vehicles/Enemy2Ship");
        }
    }

    private void Start()
    {
        shipComponentObject = Resources.Load<GameObject>("Items/ShipComponent");
        foreach (ShipComponents sp in initialComps)
            AddPieceToShip(sp);
    }

    private void FixedUpdate()
    {
        if (iFrameCounter > 0)
            iFrameCounter--;
    }

    public void AddToInitComps(ShipComponents sc) => initialComps.Add(sc);

    // public Vector3 GetPositionOfNearestNeededShipPiece(
    //     Transform source,
    //     List<ShipComponents> carrying,
    //     int availableSPace
    // ) => GetTransformOfNearestNeededShipPiece(source, carrying, availableSPace).position;

    public bool SetHuman(Human human)
    {
        bool ret = Random.Range(-1f, 1f) < 0;
        if (!spec)
            GetComponent<SpriteRenderer>().sprite = ret ? humanShip1 : humanShip2;
        this.human = human;
        return ret;
    }

    public void SetPlayer(PlayerController player) => this.player = player;

    public Transform GetTransformOfNearestNeededShipPiece(
        Transform source,
        List<ShipComponents> carrying,
        int availableSPace,
        Transform exclude = null
    )
    {
        Transform pos = source;
        List<ShipComponents> potentialTargets = new();

        foreach (ShipComponents sp in Enum.GetValues(typeof(ShipComponents)))
        {
            if (
                Item.shipComponentSizes[sp] <= availableSPace
                && GameObject.FindGameObjectsWithTag(sp.GetEnumDescription()).Length > 0
            )
                for (
                    int i = inventory[sp] + carrying.Where(it => it == sp).Count();
                    i < requiredInventory[sp];
                    i++
                )
                    potentialTargets.Add(sp);
        }
        // Debug.Log("pot targets count: " + potentialTargets.Count);

        if (potentialTargets.Count == 0)
        {
            // This means that an enemy has everything that they need.. They have a complete ship.
            // Maybe this is a loss condition? Or maybe the enemy just despawn? I dunno.

            // return to ship
            Debug.Log("GO BACK TO SHIP");
            return transform;
        }
        else
        {
            int selection = Random.Range(0, potentialTargets.Count);

            // Debug.Log(
            //     "select: "
            //         + selection
            //         + ", val: "
            //         + potentialTargets[selection].GetEnumDescription()
            // );

            // TODO:: make better
            GameObject[] potentialTargetLocs = GameObject.FindGameObjectsWithTag(
                potentialTargets[selection].GetEnumDescription()
            );

            // Debug.Log("num possibilties: " + potentialTargetLocs.Length);

            float dist = int.MaxValue;
            foreach (GameObject ptl in potentialTargetLocs)
            {
                if (ptl.transform == exclude)
                    continue;
                if (Vector3.Distance(ptl.transform.position, source.position) < dist)
                {
                    dist = Vector3.Distance(ptl.transform.position, source.position);
                    pos = ptl.transform;
                }
            }
        }

        return pos;
    }

    public static Dictionary<ShipComponents, int> GetRequiredInvetory() => requiredInventory;

    public Dictionary<ShipComponents, int> Inventory
    {
        get => inventory;
    }

    public int GetMaxHp() => maxHp;

    public void AddPieceToShip(ShipComponents piece, int qt = 1)
    {
        if (inventory.ContainsKey(piece))
            inventory[piece] += qt;
        else
            inventory.Add(piece, qt);
        maxHp += componentHpValue[piece];
        hp += componentHpValue[piece];

        if (CheckShipCompletionStatus())
        {
            // If a human completes their ship, they'll just, hang around for a while? :shrug:
            if (player == null)
                Debug.Log(
                    "<color=orange>Enemy " + human.name + " has completed their ship!</color>"
                );
            else
            {
                player.InitializeEndingSequence();
                transform.parent = player.transform;
                transform.localPosition = Vector3.zero;
                Debug.Log("<color=orange>The player has completed their ship!</color>");
                GetComponent <ParticleSystem>().Play ();
                ParticleSystem.EmissionModule em = GetComponent<ParticleSystem>().emission;
                em.enabled = true;
            }
        }
    }

    public bool RemovePieceFromShip(ShipComponents piece, int qt = 1)
    {
        if (inventory.ContainsKey(piece))
        {
            if (inventory[piece] >= qt)
            {
                inventory[piece] = Mathf.Max(inventory[piece] - qt, 0);
                return true;
            }
            // even if not enough could be removed, remove all that you can -> this is not relayed back...
            inventory[piece] = Mathf.Max(inventory[piece] - qt, 0);
        }
        return false;
    }

    bool CheckShipCompletionStatus()
    {
        foreach (KeyValuePair<ShipComponents, int> sp in inventory)
        {
            if (sp.Value < requiredInventory[sp.Key])
                return false;
        }
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Human>(out Human human))
        {
            if (human == this.human)
            {
                List<ShipComponents> toAdd = human.GetShipInventory().DeepCopy();
                foreach (ShipComponents sp in toAdd)
                {
                    human.RemoveFromShipInventory(sp);
                    AddPieceToShip(sp);
                }
                human.CollectedShipPiece();
            }
        }
        else if (other.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            if (pc == this.player)
            {
                List<ShipComponents> toAdd = pc.GetShipInventory().DeepCopy();
                foreach (ShipComponents sp in toAdd)
                {
                    pc.RemoveFromShipInventory(sp);
                    AddPieceToShip(sp);
                }

                if (PlayerController.isEndingSequence)
                    return;

                if (pc.GetQuest1Target() == null)
                {
                    pc.SetQuest1Target(
                        GetTransformOfNearestNeededShipPiece(
                            pc.transform,
                            new(),
                            pc.GetAvailableShipInventory(),
                            pc.GetQuest2Target()
                        )
                    );
                }
                if (pc.GetQuest2Target() == null)
                {
                    pc.SetQuest2Target(
                        GetTransformOfNearestNeededShipPiece(
                            pc.transform,
                            new(),
                            pc.GetAvailableShipInventory(),
                            pc.GetQuest1Target()
                        )
                    );
                }

                if (pc.GetQuest1Target() != null && pc.GetQuest2Target() != null)
                    UIManager.OpenShipInventory(
                        inventory,
                        maxHp,
                        hp,
                        pc.GetQuest1ShipComponentType(),
                        pc.GetQuest2ShipComponentType()
                    );
                else if (pc.GetQuest1Target() != null && pc.GetQuest2Target() == null)
                    UIManager.OpenShipInventory(
                        inventory,
                        maxHp,
                        hp,
                        pc.GetQuest1ShipComponentType()
                    );
                else if (pc.GetQuest1Target() == null && pc.GetQuest2Target() != null)
                    UIManager.OpenShipInventory(
                        inventory,
                        maxHp,
                        hp,
                        pc.GetQuest2ShipComponentType()
                    );
                else if (pc.GetQuest1Target() == null && pc.GetQuest2Target() == null)
                    UIManager.OpenShipInventory(inventory, maxHp, hp);
            }
        }
        else if (other.CompareTag("Bullet") || other.CompareTag("EvilBullet"))
        {
            if (
                (human != null && other.GetComponent<Bullet>().GetShooterId() == human.GetId())
                || (player != null && other.CompareTag("Bullet"))
            )
                return;
            TakeDamage(other.GetComponent<Bullet>().GetBulletDamage());
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Alien"))
        {
            TakeDamage(other.GetComponentInChildren<Enemy>().GetDamage());
            if (other.gameObject != null)
            other.GetComponentInChildren<Alien>().PlayAttackSound();
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Alien"))
        {
            if (iFrameCounter > 0)
                return;
            TakeDamage(other.gameObject.GetComponentInChildren<Enemy>().GetDamage());
            other.gameObject.GetComponentInChildren<Alien>().PlayAttackSound();
        }
    }

    void TakeDamage(int amt)
    {
        if (iFrameCounter > 0)
            return;

        iFrameCounter = 100; // 50 FUs per second => 100 / 50 = 2 seconds of iFrames
        hp -= amt;
        if (player != null && PlayerController.isEndingSequence)
        {
            player.EndSequenceDamage(hp);
        }
        if (player != null && !PlayerController.isEndingSequence)
            shipAudio.TakeDamageShortSound();
        if (hp <= 0)
        {
            if (player != null)
            {
                Debug.Log(
                    "<color=red>THE PLAYER'S SHIP HAS BEEN DESTROYED - LOSS CONDITION</color>"
                );
                UIManager.LoseCondition("Your ship has been blown up!");
            }
            // for simplicity's sake, when a ship dies, it's human will die with it
            if (human != null)
                Destroy(human.gameObject);
            GameObject component;
            foreach (ShipComponents sc in Enum.GetValues(typeof(ShipComponents)))
            {
                for (int i = 0; i < inventory[sc]; i++)
                {
                    if (Random.Range(0f, 1f) < 0.15f) // 15% chance for every component (number chosen semi-arbitrarily)
                    {
                        component = Instantiate(
                            shipComponentObject,
                            new(
                                Random.Range(transform.position.x - 5f, transform.position.x + 5f),
                                Random.Range(transform.position.y - 5f, transform.position.y + 5f),
                                0
                            ),
                            Quaternion.identity
                        );
                        component.GetComponent<ShipPiece>().SetSpecificType(sc);
                    }
                }
            }
            Destroy(gameObject);
        }
        // marked as an else if to imply that the ship hp must also be above 0 in order for this to happen
        // Send the human to the ship
        else if (human != null)
        {
            shipAudio.TakeDamageShortSound();
            human.SetTarget(transform);
            human.SetCombatState(Enemy.CombatState.FORCE_ARRIVE);
        }
    }
}
