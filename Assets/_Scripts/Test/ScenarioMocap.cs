using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioMocap : MonoBehaviour
{
    [SerializeField] private ScenarioManager scenarioManager;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            scenarioManager.PressButton(0);
        }
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            
            scenarioManager.PressButton(1);
        }
    }
}
