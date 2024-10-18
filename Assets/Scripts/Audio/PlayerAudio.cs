using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    Rigidbody2D rb;

    AudioSource walkingSource;
    AudioSource shootingSource;
    AudioSource tookDamageSource;

    AudioClip[] walkingSounds;
    AudioClip[] shootingSounds;
    AudioClip tookDamageSound;
    public bool isWalking = false;

    // Start is called before the first frame update
    void Awake()
    {
        AudioSource[] temp = GetComponents<AudioSource>();
        walkingSource = temp[0];
        shootingSource = temp[1];
        tookDamageSource = temp[2];

        walkingSounds = Resources.LoadAll<AudioClip>("Audio/walking_dirt");
        shootingSounds = Resources.LoadAll<AudioClip>("Audio/laser_fire/Player");
        tookDamageSound = Resources.Load<AudioClip>("Audio/damage_taken/player_damage_taken");

        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isWalking && rb.velocity.magnitude > 0.2f)
        {
            isWalking = true;
            StartCoroutine(PlayWalkingSound());
        }
    }

    public void ShootSoundBasic()
    {
        shootingSource.PlayOneShot(shootingSounds[Random.Range(0, 2)], 10);
    }

    public void ShootSoundAdvanced()
    {
        shootingSource.PlayOneShot(shootingSounds[Random.Range(2, 4)], 10);
    }

    IEnumerator PlayWalkingSound()
    {
        while (rb.velocity.magnitude > 0.1f)
        {
            walkingSource.PlayOneShot(walkingSounds[Random.Range(0, walkingSounds.Length)]);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.7f));
        }
        isWalking = false;
    }

    public void TookDamageSound()
    {
        tookDamageSource.PlayOneShot(tookDamageSound);
    }
}
