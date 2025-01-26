using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitFollowState : StateMachineBehaviour
{

    private AttackController _attackController;

    private NavMeshAgent _unitAgent;
    public float attackingDistance = 1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _attackController = animator.transform.GetComponent<AttackController>();
        _unitAgent = animator.transform.GetComponent<NavMeshAgent>();
        _attackController.SetFollowStateMaterial();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Should unit transition to idle state?
        if (_attackController.targetToAttack == null)
            animator.SetBool("isFollowing", false);
        else
        {
            // If there is no other direct command to move, follow the target
            if (animator.transform.GetComponent<UnitMovement>()._isCommandedToMove == false)
            {
                // Moving unit towards enemy
                _unitAgent.SetDestination(_attackController.targetToAttack.position);
                // Look at the enemy
                animator.transform.LookAt(_attackController.targetToAttack);

                // Should unit transition to Attack state?
                float distanceFromTarget = Vector3.Distance(_attackController.targetToAttack.position, animator.transform.position);

                if (distanceFromTarget < attackingDistance)
                {
                    _unitAgent.SetDestination(animator.transform.position);
                    animator.SetBool("isAttacking", true);
                }
            }
        }
    }
}
