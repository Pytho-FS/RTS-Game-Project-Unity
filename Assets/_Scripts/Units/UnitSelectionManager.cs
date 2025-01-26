using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the selection of player-controlled units within the game.
/// Provides logic for clicking on Units, highlighting them, and tracking the current selection.
/// </summary>
/// <remarks>
/// This class ensures there is a single, centralized manager for selecting and deselecting units.
/// It inherits from <see cref="Singleton{UnitSelectionManager}"/> to maintain a unique instance
/// within the scene.
/// </remarks>
public class UnitSelectionManager : Singleton<UnitSelectionManager>
{
    /// <summary>
    /// List of all units currently available to the player.
    /// </summary>
    public List<GameObject> _allUnitsList = new List<GameObject>();

    /// <summary>
    /// List of currently selected units selected by the player.
    /// </summary>
    public List<GameObject> _unitsSelected = new List<GameObject>();

    /// <summary>
    /// This layer mask is to be assigned to each selectable Unit gameobject.
    /// This will be used to detect if we have clicked on a selectable unit.
    /// </summary>
    public LayerMask _clickable;

    /// <summary>
    /// This layer mask is to be assigned to the terrain.
    /// We need this ground layer mask to assign a ground marker.
    /// </summary>
    public LayerMask _ground;

    /// <summary>
    /// This layer mask is to be assigned to enemy units.
    /// We need this layer mask to determine what the player can send units after to attack.
    /// </summary>
    public LayerMask _attackable;

    /// <summary>
    /// This will be a visual representation of where the Unit will go.
    /// </summary>
    public GameObject _groundMarker;

    /// <summary>
    /// Reference variable to the MainCamera to cache.
    /// </summary>
    private Camera _mainViewCamera;

    /// <summary>
    /// Control when the attack cursor is visible to the player.
    /// </summary>
    public bool _attackCursorVisible;

    protected override void Awake()
    {
        base.Awake();

        // Custom initialization methods:
        OnInitialization();
    }

    /// <summary>
    /// Monobehaviour method called once per frame.
    /// </summary>
    /// <remarks>
    /// Will handle updating the unit being selected by the player.
    /// </remarks>
    private void Update()
    {
        TryToSelectUnit();
        HandleAttackingEnemyUnits();
    }

    /// <summary>
    /// Initialize the UnitSelectionManager.
    /// </summary>
    private void OnInitialization()
    {
        // Initialize the camera to reference the main view camera and cache it
        _mainViewCamera = Camera.main;
    }

    /// <summary>
    /// This method will be called whenever the Player attempts to select a
    /// specified Unit.
    /// </summary>
    /// <remarks>
    /// If the Player presses the left mouse button on a Unit, the Unit should then be
    /// selected and controllable by the Player.
    /// </remarks>
    private void TryToSelectUnit()
    {
        // Check for player input of the left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            // Create the raycast from the main camera viewpoint
            RaycastHit _hit;
            // Shoot the ray from the camera to the mouse position clicked
            Ray ray = _mainViewCamera.ScreenPointToRay(Input.mousePosition);
            // If the ray is hitting a clickable object, select the specified clickable object targeted
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity, _clickable))
            {
                // Check to see if the Player is holding down the left shift key to select multiple units
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    TryMultipleUnitSelection(_hit.collider.gameObject);
                }
                else // The player is only attempting to select a single Unit
                {
                    SelectClickedUnit(_hit.collider.gameObject); // Select the object targeted
                }
            }
            else // We are NOT hitting a clickable object
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                    DeselectAllUnits(); // Deselect all Units
            }
        }

        // Check for player input of the right mouse button and if there are any units currently selected
        if (Input.GetMouseButtonDown(1) && _unitsSelected.Count > 0)
        {
            // Create the raycast from the main camera viewpoint
            RaycastHit _hit;
            // Shoot the ray from the camera to the mouse position clicked
            Ray ray = _mainViewCamera.ScreenPointToRay(Input.mousePosition);
            // If the ray is hitting the ground layer mask, take the ground marker and set the position of the marker
            // to the hit point and make the gameobject active.
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity, _ground))
            {
                // Assign the position
                _groundMarker.transform.position = new Vector3(_hit.point.x, .001f, _hit.point.z);

                // Since this is animated(eventually), deactivate it first.
                _groundMarker.SetActive(false);
                _groundMarker.SetActive(true);
            }
        }
    }

    ///<summary>
    /// Handle attacking enemy units.
    /// </summary>
    private void HandleAttackingEnemyUnits()
    {
        // Check to see if there are any valid selected units to command to attack
        if (_unitsSelected.Count > 0 && AtleastOneOffensiveUnit(_unitsSelected))
        {
            // Create the raycast from the main camera viewpoint
            RaycastHit _hit;
            // Shoot the ray from the camera to the mouse position clicked
            Ray ray = _mainViewCamera.ScreenPointToRay(Input.mousePosition);
            // If the ray is hitting the attackable enemy unit
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity, _attackable))
            {
                Debug.Log("Enemy hovered with mouse.");

                _attackCursorVisible = true;

                if (Input.GetMouseButtonDown(1))
                {
                    Transform target = _hit.transform;

                    foreach (GameObject unit in _unitsSelected)
                    {
                        if (unit.GetComponent<AttackController>())
                        {
                            unit.GetComponent<AttackController>().targetToAttack = target;
                        }
                    }
                }

            }
            else
            {
                _attackCursorVisible = false;
            }
        }
    }

    private bool AtleastOneOffensiveUnit(List<GameObject> unitsSelected)
    {
        foreach (GameObject unit in unitsSelected)
        {
            if (unit.GetComponent<AttackController>())
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Deselects all units selected/clicked on by the Player.
    /// </summary>
    public void DeselectAllUnits()
    {
        // Disable the movement and remove the selection indicator for all units within the selected unit's list.
        foreach (var unit in _unitsSelected)
        {
            SelectUnit(unit, false);
        }

        _groundMarker.SetActive(false); // Clear the ground marker

        _unitsSelected.Clear(); // Clear the list of selected units.

    }

    /// <summary>
    /// Selects the specified Unit clicked on by the Player.
    /// </summary>
    /// <param name="gameObject">The Unit gameObject that is being selected.</param>
    private void SelectClickedUnit(GameObject unit)
    {
        // Before we first select a unit, deselect all units.
        DeselectAllUnits();

        // Add the current unit to the _unitsSelected list
        _unitsSelected.Add(unit);

        // Trigger the Unit's selection indicator to tell the Player the unit is currently selected
        SelectUnit(unit, true);
    }

    /// <summary>
    /// This method will execute upon the Player holding down Left Shift key,
    /// and clicking on available units to select them.
    /// </summary>
    /// <param name="unit">The Unit gameObject that is being selected.</param>
    private void TryMultipleUnitSelection(GameObject unit)
    {
        // Check to see if the list currently contains the unit being clicked
        if (_unitsSelected.Contains(unit) == false)
        {
            // Add the selected unit to the selected unit's list and enable movement and trigger the selection indicator.
            _unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
        else
        {
            // If already added, disable the unit's movement and trigger the selection indicator for removal.
            SelectUnit(unit, false);
            _unitsSelected.Remove(unit);
        }
    }

    /// <summary>
    /// This method controls whether or not a Unit is able to move based on the boolean passed.
    /// </summary>
    /// <param name="unit">The gameObject that the Unit script is assigned too.</param>
    /// <param name="shouldMove">Boolean value that can either be true or false to allow Unit Movement.</param>
    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        // Enable the unit's movement according to the boolean parameter value passed
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }

    private void InitializeGroundMarker()
    {
        // Check for player input of the right mouse button and if there are any units currently selected
        if (Input.GetMouseButtonDown(1) && _unitsSelected.Count > 0)
        {
            // Create the raycast from the main camera viewpoint
            RaycastHit _hit;
            // Shoot the ray from the camera to the mouse position clicked
            Ray ray = _mainViewCamera.ScreenPointToRay(Input.mousePosition);
            // If the ray is hitting the ground layer mask, take the ground marker and set the position of the marker
            // to the hit point and make the gameobject active.
            if (Physics.Raycast(ray, out _hit, Mathf.Infinity, _ground))
            {
                // Assign the position
                _groundMarker.transform.position = _hit.point;

                // Since this is animated(eventually), deactivate it first.
                _groundMarker.SetActive(false);
                _groundMarker.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Activate the Unit Selection Indicator for the specific unit selected.
    /// </summary>
    /// <remarks>
    /// This visibily shows the Player exactly which Unit's they are currently selecting.
    /// </remarks>
    /// <param name="unit">The GameObject of the specific unit being selected.</param>
    /// <param name="isVisibile">The boolean value of whether or not the indicator should be visible.</param>
    private void TriggerUnitSelectionMarker(GameObject unit, bool isVisible)
    {
        unit.transform.GetChild(0).gameObject.SetActive(isVisible);
    }

    /// <summary>
    /// Third type of unit selection, this method will allow the Player to drag the mouse,
    /// creating a selection box on the screen then selecting all units within that box.
    /// </summary>
    /// <param name="unit">The gameObject of the unit.</param>
    public void DragSelect(GameObject unit)
    {
        // Check if these units are not selected
        if (_unitsSelected.Contains(unit) == false)
        {
            _unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
    }

    /// <summary>
    /// Select and activate the unit accordingly.
    /// </summary>
    /// <param name="unit">The gameObject of the Unit.</param>
    /// <param name="isSelected">The boolean value of whether or not the Unit is selected.</param>
    private void SelectUnit(GameObject unit, bool isSelected)
    {
        TriggerUnitSelectionMarker(unit, isSelected);
        EnableUnitMovement(unit, isSelected);
    }
}
