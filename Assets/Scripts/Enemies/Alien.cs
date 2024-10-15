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

    static readonly float baseWanderDistance = 4f;
    static readonly float wanderDistanceModifier = 1.5f;
    static readonly int baseWanderTimer = 150;
    static readonly int wanderTimerModifier = 50;

    float wanderDist;
    int wanderTimerReset;
    int wanderTimer;

    Vector3 wanderTarget;

    private void Start()
    {
        wanderDist =
            baseWanderDistance + Random.Range(-wanderDistanceModifier, wanderDistanceModifier);
        wanderTimerReset =
            baseWanderTimer + Random.Range(-wanderTimerModifier, wanderTimerModifier);
        wanderTimer = 0;
        hasTarget = true;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        hasTarget = true;
        combatState = CombatState.CHASE;
    }

    public void StopAttack() => combatState = CombatState.NONE;

    public void SetBase(AlienBase ab) => alienBase = ab;

    // wander
    protected override void Arrive()
    {
        // deal with the timer
        wanderTimer--;
        if (
            Vector3.Distance(transform.position, wanderTarget) <= 0.2f
            || Vector3.Distance(transform.position, alienBase.transform.position) > wanderDist * 2
        )
            wanderTimer = 0;

        if (wanderTimer <= 0)
        {
            wanderTimer = wanderTimerReset;
            // select a random point in range
            wanderTarget = new Vector3(
                alienBase.transform.position.x + Random.Range(-wanderDist, wanderDist),
                alienBase.transform.position.y + Random.Range(-wanderDist, wanderDist),
                0
            );
            hasTarget = true;
        }

        // base.Arrive(), but slightly modified
        if (
            moveState != MoveState.ROT_OBSTACLE_L
            && moveState != MoveState.ROT_OBSTACLE_R
            && holdTimer <= 0
        )
        {
            float angle = Vector3.Angle(
                (fDot.position - transform.position).normalized,
                (wanderTarget - transform.position).normalized
            );
            float righterAngle = Vector3.Angle(
                (fDot.position - transform.position).Rotate(0.4f).normalized,
                (wanderTarget - transform.position).normalized
            );
            // move towards target
            if (angle <= targetArc)
            {
                moveState = MoveState.MOV_TARGET;
                // transform.rotation = Quaternion.FromToRotation(
                //     (fDot.position - transform.position).normalized,
                //     (wanderTarget - transform.position).normalized
                // );
            }

            // rotate towards target
            if (angle > targetArc)
            {
                moveState = MoveState.ROT_TARGET;
                if (righterAngle < angle)
                    transform.Rotate(0, 0, 0.7f * turnSpeed * 2);
                else
                    transform.Rotate(0, 0, -0.7f * turnSpeed * 2);
            }
        }

        if (moveState == MoveState.MOV_TARGET)
            rb.velocity = maxMoveSpeed * (wanderTarget - transform.position).normalized;
        else
            rb.velocity = 0.8f * maxMoveSpeed * (fDot.position - transform.position).normalized;
    }

    protected override void Chase()
    {
        base.Arrive();
    }

    private void OnDestroy()
    {
        alienBase.KillAlien(this);
    }

    // TODO:: override ARRIVE and replace it with WANDER
}
