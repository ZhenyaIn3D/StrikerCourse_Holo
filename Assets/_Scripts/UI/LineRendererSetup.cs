using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LineRendererSetup : MonoBehaviour
{

    private LineRenderer lr;
    private Transform start;
    public Transform target;
    private CountdownTimer timer;
    private bool updatePos;
    public static float animationTime = 0.2f;
    public Transform test;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        start = transform.GetChild(1);
        target = transform.GetChild(0);
        
        timer = new CountdownTimer(animationTime);
        timer.OnTimerStart += () => updatePos = false;
        timer.OnTimerStop += () => updatePos = true;
    }
    
    

    private void Update()
    {
        if (updatePos)
        {
            if(test!=null) target = test;
            SetPositions();
        }
    }

    private void Cache()
    {
        lr = GetComponent<LineRenderer>();
        start = transform.GetChild(1);
        target = transform.GetChild(0);
    }
    
    
    [Button("Setup")]
    public void Setup()
    {
        Cache();
        
        lr.SetPosition(0,start.position);
        lr.SetPosition(1,target.position);
    }
    
    private void SetPositions()
    {
        lr.SetPosition(0,start.position);
        lr.SetPosition(1,target.position);
    }
    

    public async Task AnimateLine(bool show)
    {
        SetPositions();
        timer.Start();
        var start = show ? this.start.position : this.target.position;
        var target = show ? this.target.position : this.start.position;

        while (!timer.IsFinished)
        {
            var pos = Vector3.Lerp(start, target, timer.GetNormalizeTime());
            lr.SetPosition(1,pos);
            
            await Task.Yield();
        }
        
        lr.SetPosition(1,target);
    }

    public void ToggleUpdatePos(bool update)
    {
        updatePos = update;
    }
            
}
