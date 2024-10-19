using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBaseAudio : MonoBehaviour
{
    AudioSource audioSource;

    AudioClip deathSound;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        deathSound = Resources.Load<AudioClip>("Audio/damage_taken/alien_nest_break");
    }

    public void DeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }
}
