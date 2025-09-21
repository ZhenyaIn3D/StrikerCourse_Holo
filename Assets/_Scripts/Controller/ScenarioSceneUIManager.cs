using System;
using UnityEngine;

public class ScenarioSceneUIManager : MonoBehaviour
{
    [SerializeField] private ScansManager scanManager;
    [SerializeField] private ScenarioManager scenarioManager;

    private void OnEnable()
    {
        scanManager.OnAllScanFound += Toggle;
    }


    private void OnDisable()
    {
        scanManager.OnAllScanFound -= Toggle;
    }


    /// <summary>
    /// Controls the toggle between the scan menu and the message menu
    /// </summary>

    public void Toggle()
    {
        var scenarioMenu = scenarioManager.transform.GetChild(0);
        var scanMenu = scanManager.transform.GetChild(0);
        if (scenarioMenu.gameObject.activeInHierarchy)
        {
            scanMenu.position = scenarioMenu.position;
            scanMenu.rotation = scenarioMenu.rotation;
        }
        else
        {
            scenarioMenu.position = scanMenu.position;
            scenarioMenu.rotation = scanMenu.rotation;
        }
        scenarioManager.ToggleMenu();
        scanManager.ToggleMenu();
    }
 
            
}
