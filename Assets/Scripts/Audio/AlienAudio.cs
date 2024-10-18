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

        // attackingSounds = Resources.Load<AudioClip>("");
        tookDamageSounds = Resources.LoadAll<AudioClip>("Audio/damage_taken/Alien");
    }

    public void AttackingSound()
    {
        Debug.Log("alien attack sound");
    }

    public void TookDamageSound()
    {
        tookDamageSource.PlayOneShot(tookDamageSounds[Random.Range(0, tookDamageSounds.Length)]);
    }
}
