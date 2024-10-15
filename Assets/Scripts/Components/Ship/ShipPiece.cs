using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipPiece : Item
{
    [SerializeField]
    ShipComponents shipComponentType;

    protected override void OnStart()
    {
        shipComponentType = (ShipComponents)
            Random.Range(0, Enum.GetNames(typeof(ShipComponents)).Length);

        gameObject.tag = shipComponentType.GetEnumDescription();
    }

    public ShipComponents GetShipComponentType() => shipComponentType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            // TODO: set up player inventory to handle ship pieces
            // pc.AddToInventory(this);
            // we can't destroy the gameobject cause then it would also be deleted from the inventory,
            // so for now I'll just set it no longer be active so other things can't pick it up
            // gameObject.SetActive(false);
            if (pc.AddToShipInventory(shipComponentType))
            {
                gameObject.SetActive(false);
            }
        }
        else if (other.TryGetComponent<Human>(out Human human))
        {
            if (human.AddToShipInventory(shipComponentType))
            {
                gameObject.SetActive(false);
                human.CollectedShipPiece();
            }
        }
        // if it's not a human enemy or the player, do nothing.
    }
}
