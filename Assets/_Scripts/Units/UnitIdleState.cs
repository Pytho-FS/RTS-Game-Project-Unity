using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitIdleState : StateMachineBehaviour
{
    /// <summary>
    /// Get a reference to the attack controller.
    /// </summary>
    private AttackController _attackController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _attackController = animator.transform.GetComponent<AttackController>();
        _attackController.SetIdleMaterial();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check if the unit has a target
        if (_attackController.targetToAttack != null)
        {
            // --- Transition to follow state ---
            animator.SetBool("isFollowing", true);
        }

        // Play sound etc.

    }
}
