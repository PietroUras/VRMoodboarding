using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class InteractionLogger : MonoBehaviour
{
    public static InteractionLogger Instance { get; private set; }

    private string userId;
    private string systemInputMode;
    private List<string> logLines = new();
    private string contextualGestureLogFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetUp(string _contextualGestureFilePath,string _gestureManagerFilePath, string _userId, string _inputMode)
    {
        userId = _userId;
        systemInputMode = _inputMode;
        contextualGestureLogFilePath = _contextualGestureFilePath;

        logLines.Add("Timestamp;UserId;SystemInputMode;ActionPerformed;Trigger;GestureName;SceneName;SourceComponent;Other");

        Application.quitting += FlushLogsToFile;
    }


    /// <param name="actionPerformed">e.g. "create image", "start mic"</param>  
    /// <param name="usedInputMode">InputMode enum value</param>  
    /// <param name="gestureName">e.g. "swipe", "thumbs up", "frame", "start mic"</param>
    /// <param name="sourceComponent">e.g. class or UI object name</param>
    public void LogInteraction(
        string actionPerformed,
        string usedInputMode,
        string sourceScene,
        string sourceComponent,
        string gestureName = "",
        string custom_field ="")
    {
        string timestamp = DateTime.UtcNow.ToString("MM-dd HH:mm:ss.fff");

        string line = $"{timestamp};{userId};{systemInputMode};{actionPerformed};{usedInputMode};{gestureName};{sourceScene};{sourceComponent};{custom_field}";
        logLines.Add(line);
    }

    /// <summary>
    /// Logs an interaction specifically for the Gesture Manager log file.
    /// </summary>
    /// <param name="actionPerformed">e.g. "gesture detected"</param>
    /// <param name="usedInputMode">InputMode enum value</param>
    /// <param name="sourceScene">Scene name</param>
    /// <param name="sourceComponent">UI object or class name</param>
    /// <param name="gestureName">e.g. "swipe", "thumbs up"</param>
    /// <param name="custom_field">Additional custom info</param>
    
    public void FlushLogsToFile()
    {
        File.WriteAllLines(contextualGestureLogFilePath, logLines);
    }

}

