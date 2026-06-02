using UnityEngine;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class MenuManipulatorToggle : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the Object Manipulator component. If not assigned, will try to find it on this GameObject.")]
    [SerializeField] private ObjectManipulator objectManipulator;

    [Tooltip("Reference to the LazyFollow component. If not assigned, will try to find it on this GameObject.")]
    [SerializeField] private LazyFollow lazyFollow;

    [Header("Settings")]
    [Tooltip("How long LazyFollow stays active on startup before disabling (seconds)")]
    [SerializeField] private float lazyFollowInitDuration = 1.5f;

    private void Awake()
    {
        if (objectManipulator == null)
            objectManipulator = GetComponent<ObjectManipulator>();

        if (objectManipulator == null)
            Debug.LogError($"MenuManipulatorToggle: No ObjectManipulator found on {gameObject.name}.");

        if (lazyFollow == null)
            lazyFollow = GetComponent<LazyFollow>();

        if (lazyFollow == null)
            Debug.LogError($"MenuManipulatorToggle: No LazyFollow found on {gameObject.name}.");
    }

    private void OnEnable()
    {
        // Activate LazyFollow briefly so the menu spawns in front of the user
        if (lazyFollow != null)
        {
            lazyFollow.enabled = true;
            Invoke(nameof(DisableLazyFollow), lazyFollowInitDuration);
        }
    }

    private void OnDisable()
    {
        // Clean up any pending Invoke if the object is disabled early
        CancelInvoke(nameof(DisableLazyFollow));
    }

    private void DisableLazyFollow()
    {
        if (lazyFollow != null)
            lazyFollow.enabled = false;
    }

    /// <summary>
    /// Toggles the Object Manipulator on/off.
    /// Enabled = can be moved freely. Disabled = stays in place, cannot be moved.
    /// </summary>
    public void ToggleManipulator()
    {
        if (objectManipulator == null) return;
        SetManipulatorEnabled(!objectManipulator.enabled);
    }

    /// <summary>
    /// Explicitly sets the Object Manipulator enabled state.
    /// True = unblocked (free to move). False = blocked (stays in place).
    /// </summary>
    public void SetManipulatorEnabled(bool enabled)
    {
        if (objectManipulator == null) return;
        objectManipulator.enabled = enabled;
    }

    /// <summary>
    /// Returns whether the Object Manipulator is currently enabled.
    /// </summary>
    public bool IsManipulatorEnabled()
    {
        return objectManipulator != null && objectManipulator.enabled;
    }
}