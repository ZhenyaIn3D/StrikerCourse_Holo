using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UIPoint : MonoBehaviour
{
    public bool UseNarration { get; set; }
    public abstract void Show();
    public abstract void Hide();
    public abstract void Toggle();

    public abstract Task ShowAnimateTransition();

    public abstract Task HideAnimateTransition();

    public virtual float Narration()
    {
        return 0;
    }
    
    public virtual void ToggleBackgroundHighlight(bool highlight)
    {
    }

}
   
