using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : Enemy
{
    AlienBase alienBase;

    static readonly float baseWanderDistance = 4f;
    static readonly float wanderDistanceModifier = 1.5f;
    static readonly int baseWanderTimer = 150;
    static readonly int wanderTimerModifier = 50;

    float wanderDist;
    int wanderTimerReset;
    int wanderTimer;

    Vector3 wanderTarget;

    AlienAudio alienAudio;

    private void Start()
    {
        wanderDist =
            baseWanderDistance + Random.Range(-wanderDistanceModifier, wanderDistanceModifier);
        wanderTimerReset =
            baseWanderTimer + Random.Range(-wanderTimerModifier, wanderTimerModifier);
        wanderTimer = 0;

        fleePoint = alienBase.transform.position;
        if (target == null)
            target = alienBase.transform;

        id = -1;
        damage = 2;

        alienAudio = GetComponent<AlienAudio>();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        combatState = CombatState.ARRIVE;
    }

    public void StopAttack()
    {
        combatState = CombatState.WANDER;
        target = alienBase.transform;
    }

    public void SetBase(AlienBase ab) => alienBase = ab;

    // wander
    protected override void Wander()
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
        }

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

    protected override void TakeDamage(int amt, bool isBullet = false)
    {
        alienAudio.TookDamageSound();
        base.TakeDamage(amt, isBullet);
        // OnOnTrigger(alienBase.transform, isBullet);
    }

    private void OnDestroy()
    {
        alienBase.KillAlien(this);
    }

    protected override void OnOnTrigger(Transform other, bool isBullet)
    {
        alienBase.ExternalTriggerOfTempAggro(other);
    }

    public void PlayAttackSound()
    {
        alienAudio.AttackingSound();
    }
}
