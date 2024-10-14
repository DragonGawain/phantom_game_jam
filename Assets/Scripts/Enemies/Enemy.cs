using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField, Range(1f, 10f)]
    float visionDistance = 3f;

    [SerializeField, Range(15f, 75f)]
    float visionAngle = 30f;

    [SerializeField, Range(1, 5)]
    int precision = 2;

    [SerializeField, Range(1f, 5f)]
    int maxSpeed = 2;

    [SerializeField, Range(0.001f, 1f)]
    int moveDrag = 2;
    Transform fDot;
    int hitLeft = 0;
    int hitRight = 0;

    public Vector3 targetPos;
    public bool hasTarget = false;
    public int preferedTurnDir = 1;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        fDot = transform.Find("ForwardDot");

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
    }

    void VisionCast()
    {
        // RaycastHit2D
        bool hit;
        Vector3 forward;
        float deltaAngle;
        // left
        for (int i = 0; i <= precision; i++)
        {
            forward = (fDot.position - transform.position).normalized;
            deltaAngle = Mathf.Lerp(visionAngle, -visionAngle, (float)i / (precision * 2 + 1));
            forward = forward.Rotate(deltaAngle);
            Debug.DrawRay(transform.position, forward * visionDistance, Color.green);
            hit = Physics2D.Raycast(transform.position, forward, visionDistance, 6);
            if (hit)
                hitLeft++;
        }

        // right
        for (int i = precision + 1; i <= 2 * precision + 1; i++)
        {
            forward = (fDot.position - transform.position).normalized;
            deltaAngle = Mathf.Lerp(visionAngle, -visionAngle, (float)i / (precision * 2 + 1));
            forward = forward.Rotate(deltaAngle);
            Debug.DrawRay(transform.position, forward * visionDistance, Color.red);
            hit = Physics2D.Raycast(transform.position, forward, visionDistance, 6);
            if (hit)
                hitRight++;
        }
    }

    void Move()
    {
        if (hitLeft == hitRight && hitLeft > 0)
        {
            // turn to a side
            transform.Rotate(0, 0, 1f * preferedTurnDir);
        }
        else if (hitLeft > hitRight && hitLeft > 1)
        {
            // turn right
            transform.Rotate(0, 0, 1f);
        }
        else if (hitRight > hitLeft && hitRight > 1)
        {
            // turn left
            transform.Rotate(0, 0, -1f);
        }

        rb.velocity = (fDot.position - transform.position) * maxSpeed;
    }
}
