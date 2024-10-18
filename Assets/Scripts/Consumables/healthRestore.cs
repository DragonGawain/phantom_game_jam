using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthRestore : MonoBehaviour
{
    [SerializeField, Range(1, 5)]
    int hpRestore = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            pc.RestoreHealth(hpRestore);
            Destroy(gameObject);
        }
        else if (other.TryGetComponent<Human>(out Human human))
        {
            human.RestoreHealth(gameObject, hpRestore);
            Destroy(gameObject);
        }
    }
}
