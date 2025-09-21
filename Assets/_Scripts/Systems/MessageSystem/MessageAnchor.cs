using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using CustomSoundSystem;
using CustomSoundSystem.RunTime;
using Microsoft.MixedReality.GraphicsTools;
using MixedReality.Toolkit.UX;
using UnityEngine;


public class MessageAnchor : UIPoint
{
    [SerializeField] private CanvasElementRoundedRect background;
    [Inject] private GlobalReferences gr;
    [Inject] private SoundSystem soundSystem;
    private LineRendererSetup[] lineRenderersSetup;
    private Transform contentParent;
    private PressableButton button;
    private UIPoint[] children;
    private CountdownTimer timer;
    private Vector3 initialSize;
    private static float tranistionTime = 0.2f;
    private bool childrenShown;
    private bool shown = false;

    
    public SoundTypeKeys narration;

    private void Start()
    {
        children = GetComponentsInChildren<UIPoint>(true);
        button = GetComponentInChildren<PressableButton>(true);
        lineRenderersSetup = transform.GetChild(0).GetComponentsInChildren<LineRendererSetup>(true);

        foreach (var lineRendererSetup in lineRenderersSetup)
        {
            lineRendererSetup.transform.parent = transform;
        }
       
        contentParent = transform.GetChild(0);
        
        timer = new CountdownTimer(tranistionTime);
        initialSize = contentParent.transform.localScale;
        
        contentParent.transform.localScale = Vector3.zero;
        
        if (children.Length > 1 && button!=null)
        {
            button.OnClicked.AddListener(ToggleChildren);
        }
        
        shown=true;
        Toggle();

        UseNarration = narration != SoundTypeKeys.None;
    }
    

    private void OnEnable()
    {
         childrenShown = false;
    }
    
    public override void Show()
    {
        gameObject.SetActive(true);
    }
    
    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void Toggle()
    {
        if (shown)
        {
            HideAnimateTransition();
        }
        else
        {
            ShowAnimateTransition();
        }

        shown = !shown;
    }

    private void ToggleChildren()
    {
        foreach (var c in children)
        {
            if (c == this) continue;
            if (childrenShown)
            {
                c.HideAnimateTransition();
            }
            else
            {
                c.ShowAnimateTransition();
            }
            
        }
        
        childrenShown = !childrenShown;
    }

    public override async Task ShowAnimateTransition()
    {
        //if(Cancelled) return;
        Show();
        await AnimateLine(true);
        await AnimateBackPlate(true);
        //ToggleChildren();
    }
    
    
    public override async Task HideAnimateTransition()
    {
        soundSystem.Stop(SoundSourceKeys.Narration.ToString());
        ToggleBackgroundHighlight(false);
        foreach (var c in children)
        {
            if (c == this) continue;
            if (childrenShown)
            {
               await c.HideAnimateTransition();
            }
            
            childrenShown = !childrenShown;
        }
        await AnimateBackPlate(false);
        await AnimateLine(false);
        Hide();
        
    }

    public override float Narration()
    {
        //return base.Narration();
        var soundSource = SoundSourceKeys.Narration.ToString();
        soundSystem.Play(soundSource,narration.ToString());
        Debug.Log(soundSystem.GetCurrentClipLength(soundSource));
        //StartCoroutine(NarrationOverTime());
        return soundSystem.GetCurrentClipLength(soundSource);
    }
    
    
    

    private async  Task AnimateBackPlate(bool show)
    {
        var start = show ? Vector3.zero : initialSize;
        var target = show ? initialSize : Vector3.zero;
        timer.Start();
        while (!timer.IsFinished)
        {
            contentParent.transform.localScale = Vector3.Lerp(start, target, timer.GetNormalizeTime());
            await Task.Yield();
        }

        contentParent.transform.localScale = target;
    }

    private async  Task AnimateButton(bool show)
    {
        var start = show ? Vector3.zero : Vector3.one;
        var target = show ? Vector3.one : Vector3.zero;
        timer.Start();
        while (!timer.IsFinished)
        {
            button.transform.localScale = Vector3.Lerp(start, target, timer.GetNormalizeTime());
            await Task.Yield();
        }
    }

    private async Task AnimateLine(bool show)
    {
       
        foreach (var lr in lineRenderersSetup)
        { 
            lr.AnimateLine(show);
        }

        await Task.Delay((int)LineRendererSetup.animationTime*1000);
    }
    
    public override void ToggleBackgroundHighlight(bool highlight)
    {
        var mat = highlight
            ? gr.references.GetReference(ReferencesKeys.HighlightBackground)
            : gr.references.GetReference(ReferencesKeys.RegularBackground);
        background.material = (Material)mat;
    }

    

    // IEnumerator NarrationOverTime()
    // {
    //     ToggleBackgroundHighlight(true);
    //     var soundSource = SoundSourceKeys.Narration.ToString();
    //     while (soundSystem.GetSoundUnitProgression(soundSource)<0.99f)
    //     {
    //         yield return null;
    //         //Debug.Log("Narration"+ gameObject.name);
    //     }
    //     ToggleBackgroundHighlight(false);
    // }
}