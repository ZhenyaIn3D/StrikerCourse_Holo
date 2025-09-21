using UnityEngine;
using Vuforia;

public class Test : MonoBehaviour
{
   [SerializeField] private ModelTargetBehaviour modelTarget;
   
   [Button("Setup")]
   public void SetLineRenderer()
   {
      ImageTargetBehaviour f;
      var lines= GetComponentsInChildren<LineRendererSetup>(true);

      foreach (var line in lines)
      {
         line.Setup();
      }
   }

   
   [Button("Reset")]
   public void ResetModelTarget()
   {
      modelTarget.Reset();
   }
   
}

