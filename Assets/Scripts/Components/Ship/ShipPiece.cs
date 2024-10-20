using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipPiece : Item
{
    [SerializeField]
    ShipComponents shipComponentType;

    List<Human> seekers = new();

    [SerializeField]
    bool isSpecificItem = false;

    readonly static Dictionary<ShipComponents, Sprite> componentSprites =
        new()
        {
            { ShipComponents.NOSE_GEAR, null },
            { ShipComponents.LANDING_GEAR, null },
            { ShipComponents.OXYGEN_TANK, null },
            { ShipComponents.FUEL_TANK, null },
            { ShipComponents.SOLID_BOOSTERS, null },
            { ShipComponents.ENGINES, null },
            { ShipComponents.RCS, null },
            { ShipComponents.WINGS, null },
        };

    protected override void OnAwake()
    {
        if (componentSprites[ShipComponents.NOSE_GEAR] == null)
            foreach (ShipComponents sc in Enum.GetValues(typeof(ShipComponents)))
                componentSprites[sc] = Resources.Load<Sprite>(
                    "Ship Parts/" + sc.GetEnumDescription()
                );

        if (!isSpecificItem)
            shipComponentType = (ShipComponents)
                Random.Range(0, Enum.GetNames(typeof(ShipComponents)).Length);
        GetComponent<SpriteRenderer>().sprite = componentSprites[shipComponentType];

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
        if (other.TryGetComponent<PlayerController>(out PlayerController pc) && !PlayerController.isEndingSequence)
        {
            if (pc.AddToShipInventory(shipComponentType))
            {
                AudioManager.PlayShipCompGet();

                if (transform == pc.GetQuest1Target())
                    pc.SetQuest1Target(null);
                if (transform == pc.GetQuest2Target())
                    pc.SetQuest2Target(null);

                if (pc.GetQuest1Target() == null && pc.GetQuest2Target() == null)
                    UIManager.SetOperationText("OpGoToShipText");

                Destroy(gameObject);
            }
        }
        else if (other.TryGetComponent<Human>(out Human human))
        {
            if (human.AddToShipInventory(shipComponentType))
            {
                PlayerController player = GameObject
                    .FindGameObjectWithTag("Player")
                    .GetComponent<PlayerController>();
                // A human can pick up a quest target, so we need to clear the quest target if it does
                if (transform == player.GetQuest1Target())
                    player.SetQuest1Target(null);
                if (transform == player.GetQuest2Target())
                    player.SetQuest2Target(null);

                if (player.GetQuest1Target() == null && player.GetQuest2Target() == null)
                    UIManager.SetOperationText("OpGoToShipText");

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
