using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Inputs inputs;
    Vector2 movementInput;

    Rigidbody2D rb;

    [SerializeField, Range(1f, 5f)]
    float maxMoveSpeed = 2f;

    [SerializeField, Range(0f, 1f)]
    float slowdownDrag = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputs = new Inputs();
        inputs.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    private void FixedUpdate()
    {
        movementInput = inputs.Player.Move.ReadValue<Vector2>().normalized;
        rb.velocity = Vector2.ClampMagnitude(rb.velocity + movementInput, maxMoveSpeed);
        if (rb.velocity.magnitude > 0)
            rb.velocity -= rb.velocity.normalized * slowdownDrag;
        if (rb.velocity.magnitude < 0)
            rb.velocity = Vector2.zero;
    }
}
