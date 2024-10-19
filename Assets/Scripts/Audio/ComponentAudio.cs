using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentAudio : MonoBehaviour
{
    AudioSource audioSource;

    AudioClip shipComponentPickupSound;
    AudioClip playerComponentPickupSound;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        shipComponentPickupSound = Resources.Load<AudioClip>("Audio/component/ship_component_get");
        playerComponentPickupSound = Resources.Load<AudioClip>(
            "Audio/component/player_component_get"
        );
    }

    public void PlayShipCompGet()
    {
        audioSource.PlayOneShot(shipComponentPickupSound);
    }

    public void PlayPlayerCompGet()
    {
        audioSource.PlayOneShot(playerComponentPickupSound);
    }
}
