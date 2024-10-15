using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will be for all non-functional ship pieces
public class ShipPiece : Item
{
    [SerializeField]
    ShipPieces shipPieceType;

    protected override void OnStart()
    {
        size = 4;
        componentType = ComponentType.SHIP;

        gameObject.tag = shipPieceType switch
        {
            ShipPieces.NOSE_GEAR => "nose_gear",
            ShipPieces.LANDING_GEAR => "landing_gear",
            ShipPieces.OXYGEN_TANK => "oxygen_tank",
            ShipPieces.FUEL_TANK => "fuel_tank",
            ShipPieces.SOLID_BOOSTERS => "solid_boosters",
            ShipPieces.ENGINES => "engines",
            ShipPieces.RCS => "rcs",
            _ => "Untagged"
        };
    }

    public ShipPieces GetShipPiece() => shipPieceType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            // TODO: set up player inventory to handle ship pieces
            // pc.AddToInventory(this);
            // we can't destroy the gameobject cause then it would also be deleted from the inventory,
            // so for now I'll just set it no longer be active so other things can't pick it up
            // gameObject.SetActive(false);
        }
        else if (other.TryGetComponent<Human>(out Human human))
        {
            human.AddToShipInventory(this);
            gameObject.SetActive(false);
            human.CollectedShipPiece();
        }
        // if it's not a human enemy or the player, do nothing.
    }
}

// You will need all of these to be able to take off
// IMPORTANT:: All of these must have a tag
public enum ShipPieces
{
    NOSE_GEAR,
    LANDING_GEAR,
    OXYGEN_TANK,
    FUEL_TANK, // will also have a fuel quantity that needs to be full -> this can be recoreded as just 'you need multiple fuel tanks'
    SOLID_BOOSTERS,
    ENGINES, // more engines = higher max speed
    RCS, // more RCS = tighter turn radius (RCS stands for Rocket Control System)
    // SAS?
}
