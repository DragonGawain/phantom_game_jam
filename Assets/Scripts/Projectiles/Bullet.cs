using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    float bulletSpeed = 2.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 vel)
    {
        rb.velocity = vel * bulletSpeed;
    }
}
