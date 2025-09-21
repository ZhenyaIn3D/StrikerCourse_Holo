using System;
using UnityEngine;

public class ScanIconController : MonoBehaviour
{
    [Inject] private ScansManager scanManager;
    private Transform parent;
    private Vector3 initialPos;
    private Vector3 initialRot;

    private void Awake()
    {
        parent = transform.parent;
        initialPos = transform.localPosition;
        initialRot = transform.localRotation.eulerAngles;
        
    }

    private void Start()
    {
        scanManager.OnStartScan += LocateScan;
        scanManager.OnFinishScan += Deactivate;
        
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        scanManager.OnStartScan -= LocateScan;
        scanManager.OnFinishScan -= Deactivate;
    }



    private void LocateScan()
    {
        transform.SetParent(parent);
        transform.localPosition = initialPos;
        transform.localRotation = Quaternion.Euler(initialRot);
        transform.parent = null;
        gameObject.SetActive(true);
    }

    private void Deactivate(ScanTargetType scanType)
    {
        gameObject.SetActive(false);
        transform.SetParent(parent);
    }
}
