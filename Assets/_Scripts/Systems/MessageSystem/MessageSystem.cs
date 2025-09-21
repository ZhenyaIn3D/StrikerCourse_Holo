using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public abstract class MessageSystem : MonoBehaviour
{
    private Step[] steps;
    private StepName _currentStepName;
    public ScanTargetType scanTargetType;
    public StepName CurrentStepName{get => _currentStepName;
        set => _currentStepName = value;
    }

    public virtual void Start()
    {
        steps = GetComponentsInChildren<Step>();

    }
    

    public async Task ShowStep(StepName stepName)
    {
        var s = steps.FirstOrDefault(s => s.stepName == stepName);
        if (s != null)
        {
            await HideStep(_currentStepName);
            await s.Show();
            _currentStepName = stepName;
            
        }
        
    }
    
    public async Task HideStep(StepName stepName)
    {
        var s = steps.FirstOrDefault(s => s.stepName == stepName);
        if (s != null) s.Hide();
        await Task.Delay(1000);
    }
    
    
    protected abstract string GetText(StepName stepName);
    
    
}

public enum StepName
{
    Joystic_OperatorConsoleLRU,
    Joystic_VCHLRU,
    Joystic_VCHHandleGrip,
    Joystic_VehicleCommanderControlPanelHigh,
    Joystic_VehicleCommanderControlPanelLow,
    Screen_Overview,
    Screen_Display,
    Screen_Information,
    Screen_Videos,
    Intro,
    ScenarioOptions,
    ValidateScenarioOne,
    Joystick_VehicleCommanderHandle,
    FinishFirstScenario,
    Screen_OperatorConsoleLRU,
    ValidateScenarioTwo,
    SightModeSelection,
    SightMovementControl,
    CCDIRSelection,
    ZoomFocusSelection,
    LRFFiring,
    FinishSecondScenario,
    LiveExample,
    //ADVANCED
    ValidateScenarioThree,
    //Mode
    SightModeSelectionAdv,
    SightModeSelectionToObserver,
    SightModeSelectionToEnslave,
    SightModeSelectionToShooter,
    //Sensor
    SightSensorSelectionAdv,
    SensorToggleToIR,
    SensorToggleToCDD,
    //Polarity
    PolaritySelectionAdv,
    PolarityToggleToBlackHot,
    PolarityToggleToWhiteHot,
    //Joystick
    SightMovingControlAdv,
    SightMovingControlToUP,
    SightMovingControlToDOWN,
    SightMovingControlToLEFT,
    SightMovingControlToRIGHT,
    //Zoom/Focus
    //
    FinishAdvancedScenario,

}

