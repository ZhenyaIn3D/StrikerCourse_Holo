using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLogDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private int maxLines = 20;
    
    private Queue<string> logLines = new Queue<string>();

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Optional: Add log type prefix (Error, Warning, etc.)
        string prefix = type switch
        {
            LogType.Error => "[ERROR] ",
            LogType.Warning => "[WARNING] ",
            LogType.Exception => "[EXCEPTION] ",
            _ => ""
        };

        string message = prefix + logString;

        // Add to queue and maintain max line limit
        logLines.Enqueue(message);
        if (logLines.Count > maxLines)
        {
            logLines.Dequeue();
        }

        // Update TextMeshPro
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        debugText.text = string.Join("\n", logLines);
    }

    // Optional: Clear the debug display
    public void ClearDebugDisplay()
    {
        logLines.Clear();
        debugText.text = "";
    }
}