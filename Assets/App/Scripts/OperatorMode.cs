using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the hidden Operator Mode, debug log display,
/// and utility functions for developer operations.
/// </summary>

public class OperatorMode : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Image randomToggleImage;

    [Header("Tap Detection Settings")]
    [SerializeField] private float tapThreshold = 1.5f;
    [SerializeField] private int tapsToUnlock = 8;


    private int tapCount = 0;
    private float lastTapTime;

    private readonly Queue<string> logQueue = new Queue<string>();
    private const int MaxLogs = 15;

    private bool randomModeEnabled = false;

    private void Start()
    {
        if (debugPanel != null)
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

    // ---------------------------------------------------------
    //   SECRET TAP UNLOCK
    // ---------------------------------------------------------

    public void RegisterSecretTap()
    {
        if (Time.time - lastTapTime > tapThreshold)
            tapCount = 0;

        tapCount++;
        lastTapTime = Time.time;

        if (tapCount >= tapsToUnlock)
        {
            ActivateOperatorMode();
            tapCount = 0;
        }
    }

    private void ActivateOperatorMode()
    {
        if (debugText != null)
            debugText.text = "<b>[OP]</b> Console logs below\n";

        if (debugPanel != null)
            debugPanel.SetActive(true);

        Debug.Log("[OP] Operator Mode Activated");
    }

    // ---------------------------------------------------------
    //   LOG HANDLING
    // ---------------------------------------------------------

    private void HandleLog(string message, string stackTrace, LogType type)
    {
        string color = type switch
        {
            LogType.Warning => "yellow",
            LogType.Error => "red",
            LogType.Assert => "orange",
            LogType.Exception => "magenta",
            _ => "white"
        };

        string formatted = $"<color={color}>[{type}] {message}</color>";
        logQueue.Enqueue(formatted);

        if (logQueue.Count > MaxLogs)
            logQueue.Dequeue();

        if (debugText != null)
            debugText.text = string.Join("\n", logQueue.ToArray());
    }

    // ---------------------------------------------------------
    //   PUBLIC BUTTON FUNCTIONS
    // ---------------------------------------------------------

    public void CloseOperatorMode()
    {
        if (debugPanel != null)
            debugPanel.SetActive(false);

        logQueue.Clear();

        if (debugText != null)
            debugText.text = string.Empty;

        tapCount = 0;

        Debug.Log("[OP] Operator Mode Closed");
    }

    /// <summary>Clears all debug text from the panel.</summary>
    public void ClearDebugText()
    {
        logQueue.Clear();
        if (debugText != null)
            debugText.text = "<b>[OP]</b> Logs cleared.\n";

        Debug.Log("[OP] Debug logs cleared");
    }

    /// <summary>
    /// Toggles the random animal mode.
    /// (Will connect with AnimalManager later.)
    /// </summary>
    public void ToggleRandomAnimalMode()
    {
        randomModeEnabled = !randomModeEnabled;

        if (randomToggleImage != null)
        {
            if (randomModeEnabled)
            {
                Color onColor;
                ColorUtility.TryParseHtmlString("#6EFF1E", out onColor);
                randomToggleImage.color = onColor;
            }
            else
            {
                randomToggleImage.color = Color.black;
            }
        }

        Debug.Log($"[OP] Random Animal Mode: {(randomModeEnabled ? "ON" : "OFF")}");
    }
}
