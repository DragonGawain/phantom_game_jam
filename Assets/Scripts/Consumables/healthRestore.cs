using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthRestore : MonoBehaviour
{
    [SerializeField, Range(1, 5)]
    int hpRestore = 1;

    // ComponentAudio componentAudio;

    private void Awake()
    {
        // componentAudio = GetComponent<ComponentAudio>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController pc) && !PlayerCOntroller.isEndingSequence)
        {
            pc.RestoreHealth(hpRestore);
            AudioManager.HealthSound();
            Destroy(gameObject);
        }
        else if (other.TryGetComponent<Human>(out Human human))
        {
            human.RestoreHealth(gameObject, hpRestore);
            Destroy(gameObject);
        }
    }
}
