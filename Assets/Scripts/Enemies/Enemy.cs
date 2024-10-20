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

    public enum CombatState
    {
        ARRIVE,
        WANDER,
        FLEE,
        ATTACK,
        FLEE_TOWARDS,
        FORCE_ARRIVE
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

    [SerializeField, Range(15f, 100f)]
    protected float fleeDistanceBuffer = 25f;

    // HACK:: Serialized just so that we can see them in the inspector
    [SerializeField]
    protected MoveState moveState = MoveState.NONE;

    [SerializeField]
    protected CombatState combatState = CombatState.ARRIVE;

    public int holdTimer;
    protected Transform fDot;
    protected Transform rDot;
    protected Transform lDot;
    int hitLeft = 0;
    int hitRight = 0;

    bool farLeft;
    bool farRight;

    public Transform target;
    public int hp = 10;
    protected int maxHp = 10;
    protected int preferedTurnDir = 1;

    public Rigidbody2D rb; // HACK
    public Vector3 fleePoint;

    protected int id;

    protected int damage = 1;

    protected Animator animator;

    // Start is called before the first frame update
    protected override void OnAwake()
    {
        fDot = transform.Find("ForwardDot");
        rDot = transform.Find("RDot");
        lDot = transform.Find("LDot");

        // target = GameObject.FindGameObjectWithTag("Player").transform;

        preferedTurnDir = (int)Mathf.Sign(Random.Range(-1f, 1f));
        rb = GetComponentInParent<Rigidbody2D>();

        animator = GetComponentInParent<Animator>();
    }

    private void FixedUpdate()
    {
        animator.SetInteger("direction", 0);

        // left
        if (rb.velocity.x < -0.2f)
            animator.SetInteger("direction", 1);
        // right
        if (rb.velocity.x > 0.2f)
            animator.SetInteger("direction", 2);
        // up
        if (Mathf.Abs(rb.velocity.x) < 0.2f && rb.velocity.y > 0.2f)
            animator.SetInteger("direction", 3);
        // down
        if (Mathf.Abs(rb.velocity.x) < 0.2f && rb.velocity.y < -0.2f)
            animator.SetInteger("direction", 4);

        if (target != null)
        {
            VisionCast();
            ObstacleAvoidance();
            // we chose a movement pattern based on the current combat state
            switch (combatState)
            {
                case CombatState.ARRIVE:
                case CombatState.FORCE_ARRIVE:
                    Arrive();
                    break;
                case CombatState.WANDER:
                    Wander();
                    break;
                case CombatState.FLEE:
                    Flee();
                    break;
                case CombatState.ATTACK:
                    Attack();
                    break;
                case CombatState.FLEE_TOWARDS:
                    FleeTowards();
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

                // transform.rotation = Quaternion.FromToRotation(
                //     Vector3.up,
                //     (fDot.position - transform.position).normalized
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

        // if (Vector3.Distance(transform.position, target.position) < 0.05f)


        if (moveState == MoveState.MOV_TARGET)
            rb.velocity = maxMoveSpeed * (target.position - transform.position).normalized;
        else
            rb.velocity = 0.8f * maxMoveSpeed * (fDot.position - transform.position).normalized;
    }

    // Chase combat target to get within a certain range
    // possibly exclusive to the aliens?
    protected virtual void Wander() { }

    // Run away from a target -> try to run towards ship/base?
    // This is mainly a copy of ARRIVE, but with the direction flipped, and some gentle bias towards running towards their base
    protected virtual void Flee()
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
            // move away from target
            if (angle >= 180 - targetArc * 5) // by flipping this, we check for when we are facing directly away from the target
            {
                moveState = MoveState.MOV_TARGET;
                // transform.rotation = Quaternion.FromToRotation(
                //     (fDot.position - transform.position).normalized,
                //     (target.position - transform.position).normalized
                // );

                // I want the bias to work on a curve - the closer the angle is to 90, the stronger the bias will be.
                // This will mean that the AI will gently turn towards the base if it happens to be running directly away from it,
                // and the AI will smoothly aim itself at the fleePoint (base) as it's angle gets more precise.
                // I'm also going to add in a condition that only activates the fleePoint bias if the AI is a certain distance away from it
                // Debug.Log("dist: " + Vector3.Distance(fleePoint, transform.position));
                if (Vector3.Distance(fleePoint, transform.position) > fleeDistanceBuffer)
                {
                    // Debug.Log("bias");
                    // adding in the gentle base bias
                    float fleeAngle = Vector3.Angle(
                        (fDot.position - transform.position).normalized,
                        (fleePoint - transform.position).normalized
                    );
                    int dir =
                        Vector3.Angle(
                            (fDot.position - transform.position).Rotate(0.4f).normalized,
                            (fleePoint - transform.position).normalized
                        ) < fleeAngle
                            ? 1
                            : -1;
                    if (fleeAngle > 90)
                        fleeAngle -= 90;
                    fleeAngle /= 90;
                    transform.Rotate(0, 0, Mathf.Lerp(0.25f, 0.85f, fleeAngle) * turnSpeed * dir);
                }
            }

            // rotate towards target
            // the turn directions have been inverted compared to ARRIVE
            if (angle < 180 - targetArc * 5)
            {
                moveState = MoveState.ROT_TARGET;
                if (righterAngle < angle)
                    transform.Rotate(0, 0, -0.7f * turnSpeed); // right
                else
                    transform.Rotate(0, 0, 0.7f * turnSpeed); // left
            }
        }

        // if (Vector3.Distance(transform.position, target.position) < 0.05f)


        if (moveState == MoveState.MOV_TARGET)
            rb.velocity = 1.25f * maxMoveSpeed * (transform.position - target.position).normalized;
        else
            rb.velocity = maxMoveSpeed * (fDot.position - transform.position).normalized;
    }

    // maintain distance, and also fight back
    // possibly exclusive to humans (more precisly, exclusive to any enemy type that has a ranged attack)
    protected virtual void Attack() { }

    protected virtual void FleeTowards()
    {
        Arrive();
    }

    protected virtual void TakeDamage(int amt, bool isBullet = false)
    {
        hp -= amt;
        if (hp <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public int GetDamage() => damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("EvilBullet"))
        {
            if (other.GetComponent<Bullet>().GetShooterId() == id)
                return;
            TakeDamage(other.GetComponent<Bullet>().GetBulletDamage(), true);
            if (gameObject.CompareTag("Alien"))
                OnOnTrigger(other.GetComponent<Bullet>().GetShooter(), true);
            Destroy(other.gameObject);
        }
        // else if (other.gameObject.CompareTag("Alien"))
        // {
        //     TakeDamage(other.GetComponent<Alien>().GetDamage());
        //     other.GetComponent<Alien>().PlayAttackSound();
        // }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!PlayerController.isEndingSequence && other.gameObject.CompareTag("Alien"))
        {
            TakeDamage(other.gameObject.GetComponentInChildren<Enemy>().GetDamage());
            other.gameObject.GetComponentInChildren<Alien>().PlayAttackSound();
        }
    }

    protected virtual void OnOnTrigger(Transform other, bool isBullet) { }

    protected Transform DetermineFleePoint(
        Transform point,
        string tag = "",
        GameObject exclude = null
    )
    {
        if (tag == "")
            return point;
        else
        {
            GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(tag);
            Transform pos = point;

            float dist = int.MaxValue;
            foreach (GameObject pt in potentialTargets)
            {
                if (pt == exclude)
                    continue;
                if (Vector3.Distance(pt.transform.position, transform.position) < dist)
                {
                    dist = Vector3.Distance(pt.transform.position, transform.position);
                    pos = pt.transform;
                }
            }
            return pos;
        }
    }
}
