using UnityEngine;
using MixedReality.Toolkit.SpatialManipulation;
public class VideoManipulatorToggle : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the Object Manipulator component. If not assigned, will try to find it on this GameObject.")]
    [SerializeField] private ObjectManipulator objectManipulator;

    [Header("Settings")]
    [Tooltip("If true, also reset rotation when returning to start position")]
    [SerializeField] private bool resetRotation = true;
    
    [Tooltip("If true, also reset scale when returning to start position")]
    [SerializeField] private bool resetScale = true;
    
    [Tooltip("Duration of the lerp animation when returning to start position (0 for instant)")]
    [SerializeField] private float returnDuration = 0.5f;

    // Store initial transform values
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;
    
    // Animation state
    private bool isReturning = false;
    private float returnProgress = 0f;
    private Vector3 returnFromPosition;
    private Quaternion returnFromRotation;
    private Vector3 returnFromScale;
    
    private void Awake()
    {
        SetCurrentAsStartPosition();
        // Cache starting transform values
        // startPosition = transform.localPosition;
        // startRotation = transform.localRotation;
        
        // Get ObjectManipulator if not assigned
        if (objectManipulator == null)
        {
            objectManipulator = GetComponent<ObjectManipulator>();
        }

        if (objectManipulator == null)
        {
            Debug.LogError($"ObjectManipulatorToggle: No ObjectManipulator found on {gameObject.name}. Please assign one or add it to this GameObject.");
        }
        
        
    }

    private void Update()
    {
        // Handle smooth return animation
        if (isReturning && returnDuration > 0)
        {
            returnProgress += Time.deltaTime / returnDuration;
            
            if (returnProgress >= 1f)
            {
                // Animation complete
                transform.localPosition = startPosition;
                if (resetRotation) transform.localRotation = startRotation;
                if (resetScale) transform.localScale = startScale;
                isReturning = false;
            }
            else
            {
                // Interpolate
                float t = EaseOutCubic(returnProgress);
                transform.localPosition = Vector3.Lerp(returnFromPosition, startPosition, t);
                if (resetRotation) transform.localRotation = Quaternion.Slerp(returnFromRotation, startRotation, t);
                if (resetScale) transform.localScale = Vector3.Lerp(returnFromScale, startScale, t);
            }
        }
    }

    /// <summary>
    /// Toggles the Object Manipulator on/off.
    /// When turning off, returns the object to its starting position.
    /// Call this method from your button's OnClick event.
    /// </summary>
    public void ToggleManipulator()
    {
        if (objectManipulator == null) return;

        bool newState = !objectManipulator.enabled;
        SetManipulatorEnabled(newState);
    }

    /// <summary>
    /// Explicitly sets the Object Manipulator enabled state.
    /// When disabling, returns the object to its starting position.
    /// </summary>
    /// <param name="enabled">Whether to enable or disable the manipulator</param>
    public void SetManipulatorEnabled(bool enabled)
    {
        if (objectManipulator == null) return;

        objectManipulator.enabled = enabled;

        if (!enabled)
        {
            ReturnToStartPosition();
        }
        else
        {
            // Stop any ongoing return animation when re-enabling
            isReturning = false;
        }
    }

    /// <summary>
    /// Returns the object to its starting position, rotation, and scale.
    /// </summary>
    public void ReturnToStartPosition()
    {
        if (returnDuration > 0)
        {
            // Start smooth return animation
            returnFromPosition = transform.localPosition;
            returnFromRotation = transform.localRotation;
            returnFromScale = transform.localScale;
            returnProgress = 0f;
            isReturning = true;
        }
        else
        {
            // Instant return
            transform.localPosition = startPosition;
            if (resetRotation) transform.localRotation = startRotation;
            if (resetScale) transform.localScale = startScale;
        }
    }

    /// <summary>
    /// Updates the stored start position to the current position.
    /// Useful if you want to set a new "home" position at runtime.
    /// </summary>
    public void SetCurrentAsStartPosition()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
        startScale = transform.localScale;
    }

    /// <summary>
    /// Returns whether the Object Manipulator is currently enabled.
    /// </summary>
    public bool IsManipulatorEnabled()
    {
        return objectManipulator != null && objectManipulator.enabled;
    }

    // Easing function for smooth animation
    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
    
    
}