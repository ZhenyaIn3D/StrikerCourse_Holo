using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using _Scripts.Utils;
using UnityEngine.XR.MagicLeap;
using VCHStateMachine;

public class Step : MonoBehaviour
{
    private const int NARRATION_PAUSE_MS = 500;
    private const int WRONG_ANSWER_DISPLAY_MS = 6000;
    private const int CORRECT_ANSWER_DISPLAY_MS = 3000;
    
    [SerializeField] private UIPoint[] messages;
    [FormerlySerializedAs("infoStepName")] public StepName stepName;
    
    [SerializeField] private bool displayGradually;
    [SerializeField] private bool displayInteractively;
    [SerializeField] private bool hasVersions; //@TODO??
    private bool cancelled;

    public static bool IsTestDone = false;
    private VCHStateMachineController controller;

    private Dictionary<StepName, Func<bool>> endConditions;
    private Dictionary<StepName,  Func<VCHControlState>> relevantStateGetters; //Func<VCHControlState>
    [SerializeField] private AudioClip[] narrationVersions;//@TODO??
    [SerializeField] private string[] instructionsVersions;//@TODO??
    Dictionary<VCHControlState, int> stateVersions;

    [SerializeField] private GameObject wrongAnswerIndication;
    [SerializeField] private GameObject correctAnswerIndication;
    [SerializeField] private GameObject correctionTip;
    public void Toggle()
    {
        foreach (var message in messages)
        {
            message.Toggle();
        }
    }

    public async Task Show()
    {

        controller = VCHStateMachineController.Instance;// IF DONE ON START - DOESN`T HAPPEN(INTERPRETER IS DEACTIVATED)
        
        cancelled = false;
        
        // if (hasVersions)
        // {
        //     
        // }
        
        if (displayGradually) // GRADUAL DISPLAY of messages
        {
            await ShowGradually();
        }
        else if (!displayInteractively) // SIMULTANEOUS DISPLAY of messages
        {
            Debug.Log(stepName);
            await ShowSimultaniously();
        }
        else // DISPLAY messages and wait for state change
        {
            await ShowInteractively();
        }
        
        await Task.Delay(100); // was 500
    }

    public void ShowAgain()
    {
        
    }
    public void Hide()
    {
        cancelled = true;
        foreach (var message in messages)
        {
            message.HideAnimateTransition();
        }

    }
    private void InitializeIndicators()
    {
        SetIndicatorState(wrongAnswerIndication, false);
        SetIndicatorState(correctAnswerIndication, false);
    }
    
    #region Different ways to show
    private async Task ShowGradually()
    { 
        Debug.Log("ShowGradually");
        foreach (var message in messages)
        {
            if (cancelled) return;
            await message.ShowAnimateTransition();

            if (message.UseNarration)
            {
                message.ToggleBackgroundHighlight(true);
                var delayTime = (int)Mathf.Ceil(message.Narration());
                for (int i = 0; i < delayTime; i++)
                {
                    if (cancelled) return;
                    await Task.Delay(1000);
                }
                message.ToggleBackgroundHighlight(false);
            }
            else
            {
                await Task.Delay(500);
            }
        }
    }

    private async Task ShowSimultaniously()
    {
        Debug.Log("ShowSimultaniously");
        foreach (var message in messages)
        {
            message.ShowAnimateTransition(); // no await - everybody starts in one frame and run simult
        }

        foreach (var message in messages)
        {
            if (cancelled) return;
            if (message.UseNarration) // if narration => wait for it ans highlight during
            {
                message.ToggleBackgroundHighlight(true); // highlight ON
                var delayTime = (int)Mathf.Ceil(message.Narration());
                for (int i = 0; i < delayTime; i++) // wait 1000 ms for each message to finish @TODO - maybe less?! may be fitting?
                {
                    if (cancelled) return;
                    await Task.Delay(1000);
                }
                message.ToggleBackgroundHighlight(false); // highlight OFF
            }
        }
    }
    
    private async Task ShowInteractively()
    {
        InitializeIndicators();
        
        endConditions = controller.endConditions;
        relevantStateGetters = controller.relevantStateGetters;
        
        foreach (var message in messages)
        {
            if (cancelled) return;
        
            // Animation
            await message.ShowAnimateTransition();
        
            //Narration
            if (message.UseNarration)
            {
                await ShowMessageWithNarration(message);
            }
            else
            {
                await Task.Delay(NARRATION_PAUSE_MS);
            }
            
            await WaitForCorrectAnswer();

        }
    }
    #endregion
    
    private async Task ShowMessageWithNarration(UIPoint message)
    {
        message.ToggleBackgroundHighlight(true);
        var delayTime = (int)Mathf.Ceil(message.Narration());
    
        if (!await DelayWithCancellation(delayTime * 1000))
            return;
        
        message.ToggleBackgroundHighlight(false);
    }
    private async Task<bool> DelayWithCancellation(int milliseconds)
    {
        var elapsed = 0;
        while (elapsed < milliseconds)
        {
            if (cancelled) return false;
            await Task.Delay(Math.Min(100, milliseconds - elapsed));
            elapsed += 100;
        }
        return true;
    }
    private async Task WaitForCorrectAnswer()
    {
        SetIndicatorState(wrongAnswerIndication, false);
        SetIndicatorState(correctAnswerIndication, false);
        
        SetIndicatorState(correctionTip, false);
        await TaskEx.WaitForConditionWithFeedback(
            getCurrentRelevantState: () => relevantStateGetters[stepName](),
            condition: () => endConditions[stepName](),
            onCorrect: () => ShowCorrectFeedback(),
            onWrong: () =>  ShowWrongFeedback(),
            checkFrequency: 1000,
            timeout: 30000); // 30 seconds
    }
    
    #region Feedback
    private void SetIndicatorState(GameObject indicator, bool active)
    {
        indicator?.SetActive(active);
    }
    
    private async Task ShowWrongFeedback()
    {
        SetIndicatorState(correctAnswerIndication, false);
        SetIndicatorState(wrongAnswerIndication, true);
        SetIndicatorState(correctionTip, true);
        await Task.Delay(WRONG_ANSWER_DISPLAY_MS);
        Debug.Log("INCORRECT");
    }

    private async Task ShowCorrectFeedback()
    {
        SetIndicatorState(wrongAnswerIndication, false);
        SetIndicatorState(correctionTip, false);
        SetIndicatorState(correctAnswerIndication, true);
        await Task.Delay(CORRECT_ANSWER_DISPLAY_MS);
        Debug.Log("CORRECT");
    }
    
    
    #endregion

}

