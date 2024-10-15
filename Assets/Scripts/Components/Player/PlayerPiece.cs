using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : Item
{
    [SerializeField]
    PlayerComponents playerComponentType;

    protected override void OnAwake() { }

    public PlayerComponents GetPlayerComponentType() => playerComponentType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            if (pc.AddToInventory(playerComponentType))
            {
                Destroy(gameObject);
            }
        }
        else if (other.TryGetComponent<Human>(out Human human))
        {
            // TODO: set up Human inventory to handle personal equipement
            // human.AddToInventory(this);
            // we can't destroy the gameobject cause then it would also be deleted from the inventory,
            // so for now I'll just set it no longer be active so other things can't pick it up
            // gameObject.SetActive(false);
        }
        // if it's not a human enemy or the player, do nothing.
    }
}
