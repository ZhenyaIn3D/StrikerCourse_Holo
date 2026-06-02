using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("Arrows")]
    public GameObject leftArrow;
    public GameObject rightArrow;

    [Header("Target")]
    public GameObject target;

    [Header("Settings")]
    public float deviationThreshold = 30f;

    void Awake()
    {
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
    }

    void Update()
    {
        // If target is not active, hide everything and do nothing
        if (target == null || !target.activeInHierarchy)
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            return;
        }

        float deviation = GetHorizontalDeviation();

        if (Mathf.Abs(deviation) < deviationThreshold)
        {
            // User is looking close enough to the target
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            return;
        }

        // deviation > 0 means target is to the right, < 0 means to the left
        // if exactly equal (shouldn't happen but covered) show right
        bool showRight = deviation >= 0f;
        leftArrow.SetActive(!showRight);
        rightArrow.SetActive(showRight);
    }


    private float GetHorizontalDeviation()
    {
        Transform cam = Camera.main.transform;

        // Project target direction onto camera's horizontal plane
        Vector3 toTarget = target.transform.position - cam.position;

        // Use camera's right axis to get signed horizontal angle
        float rightDot = Vector3.Dot(toTarget.normalized, cam.right);
        float forwardDot = Vector3.Dot(toTarget.normalized, cam.forward);

        float angle = Mathf.Atan2(rightDot, forwardDot) * Mathf.Rad2Deg;
        return angle;
    }
    
}