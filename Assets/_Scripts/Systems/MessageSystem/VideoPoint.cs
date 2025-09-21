using System.Threading.Tasks;
using UnityEngine;


public class VideoPoint : UIPoint
{
    [SerializeField] private GameObject overlay;
    private bool shown = false;

    private void Start()
    {
        shown=true;
        Toggle();
    }
    
    

    public override void Show()
    {
        gameObject.SetActive(true);
        overlay.SetActive(false);
    }
    
    public override void Hide()
    {
        gameObject.SetActive(false);
        overlay.SetActive(true);
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
    

    public override Task ShowAnimateTransition()
    {
        Show();
        return Task.CompletedTask;
    }
    
    
    public override async Task HideAnimateTransition()
    {
        Hide();
    }
    
}
