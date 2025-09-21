using UnityEngine;

public class ScreenMessageSystem : MessageSystem
{
    private void Awake()
    {
        CurrentStepName = StepName.Screen_Display;
    }
    

    protected override string GetText(StepName stepName)
    {
        return "";
    }
    
}
