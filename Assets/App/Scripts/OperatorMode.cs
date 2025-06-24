using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OperatorMode : MonoBehaviour
{
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private TextMeshProUGUI debugText;

    private int tapCount = 0;
    private float lastTapTime;
    public float tapThreshold = 1.5f;

    private Queue<string> logQueue = new Queue<string>();
    private const int maxLogs = 15;

    private void Start()
    {
        debugPanel.SetActive(false);
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public void SecretUnlock()
    {
        if (Time.time - lastTapTime > tapThreshold)
            tapCount = 0;

        tapCount++;
        lastTapTime = Time.time;

        if (tapCount >= 8)
        {
            ActivateOPMode();
            tapCount = 0;
        }
    }

    private void ActivateOPMode()
    {
        if (debugText != null)
        {
            debugText.text = "<b>[OP]</b> Console logs below\n";
        }
        debugPanel.SetActive(true);
        Debug.Log("[OP] Operator Mode Activated!");
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = "white";
        switch (type)
        {
            case LogType.Warning: color = "yellow"; break;
            case LogType.Error: color = "red"; break;
            case LogType.Assert: color = "orange"; break;
            case LogType.Exception: color = "magenta"; break;
        }

        string formatted = $"<color={color}>[{type}] {logString}</color>";
        logQueue.Enqueue(formatted);

        if (logQueue.Count > maxLogs)
            logQueue.Dequeue();

        debugText.text = string.Join("\n", logQueue.ToArray());
    }
}
