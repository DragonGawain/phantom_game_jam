using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAudio : MonoBehaviour
{
    AudioSource shootingSource;
    AudioClip shootingSound;

    // Start is called before the first frame update
    void Awake()
    {
        shootingSource = GetComponent<AudioSource>();
        shootingSound = Resources.Load<AudioClip>("Audio/laser_fire/laser_fire");
    }

    public void ShootSound()
    {
        shootingSource.PlayOneShot(shootingSound, 7);
    }
}
