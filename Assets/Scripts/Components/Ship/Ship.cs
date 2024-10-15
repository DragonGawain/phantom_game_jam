using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEditor;

public class Ship : MonoBehaviour
{
    static readonly Dictionary<ShipComponents, int> requiredInventory =
        new()
        {
            { ShipComponents.NOSE_GEAR, 1 },
            { ShipComponents.LANDING_GEAR, 1 },
            { ShipComponents.OXYGEN_TANK, 1 },
            { ShipComponents.FUEL_TANK, 3 },
            { ShipComponents.SOLID_BOOSTERS, 2 },
            { ShipComponents.ENGINES, 1 },
            { ShipComponents.RCS, 1 }
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
            { ShipComponents.RCS, 0 }
        };

    Human human = null;
    PlayerController player = null;

    public Vector3 GetPositionOfNearestNeededShipPiece(
        Transform source,
        List<ShipComponents> carrying,
        int availableSPace
    ) => GetTransformOfNearestNeededShipPiece(source, carrying, availableSPace).position;

    public void SetHuman(Human human) => this.human = human;

    public void SetPlayer(PlayerController player) => this.player = player;

    public Transform GetTransformOfNearestNeededShipPiece(
        Transform source,
        List<ShipComponents> carrying,
        int availableSPace
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

        if (potentialTargets.Count == 0)
        {
            // This means that an enemy has everything that they need.. They have a complete ship.
            // Maybe this is a loss condition? Or maybe the enemy just despawn? I dunno.
        }
        else
        {
            int selection = Random.Range(0, potentialTargets.Count);

            // TODO:: make better
            GameObject[] potentialTargetLocs = GameObject.FindGameObjectsWithTag(
                potentialTargets[selection].GetEnumDescription()
            );

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

    public void AddPieceToShip(ShipComponents piece, int qt = 1)
    {
        if (inventory.ContainsKey(piece))
            inventory[piece] += qt;
        else
            inventory.Add(piece, qt);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Human>(out Human human))
        {
            if (human = this.human)
            {
                List<ShipComponents> toAdd = human.GetShipInventory().DeepCopy();
                foreach (ShipComponents sp in toAdd)
                {
                    human.RemoveFromShipInventory(sp);
                    AddPieceToShip(sp);
                }
                human.FindNewShipPiece();
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

                foreach (KeyValuePair<ShipComponents, int> sp in inventory)
                {
                    Debug.Log(sp.Key + " -> " + sp.Value);
                }
            }
        }
    }
}
