using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAudio : MonoBehaviour
{
    AudioSource shipSource;
    AudioClip damageSound;

    void Awake()
    {
        shipSource = GetComponent<AudioSource>();

        damageSound = Resources.Load<AudioClip>(
            "Audio/damage_taken/Ship/rocket_damage_taken_short"
        );
    }

    public void TakeDamageShortSound() => shipSource.PlayOneShot(damageSound);
}
