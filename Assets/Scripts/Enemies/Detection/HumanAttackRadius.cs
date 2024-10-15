using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAttackRadius : MonoBehaviour
{
    Human human;

    private void Awake()
    {
        human = GetComponentInParent<Human>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Alien") || other.CompareTag("Player"))
        {
            //
        }
    }
}
