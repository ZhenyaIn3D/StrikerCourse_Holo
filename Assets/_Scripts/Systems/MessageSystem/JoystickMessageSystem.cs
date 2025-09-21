using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickMessageSystem : MessageSystem
{
    
    protected override string GetText(StepName stepName)
    {
        return stepName switch
        {
            StepName.Joystic_OperatorConsoleLRU => "Operator Console LRU's",
            StepName.Joystic_VCHLRU => "VCH LRU ",
            StepName.Joystic_VCHHandleGrip => "Vehicle Commander Handle Grip",
            StepName.Joystic_VehicleCommanderControlPanelHigh => "Vehicle Commander Control Panel (High)",
            StepName.Joystic_VehicleCommanderControlPanelLow => "Vehicle Commander Control Panel (Low)",
        };
    }
}
