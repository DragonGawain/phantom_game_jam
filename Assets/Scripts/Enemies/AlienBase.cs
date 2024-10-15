using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBase : MonoBehaviour
{
    GameObject alienObject;
    List<Alien> aliens = new();

    [SerializeField, Range(2f, 15f)]
    float spawnRadius = 5;

    [SerializeField, Range(0, 10)]
    int initialSpawns = 3;

    [SerializeField, Range(5, 20)]
    int maxAliens = 10;

    [SerializeField, Range(50, 3000)]
    int spawnTimerReset = 150;

    int spawnTimer = 0;
    bool isSpawning = true;
    Transform target;
    int isAttacking = 0;

    // Start is called before the first frame update
    void Start()
    {
        alienObject = Resources.Load<GameObject>("AlienEnemy");

        for (int i = 0; i < initialSpawns; i++)
        {
            SpawnAlien();
        }
    }

    private void FixedUpdate()
    {
        if (isSpawning)
        {
            spawnTimer++;
            if (spawnTimer > spawnTimerReset)
            {
                spawnTimer = 0;
                SpawnAlien();
                if (aliens.Count == maxAliens)
                    isSpawning = false;
            }
        }
    }

    void SpawnAlien()
    {
        GameObject alo = Instantiate(
            alienObject,
            new Vector3(
                Random.Range(-spawnRadius, spawnRadius) + transform.position.x,
                Random.Range(-spawnRadius, spawnRadius) + transform.position.y,
                0
            ),
            Quaternion.identity
        );
        Alien al = alo.GetComponent<Alien>();
        aliens.Add(al);
        al.SetBase(this);

        if (isAttacking > 0)
            al.SetTarget(target);
    }

    public void KillAlien(Alien alien)
    {
        aliens.Remove(alien);

        if (aliens.Count <= Mathf.FloorToInt(maxAliens / 2f))
            isSpawning = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Human"))
        {
            foreach (Alien al in aliens)
                al.SetTarget(other.transform);
            target = other.transform;
            isAttacking++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Human"))
        {
            isAttacking--;
        }
    }

    public void StopAttack()
    {
        foreach (Alien al in aliens)
            al.StopAttack();
    }
}
