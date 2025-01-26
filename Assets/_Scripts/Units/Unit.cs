using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private float _unitHealth;
    [SerializeField] public float _unitMaxHealth;

    public HealthTracker _healthTracker;


    /// <summary>
    /// Unity's built-in Start method, called before the first frame update.
    /// Typically used for any intialization logic that requires
    /// other Awake calls to have been completed.
    /// </summary>
    private void Start()
    {
        InitializeUnit();
    }

    /// <summary>
    /// This function is called upon the creation of a Unit gameObject.
    /// </summary>
    /// <remarks>
    /// Each time a unit is created it will automatically be added to the _allUnitsList
    /// within the UnitSelectionManager.
    /// </remarks>
    private void InitializeUnit()
    {
        UnitSelectionManager.Instance._allUnitsList.Add(gameObject);

        _unitHealth = _unitMaxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        _healthTracker.UpdateSliderValue(_unitHealth, _unitMaxHealth);

        if (_unitHealth <= 0)
        {
            // Unit Death logic

            // Destruction or dying animation

            // Dying sound effect


            Destroy(gameObject); // Destroy this unit
        }
    }

    /// <summary>
    /// This function is called upon the destruction of a Unit gameObject.
    /// </summary>
    /// <remarks>
    /// Each time a unit is destroyed, it will automatically be removed from the _allUnitsList
    /// within the UnitSelectionManager.
    /// </remarks>
    private void OnDestroy() => UnitSelectionManager.Instance._allUnitsList.Remove(gameObject);

    public void TakeDamage(float damageToInflict)
    {
        _unitHealth -= damageToInflict;
        UpdateHealthUI();
    }
}
