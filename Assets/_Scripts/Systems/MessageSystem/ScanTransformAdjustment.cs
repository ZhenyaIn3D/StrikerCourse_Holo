using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;


public class ScanTransformAdjustment : MonoBehaviour
{
    [SerializeField] private GameObject scanSource;
    private DefaultObserverEventHandler eventHandler;
    private ObserverBehaviour mObserverBehaviour;
    private CountdownTimer timer;
    private StopwatchTimer overallTimer;
    private Vector3 pos = Vector3.zero;
    private Status currentStatusLevel = Status.TRACKED;
    
    private bool startFollow;
    private bool scanInitialized;
    private float smoothFactor = 0.8f;
    private float scanTime = 4;

    public ScanTargetType scanTargetType;
    public Action<ScanTargetType> OnFinishScan;
    
    private void Start()
    {
        eventHandler = scanSource.GetComponent<DefaultObserverEventHandler>();
        mObserverBehaviour = scanSource.GetComponent<ObserverBehaviour>();
        mObserverBehaviour.OnTargetStatusChanged += HandleScanStatusChange;
        eventHandler.OnTargetFound.AddListener(FlagFirstScan);
        eventHandler.OnTargetFound.AddListener(StartOverAllTimer);
        
        timer = new CountdownTimer(scanTime);
        overallTimer = new StopwatchTimer();
    }

    void Update()
    {
        AdjustTransform();
    }

    private void AdjustTransform()
    {
        if(!startFollow || !scanInitialized ) return;
        //Using exponential moving average (give a greater weight to he more recent scans)
        pos = ((1-smoothFactor)* pos+ (smoothFactor) * scanSource.transform.position);

        if (timer.IsFinished)
        {
            scanInitialized=false;
            startFollow=false;
            transform.position = pos;
            transform.rotation = scanSource.transform.rotation;
            overallTimer.Stop();
            ResetScanLevel();
            OnFinishScan?.Invoke(scanTargetType);
        }
    }
    
    /// <summary>
    /// We want to register only the results when the scan is TRACKED and disacrd every other scan.
    /// To avoid the case we did not get enough results in the scan time we are pausing the timer so it will keep going only when the scan is TRACKED
    /// </summary>
    /// <param name="ob"></param>
    /// <param name="ts"></param>

    private void HandleScanStatusChange(ObserverBehaviour ob, TargetStatus ts)
    {
        if (ts.Status == currentStatusLevel)
        {
            if(!timer.IsFinished) timer.Resume();
        }
        else
        {
            timer.Pause();
        }
        if(!scanInitialized) return;
        startFollow = ts.Status == currentStatusLevel;
    }
    
    
    /// <summary>
    /// First we let vuforia to handle the scan result and position the model target
    /// We wait for a few seconds because sometimes the initial position is not correct
    /// </summary>

    private async void FlagFirstScan()
    {
        if(scanInitialized) return;
        if(mObserverBehaviour.TargetStatus.Status!=currentStatusLevel) return;
        timer.Reset(1);
        timer.Start();
        while (!timer.IsFinished)
        {
            await Task.Yield();
        }
        scanInitialized=true;
        startFollow=mObserverBehaviour.TargetStatus.Status==currentStatusLevel;
        timer.Reset(scanTime);
        timer.Start();
    }

    private void ResetScanLevel()
    {
        currentStatusLevel = Status.TRACKED;
        Debug.Log("Changed To Tracked");
    }

    private void LowerScanLevel()
    {
        Debug.Log("Changed To Extended");
        currentStatusLevel = Status.EXTENDED_TRACKED;
    }

    private void StartOverAllTimer()
    {
        if(overallTimer.GetTime()!=0) return;
        overallTimer.Reset();
        overallTimer.Start();
    }
}
