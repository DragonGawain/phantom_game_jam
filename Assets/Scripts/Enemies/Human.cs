using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Human : Enemy
{
    // idea: make the ship an enemy be under a common parent.
    // This might even make it possible for us to introduce allies - multiple enemies working together attached to the same ship?
    public Ship ship;

    // HACK:: public just to expose them in the inspector
    public List<PlayerComponents> inventory = new();
    public List<ShipComponents> shipInventory = new();

    [SerializeField]
    int inventorySize = 10;

    [SerializeField]
    int shipInventorySize = 10;

    // Awake is used in the parent 'Enemy', so we'll just use Start instead
    void Start()
    {
        FindNewShipPiece();
        ship.SetHuman(this);
        fleePoint = ship.transform.position;
    }

    protected override void DoFixedUpdate()
    {
        //
    }

    // inventory management
    // player
    public void AddToInventory(PlayerComponents newItem)
    {
        int size = 0;
        foreach (PlayerComponents item in inventory)
            size += Item.playerComponentSizes[item];
        if (size + Item.playerComponentSizes[newItem] <= inventorySize)
            inventory.Add(newItem);
    }

    public void RemoveFromInventory(PlayerComponents item)
    {
        if (inventory.Contains(item))
            inventory.Remove(item);
    }

    // ship
    public bool AddToShipInventory(ShipComponents newItem)
    {
        int size = GetSpaceLeftInShipInv();

        if (size + Item.shipComponentSizes[newItem] <= shipInventorySize)
        {
            shipInventory.Add(newItem);
            return true;
        }
        return false;
    }

    public void RemoveFromShipInventory(ShipComponents item)
    {
        if (shipInventory.Contains(item))
            shipInventory.Remove(item);
    }

    public List<ShipComponents> GetShipInventory() => shipInventory;

    public void CollectedShipPiece()
    {
        int size = GetSpaceLeftInShipInv();

        if (size > 7)
            target = ship.transform;
        else
            FindNewShipPiece();
    }

    public void FindNewShipPiece()
    {
        target = ship.GetTransformOfNearestNeededShipPiece(
            transform,
            shipInventory,
            shipInventorySize - GetSpaceLeftInShipInv()
        );
        if (target.TryGetComponent<ShipPiece>(out ShipPiece sp))
            sp.AddToSeekers(this);
    }

    int GetSpaceLeftInShipInv()
    {
        int size = 0;
        foreach (ShipComponents item in shipInventory)
            size += Item.shipComponentSizes[item];
        return size;
    }
}
