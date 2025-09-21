using UnityEngine;

public class LookAtWrapper : MonoBehaviour
{
    [SerializeField] private LookAt lookAt;
    [SerializeField] private Transform tank;
    [SerializeField] private Transform plane;
    
    public void ChangeTTargetToTank()
    {
        lookAt.ChangeTarget(tank);
    }
    
    public void ChangeTTargetToPlane()
    {
        lookAt.ChangeTarget(plane);
    }
}
