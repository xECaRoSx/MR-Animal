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

    public EnableSeeThrough enableSeeThrough;

    public GameObject anchorRoot;

    [Header("Test Settings")]
    [SerializeField] private bool skipAnchoring = false; // For testing purposes

    private bool hasPlayedSelectionVO = false;

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
            Debug.Log("[GameManager] Skipping Anchoring -> Starting at AnimalSelectionState");
        }
        else
        {
            SetState(GameState.AnchoringState);
            Debug.Log("[GameManager] StartGame pressed -> Entering AnchoringState");
        }
    }

    public void ConfirmButton()
    {
        AnchorManager.Instance.ConfirmAnchor();
        SetState(GameState.AnimalSelectionState);
        Debug.Log("[GameManager] Anchor confirmed -> Entering AnimalSelectionState");
    }
    public void ReturnToSelection()
    {
        SetState(GameState.AnimalSelectionState);
        Debug.Log("[GameManager] Returning to AnimalSelectionState");
    }
    public void ReturnToTitle()
    {
        SetState(GameState.TitleScreenState);
        Debug.Log("[GameManager] Returning to TitleScreenState");
    }
    public void QuitGame()
    {
        Debug.Log("[GameManager] QuitGame called");
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
                anchorRoot.SetActive(false);
                break;

            case GameState.AnchoringState:
                UIManager.Instance.ShowAnchoringScreen();
                AudioManager.Instance.PlayVObyID("VO1");
                AnchorManager.Instance.EnablePreview(true);
                enableSeeThrough.SeeThroughOn();
                break;

            case GameState.AnimalSelectionState:
                UIManager.Instance.ShowSelectionScreen();
                AnimalManager.Instance.ShowAllAnimals();
                VFXManager.Instance.StopAllVFX();

                if (!hasPlayedSelectionVO)
                {
                    AudioManager.Instance.PlayVObyID("VO2");
                    hasPlayedSelectionVO = true;
                    Debug.Log("[GameManager] Playing first-time Selection VO");
                }
                break;

            case GameState.AnimalInfoState:
                UIManager.Instance.ShowInformationScreen();
                VFXManager.Instance.PlayVFX(VFXTriggerType.OnEnterInfoState);
                break;
            default:
                Debug.LogWarning("Unhandled game state: " + newState);
                break;
        }
    }
}