using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    TitleScreenState,
    AnchoringState,
    AnimalSelectionState,
    AnimalInfoState,
    ResultState
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    public EnableSeeThrough enableSeeThrough;
    public GameObject anchorRoot;

    [Header("Game Settings")]
    public bool useRandomAnimals = true;
    private bool hasPlayedSelectionVO = false;

    [Header("Time Settings")]
    public float playTime = 120f;

    private int score = 0;
    private int maxAnimals = 0;
    private float timer = 0f;
    private bool timerRunning = false;
    private HashSet<AnimalController> foundAnimals = new HashSet<AnimalController>();


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
        SetState(GameState.TitleScreenState);
    }

    private void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            UIManager.Instance.UpdateTimer(timer);
            if (timer <= 0f)
            {
                timer = 0;
                timerRunning = false;
                EndGame();
            }
        }
    }
    // =================== Button Functions: State Change ===================
    public void StartGame()
    {
        SetState(GameState.AnchoringState);
        Debug.Log("[GameManager] StartGame pressed -> Entering AnchoringState");
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
    public void EndGame()
    {
        Debug.Log("[GameManager] Time Out -> ResultState");
        UIManager.Instance.UpdateResult(score, maxAnimals);
        SetState(GameState.ResultState);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                UIManager.Instance.ShowTitleScreen(); //UIManager.Instance.statusUI.SetActive(false);
                AnimalManager.Instance.HideAllAnimals();
                anchorRoot.SetActive(false);
                break;

            case GameState.AnchoringState:
                UIManager.Instance.ShowAnchoringScreen(); //UIManager.Instance.statusUI.SetActive(false);
                AudioManager.Instance.PlayVObyID("VO1");
                AnchorManager.Instance.EnablePreview(true);
                enableSeeThrough.SeeThroughOn();
                break;

            case GameState.AnimalSelectionState:
                anchorRoot.SetActive(true);
                AnimalManager.Instance.SpawnAnimals();
                UIManager.Instance.ShowSelectionScreen();
                UIManager.Instance.statusUI.SetActive(true);
                VFXManager.Instance.StopAllVFX();

                if (!hasPlayedSelectionVO)
                {
                    AudioManager.Instance.PlayVObyID("VO2");
                    hasPlayedSelectionVO = true;
                    Debug.Log("[GameManager] Playing first-time Selection VO");

                    timer = playTime;
                    timerRunning = true;
                }
                break;

            case GameState.AnimalInfoState:
                UIManager.Instance.ShowInformationScreen();
                UIManager.Instance.statusUI.SetActive(true);
                VFXManager.Instance.PlayVFX(VFXTriggerType.OnEnterInfoState);
                break;

            case GameState.ResultState:
                UIManager.Instance.ShowResultScreen(); //UIManager.Instance.statusUI.SetActive(false);
                break;

            default:
                Debug.LogWarning("Unhandled game state: " + newState);
                break;
        }
    }
    // ======================================================================
    public void SetMaxAnimals(int count)
    {
        maxAnimals = count;
        UIManager.Instance.UpdateScore(score, maxAnimals);
        Debug.Log($"[GameManager] maxAnimals set to {maxAnimals}");
    }

    public void OnAnimalFound(AnimalController animal)
    {
        if (!foundAnimals.Contains(animal))
        {
            foundAnimals.Add(animal);
            score++;
            UIManager.Instance.UpdateScore(score, maxAnimals);
        }
    }
}