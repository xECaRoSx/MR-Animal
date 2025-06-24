using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    TitleScreenState,
    AnchoringState,
    AnimalSelectionState,
    AnimalInfoState,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    [Header("Test Settings")]
    [SerializeField] private bool skipAnchoring = false; // For testing purposes

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (!skipAnchoring)
            SetState(GameState.TitleScreenState);
        else
            SetState(GameState.AnimalSelectionState);
    }

    // =================== Button Functions: State Change ===================
    public void StartGame()
    {
        if (skipAnchoring)
        {
            SetState(GameState.AnimalSelectionState);
            Debug.Log("Skipping Anchoring, starting game directly in Animal Selection");
        }
        else
        {
            SetState(GameState.AnchoringState);
            Debug.Log("Starting...");
        }
    }
    public void ConfirmAnchor()
    {
        SetState(GameState.AnimalSelectionState);
        Debug.Log("Anchor Confirmed");
    }
    public void ReturnToSelection()
    {
        SetState(GameState.AnimalSelectionState);
        Debug.Log("Returning to Animal Selection");
    }
    public void ReturnToTitle()
    {
        SetState(GameState.TitleScreenState);
        Debug.Log("Returning to Title Screen");
    }
    public void QuitGame()
    {
        Debug.Log("Game Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID || UNITY_STANDALONE
    Application.Quit();
#endif
    }
    // ======================================================================

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"[GameManager] GameState changed to: {newState}");

        switch (newState)
        {
            case GameState.TitleScreenState:
                UIManager.Instance.ShowTitleScreen();
                AnimalManager.Instance.HideAllAnimals();
                break;
            case GameState.AnchoringState:
                UIManager.Instance.ShowAnchoringScreen();
                break;
            case GameState.AnimalSelectionState:
                UIManager.Instance.ShowSelectionScreen();
                AnimalManager.Instance.ShowAllAnimals();
                break;
            case GameState.AnimalInfoState:
                UIManager.Instance.ShowInformationScreen();
                break;
            default:
                Debug.LogWarning("Unhandled game state: " + newState);
                break;
        }
    }
}
