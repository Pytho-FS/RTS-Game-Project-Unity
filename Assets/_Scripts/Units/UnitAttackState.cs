using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAttackState : StateMachineBehaviour
{
    /// <summary>
    /// Variable to determine the distance at which the unit will disengage their attacks.
    /// </summary>
    public float stopAttackingDistance = 1.2f;

    /// <summary>
    /// Reference to our attack controller script
    /// </summary>
    private AttackController _attackController;

    /// <summary>
    /// Reference to our unit's navmesh agent.
    /// </summary>
    private NavMeshAgent _unitAgent;

    public float _attackRate = 1f;
    private float attackTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _unitAgent = animator.transform.GetComponent<NavMeshAgent>();
        _attackController = _unitAgent.transform.GetComponent<AttackController>();
        _attackController.SetAttackStateMaterial();

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_attackController.targetToAttack != null && animator.transform.GetComponent<UnitMovement>()._isCommandedToMove == false)
        {
            LookAtTarget();

            // Move the unit towards the enemy
            _unitAgent.SetDestination(_attackController.targetToAttack.position);

            if (attackTimer <= 0)
            {
                AttackTarget();

                attackTimer = 1f / _attackRate;
            }
            else
                attackTimer -= Time.deltaTime;


            // Should unit still attack
            float distanceFromTarget = Vector3.Distance(_attackController.targetToAttack.position, animator.transform.position);

            if (distanceFromTarget > stopAttackingDistance || _attackController.targetToAttack == null)
            {
                animator.SetBool("isAttacking", false); // Move to follow state
            }

        }
    }

    private void AttackTarget()
    {
        var damageToInflict = _attackController._unitDamage;

        // Actually attack unit
        _attackController.targetToAttack.GetComponent<Unit>().TakeDamage(damageToInflict);
    }

    private void LookAtTarget()
    {
        Vector3 direction = _attackController.targetToAttack.position - _unitAgent.transform.position;
        _unitAgent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = _unitAgent.transform.eulerAngles.y;
        _unitAgent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
