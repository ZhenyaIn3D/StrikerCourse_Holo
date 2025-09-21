using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ActivationController : MonoBehaviour
{
    [SerializeField] private ScanTargetType scanTargetType;
    [SerializeField] private GameObject[] targets;
    [SerializeField] private int delayTime;
    [Inject] private ScansManager scansManager;

    private void Start()
    {
        scansManager.OnFinishScan += Activate;
    }

    private void OnDestroy()
    {
        scansManager.OnFinishScan -= Activate;
    }

    private async void Activate(ScanTargetType stt)
    {
        if(scanTargetType!=stt) return;
        await Task.Delay(delayTime * 1000);
        foreach (var target in targets)
        {
            target.SetActive(true);
        }
    }
}
