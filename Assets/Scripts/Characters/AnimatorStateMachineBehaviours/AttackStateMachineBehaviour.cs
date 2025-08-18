using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Diablo.Characters;
using Diablo.AI;

public class AttackStateMachineBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<AttackStateController>()?.OnStartOfAttackState();
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<AttackStateController>()?.OnEndOfAttackState();
    }
}
