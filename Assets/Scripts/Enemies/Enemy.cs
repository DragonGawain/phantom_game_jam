using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Enemy : Movement
{
    protected enum MoveState
    {
        NONE,
        HOLD,
        ROT_TARGET,
        ROT_OBSTACLE_L,
        ROT_OBSTACLE_R,
        MOV_TARGET
    }

    protected enum CombatState
    {
        NONE,
        CHASE,
        FLEE,
        ATTACK
    }

    [SerializeField, Range(1f, 10f)]
    float visionDistance = 3f;

    [SerializeField, Range(15f, 75f)]
    float visionAngle = 30f;

    [SerializeField, Range(1, 5)]
    int precision = 2;

    [SerializeField, Range(0.5f, 15f)]
    protected float targetArc = 5;

    [SerializeField, Range(0.5f, 5f)]
    protected float turnSpeed = 1;

    // HACK:: Serialized just so that we can see them in the inspector
    [SerializeField]
    protected MoveState moveState = MoveState.NONE;

    [SerializeField]
    protected CombatState combatState = CombatState.NONE;

    public int holdTimer;
    protected Transform fDot;
    Transform rDot;
    Transform lDot;
    int hitLeft = 0;
    int hitRight = 0;

    bool farLeft;
    bool farRight;

    public Transform target;
    protected bool hasTarget = false;
    protected int hp = 10;
    int preferedTurnDir = 1;

    protected Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        fDot = transform.Find("ForwardDot");
        rDot = transform.Find("RDot");
        lDot = transform.Find("LDot");

        // target = GameObject.FindGameObjectWithTag("Player").transform;

        preferedTurnDir = (int)Mathf.Sign(Random.Range(-1f, 1f));
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (hasTarget)
        {
            VisionCast();
            ObstacleAvoidance();
            // we chose a movement pattern based on the current combat state
            switch (combatState)
            {
                case CombatState.NONE:
                    Arrive();
                    break;
                case CombatState.CHASE:
                    Chase();
                    break;
                case CombatState.FLEE:
                    Flee();
                    break;
                case CombatState.ATTACK:
                    Attack();
                    break;
            }
        }
        else
            rb.velocity = Vector2.zero;

        DoFixedUpdate();
    }

    protected virtual void DoFixedUpdate() { }

    // Cast the vision rays used in ObstacleAvoidance
    void VisionCast()
    {
        // RaycastHit2D
        bool hit;
        Vector3 forward;
        float deltaAngle;
        hitLeft = 0;
        hitRight = 0;
        // left
        for (int i = 0; i <= precision; i++)
        {
            forward = (fDot.position - transform.position).normalized;
            deltaAngle = Mathf.Lerp(visionAngle, -visionAngle, (float)i / (precision * 2 + 1));
            forward = forward.Rotate(deltaAngle);
            Debug.DrawRay(transform.position, forward * visionDistance, Color.green);
            hit = Physics2D.Raycast(
                transform.position,
                forward,
                visionDistance,
                LayerMask.GetMask("Obstacle")
            );
            if (hit)
                hitLeft++;
        }

        Debug.DrawRay(
            lDot.position,
            new Vector3(1, 1, 0).Rotate(this.transform.localEulerAngles.z - 30).normalized
                * visionDistance,
            Color.green
        );
        hit = Physics2D.Raycast(
            lDot.position,
            new Vector3(1, 1, 0).Rotate(this.transform.localEulerAngles.z - 30).normalized,
            visionDistance,
            LayerMask.GetMask("Obstacle")
        );
        farLeft = false;
        if (hit)
            farLeft = true;

        // right
        for (int i = precision + 1; i <= 2 * precision + 1; i++)
        {
            forward = (fDot.position - transform.position).normalized;
            deltaAngle = Mathf.Lerp(visionAngle, -visionAngle, (float)i / (precision * 2 + 1));
            forward = forward.Rotate(deltaAngle);
            Debug.DrawRay(transform.position, forward * visionDistance, Color.red);
            hit = Physics2D.Raycast(
                transform.position,
                forward,
                visionDistance,
                LayerMask.GetMask("Obstacle")
            );
            if (hit)
                hitRight++;
        }

        Debug.DrawRay(
            rDot.position,
            new Vector3(1, -1, 0).Rotate(this.transform.localEulerAngles.z + 30).normalized
                * visionDistance,
            Color.red
        );
        hit = Physics2D.Raycast(
            rDot.position,
            new Vector3(1, -1, 0).Rotate(this.transform.localEulerAngles.z + 30).normalized,
            visionDistance,
            LayerMask.GetMask("Obstacle")
        );
        farRight = false;
        if (hit)
            farRight = true;
    }

    // none, chase, flee, attack
    // Parse vision rays and turn to avoid obstacles
    void ObstacleAvoidance()
    {
        // set rotation direction
        if (moveState != MoveState.ROT_OBSTACLE_L && moveState != MoveState.ROT_OBSTACLE_R)
        {
            if (hitLeft == hitRight && hitLeft > 0 && farLeft == farRight)
            {
                // turn to a side
                if (preferedTurnDir < 0)
                    moveState = MoveState.ROT_OBSTACLE_R;
                else
                    moveState = MoveState.ROT_OBSTACLE_L;
            }
            // turn right
            else if (
                (hitLeft > hitRight && hitRight > 0)
                || (hitLeft > 0 && hitRight == 0)
                || (farLeft && hitLeft > 0 && !farRight)
            )
            {
                moveState = MoveState.ROT_OBSTACLE_R;
            }
            // turn left
            else if (
                (hitRight > hitLeft && hitLeft > 0)
                || (hitRight > 0 && hitLeft == 0)
                || (farRight && hitRight > 0 && !farLeft)
            )
            {
                moveState = MoveState.ROT_OBSTACLE_L;
            }
        }

        // keep turning left
        if (moveState == MoveState.ROT_OBSTACLE_L)
        {
            if (!farRight)
            {
                moveState = MoveState.HOLD;
                holdTimer = 25;
            }
            transform.Rotate(0, 0, 1f * turnSpeed);
        }

        // keep turning right
        if (moveState == MoveState.ROT_OBSTACLE_R)
        {
            if (!farLeft)
            {
                moveState = MoveState.HOLD;
                holdTimer = 25;
            }
            transform.Rotate(0, 0, -1f * turnSpeed);
        }

        if (holdTimer >= 0)
            holdTimer--;
    }

    // Go directly to the target
    protected virtual void Arrive()
    {
        if (
            moveState != MoveState.ROT_OBSTACLE_L
            && moveState != MoveState.ROT_OBSTACLE_R
            && holdTimer <= 0
        )
        {
            float angle = Vector3.Angle(
                (fDot.position - transform.position).normalized,
                (target.position - transform.position).normalized
            );
            float righterAngle = Vector3.Angle(
                (fDot.position - transform.position).Rotate(0.4f).normalized,
                (target.position - transform.position).normalized
            );
            // move towards target
            if (angle <= targetArc)
            {
                moveState = MoveState.MOV_TARGET;
                // transform.rotation = Quaternion.FromToRotation(
                //     (fDot.position - transform.position).normalized,
                //     (target.position - transform.position).normalized
                // );
            }

            // rotate towards target
            if (angle > targetArc)
            {
                moveState = MoveState.ROT_TARGET;
                if (righterAngle < angle)
                    transform.Rotate(0, 0, 0.7f * turnSpeed);
                else
                    transform.Rotate(0, 0, -0.7f * turnSpeed);
            }
        }

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
            hasTarget = false;

        if (moveState == MoveState.MOV_TARGET)
            rb.velocity = maxMoveSpeed * (target.position - transform.position).normalized;
        else
            rb.velocity = 0.8f * maxMoveSpeed * (fDot.position - transform.position).normalized;
    }

    // Chase combat target to get within a certain range
    protected virtual void Chase() { }

    // Run away from a target -> try to run towards ship/base?
    protected virtual void Flee() { }

    // maintain distance, and also fight back
    protected virtual void Attack() { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            hp -= other.GetComponent<Bullet>().GetBulletDamage();
            if (hp <= 0)
                Destroy(this.gameObject);

            Destroy(other.gameObject);
        }
    }
}
