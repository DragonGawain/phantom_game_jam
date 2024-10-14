using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected int size;
    protected ComponentType componentType;

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
