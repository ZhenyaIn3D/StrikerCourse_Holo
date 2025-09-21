using System.Threading.Tasks;
using Microsoft.MixedReality.GraphicsTools;
using MixedReality.Toolkit.UX;
using UnityEngine;
using Vuforia;

public class ScreenController : ScanTargetControllerBase
{
    [SerializeField] private ScalableDataSetTrackableBehaviour scanTarget;
    [SerializeField] private Material pickedmaterial;
    [SerializeField] private Material notPickedmaterial;
    [SerializeField] private Transform resetIcon;
    [Inject] private ScreenMessageSystem messageSystem;
    [SerializeField] private ScanVisual scanVisual;
    private PressableButton[] buttons;
    private CanvasElementRoundedRect[] cer;
    private int enumValueOffset;
    
    
    private void Start()
    {
        buttons = GetComponentsInChildren<PressableButton>();
        cer = new CanvasElementRoundedRect[buttons.Length];
        timer = new CountdownTimer(1);
        enumValueOffset = 5;
        InitButtons();
        
        scanTarget.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResetTarget();
        }
    }

    

    private void InitButtons()
    {
        for (var index = 0; index < buttons.Length-1; index++)
        {
            var stepName = (StepName)(index+enumValueOffset);
            buttons[index].OnClicked.AddListener(() => SendShowStepRequest(stepName));
            cer[index]=buttons[index].GetComponentInParent<CanvasElementRoundedRect>();
        }
        
        buttons[^1].OnClicked.AddListener(ResetTarget);
    }
    

    public override async void SendShowStepRequest(StepName stepName)
    {
        var currentButton = (int)messageSystem.CurrentStepName-enumValueOffset;
        HighLight(cer[currentButton],false);
        await messageSystem.ShowStep(stepName);
        currentButton = (int) messageSystem.CurrentStepName-enumValueOffset;
        HighLight(cer[currentButton],true);
    }

    private void HighLight(CanvasElementRoundedRect cerr, bool show)
    {
        var mat = show ? pickedmaterial : notPickedmaterial;
        cerr.material = mat;
    }
    
    protected override async Task AnimateIcon()
    {
        timer.Start();
        var currentRotation = Quaternion.Euler(0, 0, 0);
        var start = 0f;
        var target = 360f;
        resetIcon.rotation=currentRotation;
        while (!timer.IsFinished)
        {
            var rotZ = Mathf.Lerp(start, target, timer.GetNormalizeTime());
            resetIcon.rotation = Quaternion.Euler(0, 0, rotZ);
            await Task.Yield();
        }
    }

    public override async void ResetTarget()
    {
        await AnimateIcon();
        ScanManager.StartScan(1);
    }

    public override async void ShowAll()
    {
        await scanVisual.AnimateScan();
        messageSystem.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public override void HideAll()
    {
        messageSystem.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    
    public override void ActivateScanTarget(ScanTargetType st)=>scanTarget.gameObject.SetActive(true);

    public override bool DeActivateScanTarget(ScanTargetType st)
    {
        scanTarget.gameObject.SetActive(false);
        return true;
    }
}
