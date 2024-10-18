using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAudio : MonoBehaviour
{
    AudioSource shootingSource;
    AudioSource tookDamageSource;
    AudioClip[] shootingSounds;
    AudioClip tookDamageSound;

    // Start is called before the first frame update
    void Awake()
    {
        AudioSource[] temp = GetComponents<AudioSource>();
        shootingSource = temp[0];
        tookDamageSource = temp[1];

        shootingSounds = Resources.LoadAll<AudioClip>("Audio/laser_fire/Enemy");
        tookDamageSound = Resources.Load<AudioClip>("Audio/damage_taken/enemy_damage_taken");
    }

    public void ShootSound()
    {
        shootingSource.PlayOneShot(shootingSounds[Random.Range(0, shootingSounds.Length)], 7);
    }

    public void TookDamageSound()
    {
        tookDamageSource.PlayOneShot(tookDamageSound);
    }
}
