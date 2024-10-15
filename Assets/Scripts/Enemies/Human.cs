using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Human : Enemy
{
    // idea: make the ship an enemy be under a common parent.
    // This might even make it possible for us to introduce allies - multiple enemies working together attached to the same ship?
    public Ship ship;

    // public just to expose them in editor for debug purposes
    public List<Item> inventory = new();
    public List<ShipPiece> shipInventory = new();

    [SerializeField]
    int inventorySize = 10;

    [SerializeField]
    int shipInventorySize = 10;

    // Awake is used in the parent 'Enemy', so we'll just use Start instead
    void Start()
    {
        target = ship.GetTransformOfNearestNeededShipPiece(transform, shipInventory);
        hasTarget = true;
        ship.SetHuman(this);
    }

    // inventory management
    // player
    public void AddToInventory(Item newItem)
    {
        if (newItem.GetComponentType() != ComponentType.PLAYER)
            return;
        int size = 0;
        foreach (Item item in inventory)
            size += item.GetSize();
        if (size + newItem.GetSize() <= inventorySize)
            inventory.Add(newItem);
    }

    public void RemoveFromInventory(Item item)
    {
        if (inventory.Contains(item))
            inventory.Remove(item);
    }

    // ship
    public bool AddToShipInventory(ShipPiece newItem)
    {
        if (newItem.GetComponentType() != ComponentType.SHIP)
            return false;
        int size = 0;
        foreach (ShipPiece item in shipInventory)
            size += item.GetSize();
        if (size + newItem.GetSize() <= shipInventorySize)
        {
            shipInventory.Add(newItem);
            return true;
        }
        return false;
    }

    public void RemoveFromShipInventory(ShipPiece item)
    {
        if (shipInventory.Contains(item))
            shipInventory.Remove(item);
    }

    public List<ShipPiece> GetShipInventory() => shipInventory;

    public void CollectedShipPiece()
    {
        int size = 0;
        foreach (ShipPiece item in shipInventory)
            size += item.GetSize();

        if (size > 7)
        {
            target = ship.transform;
            hasTarget = true;
        }
        else
        {
            FindNewShipPiece();
        }
    }

    public void FindNewShipPiece()
    {
        target = ship.GetTransformOfNearestNeededShipPiece(transform, shipInventory);
        hasTarget = true;
    }
}
