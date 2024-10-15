using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected int size;
    protected ComponentType componentType;

    public static readonly Dictionary<ShipComponents, int> shipComponentSizes =
        new()
        {
            { ShipComponents.NOSE_GEAR, 3 },
            { ShipComponents.LANDING_GEAR, 2 },
            { ShipComponents.OXYGEN_TANK, 4 },
            { ShipComponents.FUEL_TANK, 5 },
            { ShipComponents.SOLID_BOOSTERS, 3 },
            { ShipComponents.ENGINES, 3 },
            { ShipComponents.RCS, 1 },
        };

    public static readonly Dictionary<PlayerComponents, int> playerComponentSizes =
        new() { { PlayerComponents.GUN, 2 }, };

    public int GetSize() => size;

    public ComponentType GetComponentType() => componentType;

    protected virtual void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        size = 2;
        componentType = ComponentType.PLAYER;
    }
}

public enum ComponentType
{
    SHIP,
    PLAYER
}

// You will need all of these to be able to take off
// IMPORTANT:: All of these must have a tag
public enum ShipComponents
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

public enum PlayerComponents
{
    GUN,
}
