using System.Threading.Tasks;
using UnityEngine;
using Vuforia;

public abstract class ScanTargetControllerBase : MonoBehaviour
{
   
    [Inject] protected ScansManager ScanManager;
    protected CountdownTimer timer;
    public abstract void SendShowStepRequest(StepName stepName);
    
    public abstract  void ResetTarget();
    public abstract  void ShowAll();
    public abstract  void HideAll();
    
    protected virtual async Task AnimateIcon(){}
  
    
    public virtual void ActivateScanTarget(ScanTargetType scanTargetType){}

    public virtual bool DeActivateScanTarget(ScanTargetType scanTargetType)
    {
        return true;
    }

}
