using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : Enemy
{
    AlienBase alienBase;

    // // Start is called before the first frame update
    // void Start() { }

    // // Update is called once per frame
    // void Update() { }

    public void SetTarget(Transform target)
    {
        this.target = target;
        hasTarget = true;
    }

    public void StopAttack() => hasTarget = false;

    public void SetBase(AlienBase ab) => alienBase = ab;

    private void OnDestroy()
    {
        alienBase.KillAlien(this);
    }

    // TODO:: override ARRIVE and replace it with WANDER
}
