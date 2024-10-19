using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBaseAudio : MonoBehaviour
{
    AudioSource audioSource;

    AudioClip deathSound;
    AudioClip spawnSound;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        deathSound = Resources.Load<AudioClip>("Audio/damage_taken/alien_nest_break");
        spawnSound = Resources.Load<AudioClip>("Audio/alien_spawn");
    }

    public void DeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }

    public void SpawnSound()
    {
        audioSource.PlayOneShot(spawnSound);
    }
}
