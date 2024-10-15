using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienEscapeRadius : MonoBehaviour
{
    AlienBase alienBase;

    // Start is called before the first frame update
    void Awake()
    {
        alienBase = GetComponentInParent<AlienBase>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Human"))
            alienBase.ReduceAggro(other.transform);
    }
}
