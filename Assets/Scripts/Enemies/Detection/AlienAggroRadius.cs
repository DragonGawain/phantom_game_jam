using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAggroRadius : MonoBehaviour
{
    AlienBase alienBase;

    // Start is called before the first frame update
    void Awake()
    {
        alienBase = GetComponentInParent<AlienBase>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Human"))
            alienBase.IncreaseAggro(other.transform);
    }
}
