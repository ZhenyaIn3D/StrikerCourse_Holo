using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CustomSoundSystem;
using CustomSoundSystem.RunTime;
using MixedReality.Toolkit.SpatialManipulation;
using MixedReality.Toolkit.UX;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ScenarioManager : ScanTargetControllerBase
{
    [SerializeField] private MessageSystem[] messagesSystems;
    [SerializeField] private MessageSystem[] messageSystemsCatalog; //COURSE3
    
    [SerializeField] private ScenarioConfig[] scenarios;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject ActionRequiredTag;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Material[] mats;
    [SerializeField] private GameObject scanOptions;
    [SerializeField]private PressableButton[] buttons;

    private Action<string>[] vchActions;
    
    [Inject] private SoundSystem soundSystem;
    [Inject] private InputManager inputManager;
    [Inject] private TimelineController timelineController;
    private Follow follow;
    private Transform menuParent;
    private TextMeshProUGUI[] buttonsText;
    private RawImage[] buttonsRawImages;
    private CountdownTimer timer;
    private CountdownTimer pressDelayTimer;
    private bool[] scanStatus;
    private bool cancelled;
    private bool inProgress;
    private int currentStep = -1;
    private int currentSenario = 0;

    public event Action<StepName> OnStepCompleted;
    public event Action OnStartFinalSequence;

    public PressableButton CoursesMenu;
    [SerializeField] GameObject AdvancedUtilities;
    private void Start()
    {
        menuParent = transform.GetChild(0);
        follow=menuParent.GetComponent<Follow>();
        pressDelayTimer = new CountdownTimer(1f);
        pressDelayTimer.Start();
        scanStatus = new bool[Enum.GetValues(typeof(ScanTargetType)).Length];
        InitButtons();
        
        inputManager.SubscribeToSpeech("Continue",Next);
        inputManager.SubscribeToSpeech("Back",Back);
    }

    private void OnDestroy()
    {
        inputManager.UnSubscribeToSpeech("Continue",Next);
        inputManager.UnSubscribeToSpeech("Back",Back);
    }
    

    private void InitButtons()
    {
        timer = new CountdownTimer(0.2f);
        buttonsText = new TextMeshProUGUI[buttons.Length];
        buttonsRawImages = new RawImage[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            var index = i;
            buttons[i].OnClicked.AddListener(() => PressButton(index));
            buttonsText[i] = buttons[i].GetComponentInChildren<TextMeshProUGUI>(true);
            buttonsRawImages[i] = buttons[i].transform.GetChild(0).GetComponent<RawImage>();
        }


    }

    private void InitVCHActions()
    {
        
        
    }
    
    private void Next()
    {
        HideStep();
        currentStep++;
        if (currentStep >= scenarios[currentSenario].menuConfig.Length) // if finished steps in current coursem change current scenario and show steps from the beginning
        {
            currentSenario++;
            currentStep = 0;
            currentSenario = Mathf.Clamp(currentSenario, 0, scenarios.Length-1);
        }
        ShowStep();
    }

    private void Back()
    {
        HideStep();
        currentStep--;
        if (currentStep < 0)
        {
            if(currentSenario==0) return;
            currentSenario--;
            currentStep = scenarios[currentSenario].menuConfig.Length - 1;
            currentSenario = Mathf.Clamp(currentSenario, 0, scenarios.Length - 1);
        }
        ShowStep();
        timelineController.ResetSequence();
    }

    private void SetButtonMaterial(int index, bool isNext)
    {
        var matsIndex = isNext ? 1 : 0;
        buttonsRawImages[index].material = mats[matsIndex];
    }

    private void UpdateUI()
    {
        var menuConfig = scenarios[currentSenario].menuConfig[currentStep];
        UpdateButtons(menuConfig);
        UpdateText(menuConfig);
        
        if(menuConfig.isInteractive)ActionRequiredTag.SetActive(true);
        else ActionRequiredTag.SetActive(false);
    }

    private void PlaySound()
    {
        var menuConfig = scenarios[currentSenario].menuConfig[currentStep];
        if (menuConfig.hasSound)
        {
            soundSystem.Play(SoundSourceKeys.Scan.ToString(),menuConfig.SoundTypeKeys.ToString());
        }
    }

    private void UpdateButtons(StepConfig stepConfig)
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
        
        for (int i = 0; i <stepConfig.numOfButtons ; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttonsText[i].text = stepConfig.buttonTexts[i];
            SetButtonMaterial(i,scenarios[currentSenario].menuConfig[currentStep].actions[i] == MenuActions.Next);
        }
    }

    private void UpdateText(StepConfig stepConfig)
    {
        var text = stepConfig.text; 
        
         if (!string.IsNullOrEmpty(text))
         {
             description.text = text;
             description.gameObject.SetActive(true);
         }
         else //if no text - dont show 
         {
             description.gameObject.SetActive(false);
         }
         
        text = stepConfig.title; 
        if (!string.IsNullOrEmpty(text))
        {
            title.text = text;
            title.gameObject.SetActive(true);
        }
        else //if no title - dont show 
        {
            title.gameObject.SetActive(false);
        }
    }
    

    private void RestartScenario()
    {
        currentStep = 0;
        ShowStep();
        timelineController.ResetSequence();
    }
    
    public void BackToMenu()
    {
        HideStep();
        currentSenario = 0;
        currentStep = 0;
        
        ShowStep();
        timelineController.ResetSequence();
    }

    public void PressButton(int index)
    {
        if(!pressDelayTimer.IsFinished)
        {
            return;
        }
        pressDelayTimer.Start();
        switch (index)
        {
            case 0:
                var a0 = scenarios[currentSenario].menuConfig[currentStep].actions[0];
                GetCurrentAction(a0).Invoke();
                break;
            case 1:
                var a1 = scenarios[currentSenario].menuConfig[currentStep].actions[1];
                GetCurrentAction(a1).Invoke();
                break;
            case 2:
                var a2 = scenarios[currentSenario].menuConfig[currentStep].actions[2];
                GetCurrentAction(a2).Invoke();
                break;
            case 3:
                var a3 = scenarios[currentSenario].menuConfig[currentStep].actions[3];
                GetCurrentAction(a3).Invoke();
                break;
        }
    }

    private Action GetCurrentAction(MenuActions menuAction)
    {
        return menuAction switch
        {
            MenuActions.Next => Next,
            MenuActions.Back => Back,
            MenuActions.Restart => RestartScenario,
            MenuActions.Basic => ()=>ChooseScenario(MenuActions.Basic),
            MenuActions.Advanced => ()=>ChooseScenario(MenuActions.Advanced), // COURSE3
            MenuActions.LiveExp => ()=>OnStartFinalSequence?.Invoke(),
            MenuActions.BackToMenu => BackToMenu,
            _ => null
        };
    }
    

    private void ChooseScenario(MenuActions action)
    {
        switch (action)
        {
            case MenuActions.Basic:
                currentSenario = 1;
                currentStep = 0;
                ShowStep();
                break;
            case MenuActions.Advanced: //COURSE_3
                currentSenario = 3;
                currentStep = 0;
                AdvancedUtilities.SetActive(true);
                ShowStep();
                return;
        }
    }
    
    
    public override async void SendShowStepRequest(StepName stepName)
    {
        cancelled = inProgress;
        inProgress = true;
        var priority = scenarios[currentSenario].menuConfig[currentStep].scanPriority;
        if (priority==null || priority.Length==0)
        {
            //Show all the messages together
            foreach (var messageSystem in messagesSystems)
            {
                messageSystem.ShowStep(stepName);
            }
            inProgress=false;
            return;
        }
        
        var sortedMessageSystem = messagesSystems.OrderBy(t => Array.IndexOf(priority, t.scanTargetType)).ToList();
        
        foreach (var messageSystem in sortedMessageSystem)
        {
            if (cancelled) return;
            await messageSystem.ShowStep(stepName);
            await Task.Delay((int)scenarios[currentSenario].menuConfig[currentStep].waitTimeBetweenPriorities*1000);
        }
        
        OnStepCompleted?.Invoke(stepName);
        inProgress=false;
        if(stepName == StepName.SightModeSelectionToObserver || stepName == StepName.SightModeSelectionToEnslave || stepName == StepName.SightModeSelectionToShooter 
           || stepName == StepName.SensorToggleToIR || stepName == StepName.SensorToggleToCDD || stepName == StepName.PolarityToggleToBlackHot || stepName == StepName.PolarityToggleToWhiteHot)
        {
            Debug.Log($"FINISHED {stepName}");
            Next();
        }
        
    }

    private void ShowStep()
    {
        UpdateUI();
        PlaySound();
        var step = scenarios[currentSenario].menuConfig[currentStep].stepName;
        SendShowStepRequest(step);
        OnStepCompleted?.Invoke(step);
    }
    
    private void HideStep()
    {
        if(currentStep==-1) return;
        var stepName = scenarios[currentSenario].menuConfig[currentStep].stepName;
        foreach (var messageSystem in messagesSystems) //we want to hide all the step together that is why we don't await
        {
            messageSystem.HideStep(stepName);
        }
    
    }

    public override void ResetTarget()
    {
       
    }

    public override void ShowAll()
    {
        foreach (var messageSystem in messagesSystems)
        {
            messageSystem.gameObject.SetActive(true);
        }
    }

    public override void HideAll()
    {
        foreach (var messageSystem in messagesSystems)
        {
            messageSystem.gameObject.SetActive(false);
        }
    }

    private async void ShowAfterScans()
    {
        follow.enabled=true;
        //menuParent.gameObject.SetActive(true);
        currentSenario = 0;
        currentStep = 0;
        ShowStep();

        await Task.Delay(2000);
        follow.enabled=false;;
    }

    /// <summary>
/// Deactivate the the scan target after we scanned it
/// </summary>
/// <param name="scanTargetType"></param>
/// <returns>If all of the scans are found. relevant when one controller controls more than one target scan</returns>
    public override bool DeActivateScanTarget(ScanTargetType scanTargetType)
    {
        scanStatus[(int)scanTargetType]=true;
        if (AreAllScansFound())
        {
            ShowAfterScans();
            return true;
        }

        return false;
    }

    private bool AreAllScansFound()
    {
        return scanStatus.All(st => st);
    }

    public async void ToggleMenu()
    {
        timer.Start();
        var show = !menuParent.gameObject.activeInHierarchy;
        if (show)
        {
            menuParent.gameObject.SetActive(true);
        }
        var start = show ? Vector3.one*0.01f : Vector3.one;
        var target = show ? Vector3.one : Vector3.one*0.01f;
        
        while (!timer.IsFinished)
        {
            menuParent.localScale = Vector3.Lerp(start, target, timer.GetNormalizeTime());
            await Task.Yield();
        }

        menuParent.localScale = target;
        
        if (!show)
        {
            menuParent.gameObject.SetActive(false);
        }
    }

}
