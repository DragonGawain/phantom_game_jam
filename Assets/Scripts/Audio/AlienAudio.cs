using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAudio : MonoBehaviour
{
    AudioSource attackingSource;
    AudioSource tookDamageSource;
    AudioClip[] attackingSounds;
    AudioClip[] tookDamageSounds;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] temp = GetComponents<AudioSource>();
        attackingSource = temp[0];
        tookDamageSource = temp[1];

        attackingSounds = Resources.LoadAll<AudioClip>("Audio/alien_attack");
        tookDamageSounds = Resources.LoadAll<AudioClip>("Audio/damage_taken/Alien");
    }

    public void AttackingSound()
    {
        attackingSource.PlayOneShot(attackingSounds[Random.Range(0, attackingSounds.Length)]);
    }

    public void TookDamageSound()
    {
        tookDamageSource.PlayOneShot(tookDamageSounds[Random.Range(0, tookDamageSounds.Length)]);
    }
}
