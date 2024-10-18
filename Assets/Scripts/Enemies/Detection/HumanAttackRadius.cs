using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAttackRadius : MonoBehaviour
{
    Human human;

    private void Awake()
    {
        human = transform.parent.GetComponentInChildren<Human>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Alien") || other.CompareTag("Player") || other.CompareTag("Human"))
        {
            if (human.GetCombatState() == Enemy.CombatState.ARRIVE)
            {
                human.SetTarget(other.transform);
                human.SetCombatState(Enemy.CombatState.ATTACK);
            }
            else if (human.GetCombatState() == Enemy.CombatState.FLEE_TOWARDS)
            {
                human.SetTarget(other.transform);
                human.SetCombatState(Enemy.CombatState.FLEE);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (
            other.transform == human.GetTarget() && human.GetCombatState() == Enemy.CombatState.FLEE
        )
        {
            human.SetCombatState(Enemy.CombatState.FLEE_TOWARDS);
            human.SetTargetAsFleePoint();
        }
    }
}
