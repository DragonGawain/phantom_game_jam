using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEscapeRadius : MonoBehaviour
{
    Human human;

    private void Awake()
    {
        human = GetComponentInParent<Human>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (
            human.GetCombatState() == Enemy.CombatState.ATTACK
            && human.GetTarget() == other.transform
        )
            human.StopAttack();
    }
}
