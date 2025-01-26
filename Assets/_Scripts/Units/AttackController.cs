using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    /// <summary>
    /// A transform of the target the unit will attack.
    /// </summary>
    public Transform targetToAttack;

    public Material _idleStateMaterial;
    public Material _followStateMaterial;
    public Material _attackStateMaterial;

    [SerializeField] public float _unitDamage;

    public bool isPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayer && other.CompareTag("Enemy") && targetToAttack != null)
        {
            targetToAttack = null;
        }
    }

    public void SetIdleMaterial() => GetComponent<Renderer>().material = _idleStateMaterial;

    public void SetFollowStateMaterial() => GetComponent<Renderer>().material = _followStateMaterial;

    public void SetAttackStateMaterial() => GetComponent<Renderer>().material = _attackStateMaterial;

    /// <summary>
    /// Display the the follow distance, attack distance, and stop attack distance of the unit.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Follow Distance / Area
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 10f*0.2f);

        // Attack Distance / Area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.0f);

        // Stop Attack Distance / Area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }
}
