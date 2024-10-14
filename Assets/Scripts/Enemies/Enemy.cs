using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Movement
{
    enum MoveState
    {
        NONE,
        HOLD,
        ROT_TARGET,
        ROT_OBSTACLE_L,
        ROT_OBSTACLE_R,
        MOV_TARGET
    }

    [SerializeField, Range(1f, 10f)]
    float visionDistance = 3f;

    [SerializeField, Range(15f, 75f)]
    float visionAngle = 30f;

    [SerializeField, Range(1, 5)]
    int precision = 2;

    [SerializeField, Range(0.5f, 15f)]
    float targetArc = 5;

    [SerializeField, Range(0.5f, 5f)]
    float turnSpeed = 1;

    [SerializeField]
    MoveState moveState = MoveState.NONE;

    public int holdTimer;
    Transform fDot;
    Transform rDot;
    Transform lDot;
    int hitLeft = 0;
    int hitRight = 0;

    bool farLeft;
    bool farRight;

    public Transform target;
    public bool hasTarget = false;
    public int preferedTurnDir = 1;

    Rigidbody2D rb;

    int hp = 10;

    // Start is called before the first frame update
    void Awake()
    {
        fDot = transform.Find("ForwardDot");
        rDot = transform.Find("RDot");
        lDot = transform.Find("LDot");

        target = GameObject.FindGameObjectWithTag("Player").transform;

        preferedTurnDir = (int)Mathf.Sign(Random.Range(-1f, 1f));
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() { }

    private void FixedUpdate()
    {
        if (hasTarget)
        {
            VisionCast();
            Move();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

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

    void Move()
    {
        // states: rotating towards target, rotating to avoid obstacle, moving towards target.


        // rotating to avoid obstacle
        // if (!farRight || !farLeft)
        //     moveState = MoveState.NONE;
        // if (hitLeft > 0 || hitRight > 0)
        //     moveState = MoveState.ROT_OBSTACLE;

        // set rotation direction
        if (moveState != MoveState.ROT_OBSTACLE_L && moveState != MoveState.ROT_OBSTACLE_R)
        {
            if (hitLeft == hitRight && hitLeft > 0 && farLeft == farRight)
            {
                // turn to a side
                // transform.Rotate(0, 0, 1f * preferedTurnDir * turnSpeed);
                if (preferedTurnDir < 0)
                    moveState = MoveState.ROT_OBSTACLE_R;
                else
                    moveState = MoveState.ROT_OBSTACLE_L;
                // moveState = MoveState.ROT_OBSTACLE;
            }
            // turn right
            else if (
                (hitLeft > hitRight && hitRight > 0)
                || (hitLeft > 0 && hitRight == 0)
                || (farLeft && hitLeft > 0 && !farRight)
            )
            {
                // transform.Rotate(0, 0, -1f * turnSpeed);
                moveState = MoveState.ROT_OBSTACLE_R;
            }
            // turn left
            else if (
                (hitRight > hitLeft && hitLeft > 0)
                || (hitRight > 0 && hitLeft == 0)
                || (farRight && hitRight > 0 && !farLeft)
            )
            {
                // transform.Rotate(0, 0, 1f * turnSpeed);
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
                // transform.rotation = Quaternion.SetFromToRotation(
                //     (target.position - transform.position).normalized,
                //     new Vector3(0, 0, 0)
                // );

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

        if (Vector3.Distance(transform.position, target.position) < 2)
            hasTarget = false;

        if (moveState == MoveState.MOV_TARGET)
            rb.velocity = maxMoveSpeed * (target.position - transform.position).normalized;
        else
            rb.velocity = 0.8f * maxMoveSpeed * (fDot.position - transform.position).normalized;
    }

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
