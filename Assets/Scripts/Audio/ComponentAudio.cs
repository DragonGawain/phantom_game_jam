using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentAudio : MonoBehaviour
{
    AudioSource audioSource;

    AudioClip shipComponentPickupSound;
    AudioClip playerComponentPickupSound;
    AudioClip healthSound;

    AudioClip sonicSound;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        shipComponentPickupSound = Resources.Load<AudioClip>("Audio/component/ship_component_get");
        playerComponentPickupSound = Resources.Load<AudioClip>(
            "Audio/component/player_component_get"
        );
        healthSound = Resources.Load<AudioClip>("Audio/component/hp_pickup");
        sonicSound = Resources.Load<AudioClip>("Audio/component/sonic_ring");
    }

    public void PlayShipCompGet() => audioSource.PlayOneShot(shipComponentPickupSound);

    public void PlayPlayerCompGet() => audioSource.PlayOneShot(playerComponentPickupSound);

    public void HealthSound() => audioSource.PlayOneShot(healthSound);

    public void SonicSound() => audioSource.PlayOneShot(sonicSound);
}
