using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;


    private void Update()
    {
        transform.LookAt(target);
        
        transform.rotation*=Quaternion.Euler(offset);
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
