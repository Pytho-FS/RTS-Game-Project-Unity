using UnityEngine;

/// <summary>
/// A generic singleton pattern base class for Unity Monobehaviours.
/// Inherit from this class to enforce a single instance of the derived class.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// public class UnitSelectionManager : Singleton<UnitSelectionManager>
/// {
///     protected override void Awake()
///     {
///         base.Awake();
///         // Custom Initilization
///     }
/// }
/// </code>
/// This pattern prevents multiple instances of the same singleton from existing.
/// If a duplicate is created, it will automatically be destroyed.
/// If you wish for a specific singleton to persist across all scenes, call DontDestroyOnLoad in Awake().
/// </remarks>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    /// <summary>
    /// Provides global access to the single instance of this class.
    /// </summary>
    public static T Instance {  get; private set; }

    /// <summary>
    /// When this script is awakened, it assigns itself to the static Instance if none exists.
    /// If a duplicate is detected, it will be destroyed.
    /// </summary>
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy the duplicate
            return;
        }

        Instance = this as T; // "Treat (this) as type T"
    }
}
