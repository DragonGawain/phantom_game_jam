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
            if (pc.AddToShipInventory(shipComponentType))
            {
                Destroy(gameObject);
            }
        }
        else if (other.TryGetComponent<Human>(out Human human))
        {
            if (human.AddToShipInventory(shipComponentType))
            {
                Destroy(gameObject);
                human.CollectedShipPiece();
            }
        }
        // if it's not a human enemy or the player, do nothing.
    }
}
