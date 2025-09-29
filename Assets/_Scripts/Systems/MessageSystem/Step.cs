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
    [SerializeField] private UIPoint[] messages;
    [FormerlySerializedAs("infoStepName")] public StepName stepName;
    
    [SerializeField] private bool displayGradually;
    [SerializeField] private bool displayInteractively;
    [SerializeField] private bool hasVersions; //@TODO??
    private bool cancelled;

    public static bool IsTestDone = false;
    private VCHStateMachineController controller;

    private Dictionary<StepName, Func<bool>> endConditions;
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
        
        if (hasVersions)
        {
            
        }
        
        
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

    public void Hide()
    {
        cancelled = true;
        foreach (var message in messages)
        {
            message.HideAnimateTransition();
        }

    }

    /// <summary>
    /// Ways of showing step content
    /// </summary>
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
        wrongAnswerIndication?.SetActive(false);
        correctAnswerIndication?.SetActive(false);
        
        endConditions = controller.endConditions;
        
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
            } // showed & said everything
            else
            {
                await Task.Delay(500);
            }

            while (!endConditions[stepName]())
            {
                wrongAnswerIndication.SetActive(false);
                await TaskEx.WaitUntil(controller.IsStateChanged, 5, -1);
                if (!endConditions[stepName]())
                {
                    wrongAnswerIndication.SetActive(true);
                    Debug.Log("INCORRECT");
                    correctionTip.SetActive(true);
                    if (cancelled) return;
                    await Task.Delay(6000); // INCORRECT CLIP IS 2.3 SEC
                }
            }
            
            correctionTip.SetActive(false);
            wrongAnswerIndication.SetActive(false);
            correctAnswerIndication.SetActive(true);
            await Task.Delay(1000); // CORRECT CLIP IS 0.9 SEC
            Debug.Log("CORRECT");

        }
        
    }



}

