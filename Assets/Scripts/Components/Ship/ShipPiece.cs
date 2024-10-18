using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipPiece : Item
{
    [SerializeField]
    ShipComponents shipComponentType;

    List<Human> seekers = new();

    [SerializeField]
    bool isSpecificItem = false;

    protected override void OnAwake()
    {
        if (!isSpecificItem)
            shipComponentType = (ShipComponents)
                Random.Range(0, Enum.GetNames(typeof(ShipComponents)).Length);

        gameObject.tag = shipComponentType.GetEnumDescription();
        // set the sprite of the component (or UI popup if there are no specific sprites)
    }

    public void SetSpecificType(ShipComponents type)
    {
        shipComponentType = type;
        isSpecificItem = true;
        OnAwake();
    }

    public ShipComponents GetShipComponentType() => shipComponentType;

    public void AddToSeekers(Human human) => seekers.Add(human);

    // This is some super fancy trickery I'm doing to get this to work and be very concise.
    // Don't worry if you don't understand it, and feel free to ask me to explain it! - Craig.
    public bool RemoveFromSeekers(Human human) => seekers.Contains(human) && seekers.Remove(human);

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
                human.CollectedShipPiece();
                // bool check = seekers.Contains(human);
                // if (check)
                //     seekers.Remove(human);

                _ = seekers.Contains(human) && seekers.Remove(human);

                // if (!seekers.Contains(human))
                //     seekers.Add(human);

                Destroy(gameObject);
            }
        }
        // if it's not a human enemy or the player, do nothing.
    }

    private void OnDestroy()
    {
        foreach (Human seeker in seekers)
            if (seeker != null && seeker.GetShip() != null) // This if statement prevents a crash when the scene is unloaded
                seeker.CollectedShipPiece();
    }
}
