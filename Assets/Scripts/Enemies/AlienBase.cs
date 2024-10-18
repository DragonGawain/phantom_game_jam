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

    [SerializeField, Range(5, 180)]
    int tempAggroTimerInSeconds = 30;

    int spawnTimer = 0;
    bool isSpawning = true;
    int hp = 60;

    public List<Transform> targets = new();

    Coroutine tempAggro;

    AlienBaseAudio alienBaseAudio;

    // Start is called before the first frame update
    void Start()
    {
        alienBaseAudio = GetComponent<AlienBaseAudio>();
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

        if (targets.Count > 0)
            al.SetTarget(targets[Random.Range(0, targets.Count)]);
        else
            al.StopAttack();
    }

    public void KillAlien(Alien alien)
    {
        aliens.Remove(alien);

        if (aliens.Count <= Mathf.FloorToInt(maxAliens / 2f))
            isSpawning = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("EvilBullet"))
        {
            hp -= other.GetComponent<Bullet>().GetBulletDamage();
            if (hp <= 0)
            {
                alienBaseAudio.DeathSound();
                foreach (Alien al in aliens)
                    Destroy(al.gameObject);
                Destroy(gameObject);
            }
            ExternalTriggerOfTempAggro(other.GetComponent<Bullet>().GetShooter());
            Destroy(other.gameObject);
        }
    }

    public void ExternalTriggerOfTempAggro(Transform target)
    {
        if (tempAggro != null)
            StopCoroutine(tempAggro);
        tempAggro = StartCoroutine(IncurTemporaryAggro(target));
    }

    IEnumerator IncurTemporaryAggro(Transform target)
    {
        IncreaseAggro(target);
        yield return new WaitForSeconds(tempAggroTimerInSeconds);
        ReduceAggro(target);
        tempAggro = null;
    }

    public void IncreaseAggro(Transform target)
    {
        if (PlayerController.isEndingSequence)
            return;
        if (!targets.Contains(target))
            targets.Add(target);
        foreach (Alien al in aliens)
            al.SetTarget(targets[Random.Range(0, targets.Count)]);
    }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player") || other.CompareTag("Human"))
    //     {
    //         isAttacking--;
    //         if (isAttacking <= 0)
    //             StopAttack();
    //     }
    // }

    public void ReduceAggro(Transform target)
    {
        if (PlayerController.isEndingSequence)
            return;
        if (targets.Contains(target))
            targets.Remove(target);

        if (targets.Count == 0)
            StopAttack();
        else
            foreach (Alien al in aliens)
                al.SetTarget(targets[Random.Range(0, targets.Count)]);
    }

    public void StopAttack()
    {
        foreach (Alien al in aliens)
            al.StopAttack();
    }

    public void TriggerEndSequence()
    {
        targets.Clear();
    }
}
