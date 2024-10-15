using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
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

    protected virtual void Start()
    {
        OnStart();
    }

    protected abstract void OnStart();
}

// You will need all of these to be able to take off
// IMPORTANT:: All of these must have a tag
public enum ShipComponents
{
    [Description("nose_gear")]
    NOSE_GEAR,

    [Description("landing_gear")]
    LANDING_GEAR,

    [Description("oxygen_tank")]
    OXYGEN_TANK,

    [Description("fuel_tank")]
    FUEL_TANK, // will also have a fuel quantity that needs to be full -> this can be recoreded as just 'you need multiple fuel tanks'

    [Description("solid_boosters")]
    SOLID_BOOSTERS,

    [Description("engines")]
    ENGINES, // more engines = higher max speed

    [Description("rcs")]
    RCS, // more RCS = tighter turn radius (RCS stands for Rocket Control System)
    // SAS?
}

public enum PlayerComponents
{
    GUN,
}
