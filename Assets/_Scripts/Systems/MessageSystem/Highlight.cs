using System.Threading.Tasks;
using UnityEngine;


public class Highlight : UIPoint
{
    private CountdownTimer timer;
    private bool shown = false;

    private void Start()
    {
        timer = new CountdownTimer(0.2f);
        shown=true;
        Toggle();
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

    public override async Task ShowAnimateTransition()
    {
        Show();
        timer.Start();
        var start = Vector3.one*0.01f;
        var target = Vector3.one;

        while (!timer.IsFinished)
        {
            transform.localScale = Vector3.Lerp(start, target, timer.GetNormalizeTime());
            await Task.Yield();
        }

        transform.localScale = target;
    }

    public override async Task HideAnimateTransition()
    {
        timer.Start();
        var start = Vector3.one;
        var target = Vector3.one*0.01f;

        while (!timer.IsFinished)
        {
            transform.localScale = Vector3.Lerp(start, target, timer.GetNormalizeTime());
            await Task.Yield();
        }

        transform.localScale = target;
        Hide();
    }
}
