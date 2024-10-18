using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    float bulletSpeed = 2.5f;

    int dmg = 2;

    int bulletLifeTimer = 500; // 50 FUs persecond -> this bullet will last for 10 seconds

    int shooterId;
    Transform shooter;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        bulletLifeTimer--;
        if (bulletLifeTimer <= 0)
            Destroy(this.gameObject);
    }

    public void SetShooterId(int id) => shooterId = id;

    public int GetShooterId() => shooterId;

    public void SetShooter(Transform shooter) => this.shooter = shooter;

    public Transform GetShooter() => shooter;

    public void Launch(Vector2 vel)
    {
        rb.velocity = vel * bulletSpeed;
    }

    public int GetBulletDamage() => dmg;
}
