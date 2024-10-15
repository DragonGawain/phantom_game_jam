using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class Ship : MonoBehaviour
{
    static readonly Dictionary<ShipPieces, int> requiredInventory =
        new()
        {
            { ShipPieces.NOSE_GEAR, 1 },
            { ShipPieces.LANDING_GEAR, 1 },
            { ShipPieces.OXYGEN_TANK, 1 },
            { ShipPieces.FUEL_TANK, 3 },
            { ShipPieces.SOLID_BOOSTERS, 2 },
            { ShipPieces.ENGINES, 1 },
            { ShipPieces.RCS, 1 }
        };

    Dictionary<ShipPieces, int> inventory =
        new()
        {
            { ShipPieces.NOSE_GEAR, 0 },
            { ShipPieces.LANDING_GEAR, 0 },
            { ShipPieces.OXYGEN_TANK, 0 },
            { ShipPieces.FUEL_TANK, 0 },
            { ShipPieces.SOLID_BOOSTERS, 0 },
            { ShipPieces.ENGINES, 0 },
            { ShipPieces.RCS, 0 }
        };

    Human human;

    public Vector3 GetPositionOfNearestNeededShipPiece(
        Transform source,
        List<ShipPiece> carrying
    ) => GetTransformOfNearestNeededShipPiece(source, carrying).position;

    public void SetHuman(Human human) => this.human = human;

    public Transform GetTransformOfNearestNeededShipPiece(
        Transform source,
        List<ShipPiece> carrying
    )
    {
        Transform pos = source;
        List<ShipPieces> potentialTargets = new();

        foreach (ShipPieces sp in Enum.GetValues(typeof(ShipPieces)))
            for (
                int i = inventory[sp] + carrying.Where(it => it.GetShipPiece() == sp).Count();
                i < requiredInventory[sp];
                i++
            )
                potentialTargets.Add(sp);

        if (potentialTargets.Count == 0)
        {
            // This means that an enemy has everything that they need.. They have a complete ship.
            // Maybe this is a loss condition? Or maybe the enemy just despawn? I dunno.
        }
        else
        {
            int selection = Random.Range(0, potentialTargets.Count);
            Debug.Log("selection: " + selection);
            Debug.Log(potentialTargets + "\n" + potentialTargets[selection]);
            GameObject[] potentialTargetLocs = potentialTargets[selection] switch
            {
                ShipPieces.NOSE_GEAR => GameObject.FindGameObjectsWithTag("nose_gear"),
                ShipPieces.LANDING_GEAR => GameObject.FindGameObjectsWithTag("landing_gear"),
                ShipPieces.OXYGEN_TANK => GameObject.FindGameObjectsWithTag("oxygen_tank"),
                ShipPieces.FUEL_TANK => GameObject.FindGameObjectsWithTag("fuel_tank"),
                ShipPieces.SOLID_BOOSTERS => GameObject.FindGameObjectsWithTag("solid_boosters"),
                ShipPieces.ENGINES => GameObject.FindGameObjectsWithTag("engines"),
                ShipPieces.RCS => GameObject.FindGameObjectsWithTag("rcs"),
                _ => null
            };

            float dist = int.MaxValue;
            foreach (GameObject ptl in potentialTargetLocs)
            {
                if (Vector3.Distance(ptl.transform.position, source.position) < dist)
                {
                    dist = Vector3.Distance(ptl.transform.position, source.position);
                    pos = ptl.transform;
                }
            }
        }

        return pos;
    }

    public void AddPieceToShip(ShipPieces piece, int qt = 1)
    {
        if (inventory.ContainsKey(piece))
            inventory[piece] += qt;
        else
            inventory.Add(piece, qt);
    }

    public bool RemovePieceFromShip(ShipPieces piece, int qt = 1)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Human>(out Human human))
        {
            if (human = this.human)
            {
                List<ShipPiece> toAdd = human.GetShipInventory().DeepCopy();
                foreach (ShipPiece sp in toAdd)
                {
                    human.RemoveFromShipInventory(sp);
                    AddPieceToShip(sp.GetShipPiece());
                }
                human.FindNewShipPiece();
            }
        }
    }
}
