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

    // Start is called before the first frame update
    void Start()
    {
        alienObject = Resources.Load<GameObject>("AlienEnemy");
        GameObject al;
        for (int i = 0; i < initialSpawns; i++)
        {
            al = Instantiate(
                alienObject,
                new Vector3(
                    Random.Range(-spawnRadius, spawnRadius) + transform.position.x,
                    Random.Range(-spawnRadius, spawnRadius) + transform.position.y,
                    0
                ),
                Quaternion.identity
            );
            aliens.Add(al.GetComponent<Alien>());
        }
    }

    // Update is called once per frame
    void Update() { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Human"))
        {
            foreach (Alien al in aliens)
                al.SetTarget(other.transform);
        }
    }
}
