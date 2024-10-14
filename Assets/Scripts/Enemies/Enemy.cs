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
    int precision = 1;
    Transform fDot;

    // Start is called before the first frame update
    void Awake()
    {
        fDot = transform.Find("ForwardDot");
    }

    // Update is called once per frame
    void Update() { }

    private void FixedUpdate()
    {
        VisionCast();
    }

    void VisionCast()
    {
        // RaycastHit2D
        bool hit;
        Vector3 forward;
        float deltaAngle;
        for (int i = 0; i <= 2 * precision + 1; i++)
        {
            // hit = Physics2D.Raycast(transform.position)
            forward = (fDot.position - transform.position).normalized;
            // left to right
            deltaAngle = Mathf.Lerp(visionAngle, -visionAngle, (float)i / (precision * 2 + 1));
            forward = forward.Rotate(deltaAngle);
            Debug.DrawRay(transform.position, forward * visionDistance, Color.green);
            // Debug.Log(forward);
        }
    }
}
