using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudio : MonoBehaviour
{
    AudioSource inventorySource;
    AudioClip openSound;
    AudioClip closeSound;

    void Awake()
    {
        inventorySource = GetComponent<AudioSource>();

        openSound = Resources.Load<AudioClip>("Audio/ship_inventory/rocket_open");
        closeSound = Resources.Load<AudioClip>("Audio/ship_inventory/rocket_close");
    }

    public void OpenSound()
    {
        inventorySource.PlayOneShot(openSound);
    }

    public void CloseSound()
    {
        inventorySource.PlayOneShot(closeSound);
    }
}
