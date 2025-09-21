using System;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class ScanVisual : MonoBehaviour
{ 
    [SerializeField] private ScanTargetType scanTargetType;
    [Inject] private ScansManager scanManager;
   private CountdownTimer timer;
   private MeshRenderer mr;

   private float animationTime = 4;
   

   private void Awake()
   {
       mr = GetComponent<MeshRenderer>();
       timer = new CountdownTimer(animationTime);
       scanManager.OnFinishScan += FinishScanVisual;
   }


   private void OnDestroy()
   {
       scanManager.OnFinishScan -= FinishScanVisual;
   }

   private void FinishScanVisual(ScanTargetType scanTargetType)
   {
       if(this.scanTargetType!=scanTargetType) return;
       AnimateScan();
   }
   

   public async Task AnimateScan()
   {
       gameObject.SetActive(true);
       timer.Reset(animationTime);
       timer.Start();
       var mpb = new MaterialPropertyBlock();
       while (!timer.IsFinished)
       {
           mpb.SetFloat("_Progression", Mathf.Lerp(1,0,timer.GetNormalizeTime()));
           mr.SetPropertyBlock(mpb);
           await Task.Yield();
       }
       
       timer.Reset(0.2f);
       timer.Start();
       while (!timer.IsFinished)
       {
           
           mpb.SetFloat("_Alpha", Mathf.Lerp(1,0,timer.GetNormalizeTime()));
           mr.SetPropertyBlock(mpb);
           await Task.Yield();
       }
       
       gameObject.SetActive(false);
       
   }
}
