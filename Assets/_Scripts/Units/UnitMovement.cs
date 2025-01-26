using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/// <summary>
/// Manages player-related Unit movement logic.
/// </summary>
/// <remarks>
/// This class handles player input to apply movement force.
/// It should be attatched to any Unit GameObject for the player to control movement.
/// This script requires a reference to
/// </remarks>

public class UnitMovement : MonoBehaviour
{
    /// <summary>
    /// Reference variable to the MainCamera to cache.
    /// </summary>
    private Camera _mainViewCamera;

    /// <summary>
    /// Did the Player give the unit a command to move?
    /// </summary>
    public bool _isCommandedToMove;

    /// <summary>
    /// Reference variable to the local NavMeshAgent attatched to the Unit.
    /// </summary>
    private NavMeshAgent _agent;

    /// <summary>
    /// Reference variable to the ground LayerMask for navigation.
    /// </summary>
    public LayerMask _ground;

    /// <summary>
    /// Unity's built-in Start method, called before the first frame update.
    /// Typically used for any intialization logic that requires
    /// other Awake calls to have been completed.
    /// </summary>
    private void Start()
    {
        // Initialization Code:
        // Get the reference of the main camera and cache it
        // Get the reference of the navmesh agent on the unit and cache it
        _mainViewCamera = Camera.main;
        _agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Monobehaviour method called once per frame.
    /// </summary>
    /// <remarks>
    /// Will handle updating the unit's location depending on
    /// the player's input.
    /// </remarks>
    private void Update()
    {
        MoveUnit();
    }

    /// <summary>
    /// Move the unit to the destination of the Player's right mouse click.
    /// Checks for input of the right mouse button and moves the unit to the
    /// hit point of the raycast from the main camera.
    /// </summary>
    private void MoveUnit()
    {
        // Check for player input of the right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            // Create the raycast from the main camera viewpoint
            RaycastHit _hit;
            // Shoot the ray from the camera to the mouse position clicked
            Ray ray = _mainViewCamera.ScreenPointToRay(Input.mousePosition);
            // If the ray is hitting something, and is on ground layer
            //      set the destination of the agent to the position of the hit
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity, _ground))
            {
                _isCommandedToMove = true;
                _agent.SetDestination(_hit.point);
            }
        }

        DetectUnitMovement();
    }

    /// <summary>
    /// Detect if the selected unit has reached their end destination.
    /// If they have, return isCommandedToMove as false to indicate the unit has reached their final destination.
    /// </summary>
    private void DetectUnitMovement()
    {
        if (_agent.hasPath == false || _agent.remainingDistance <= _agent.stoppingDistance)
            _isCommandedToMove = false;
    }
}
