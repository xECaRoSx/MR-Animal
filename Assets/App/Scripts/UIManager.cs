using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Canvas References")]
    public GameObject titleUI;
    public GameObject anchoringUI;
    public GameObject selectionUI;
    public GameObject informationUI;

    [Header("Animal Selection Panel")]
    public GameObject animalTooltip; // Tooltip for animal name
    public TextMeshProUGUI tooltipText; // Tooltip text for animal name

    [Header("Animal Information Panel")]
    public TextMeshProUGUI animalNameText;
    public TextMeshProUGUI scientificNameText;
    public TextMeshProUGUI familyText;
    public GameObject[] conservationIcons; // Conservation status icons
    [Range(20, 40)]
    public int maxNameLength = 31; // Max length animal name -> New line if too long

    private AnimalController currentAnimal;
    
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
        HideAllScreens(); // Hide all screens at the start
        ShowTitleScreen(); // Show the title screen
    }

    // ======================== UI Screen Management ========================
    public void ShowTitleScreen()       => ActivateOnly(titleUI);
    public void ShowAnchoringScreen()   => ActivateOnly(anchoringUI);
    public void ShowSelectionScreen()   => ActivateOnly(selectionUI);
    public void ShowInformationScreen() => ActivateOnly(informationUI);

    private void ActivateOnly(GameObject screen)
    {
        HideAllScreens(); // Hide all other screens
        if (screen != null) screen.SetActive(true); // Activate the specified screen
    }

    private void HideAllScreens()
    {
        titleUI.SetActive(false);
        anchoringUI.SetActive(false);
        selectionUI.SetActive(false);
        informationUI.SetActive(false);
    }
    
    // ==================== Button Functions: UI Actions ====================
    public void AnimationButton(int actionIndex)
    {
        if (currentAnimal != null) currentAnimal.PlayAnimation(actionIndex);
    }

    public void ReturnButton()
    {
        if (currentAnimal != null)
        {
            Debug.Log($"Current Animal: {currentAnimal}");
            currentAnimal.OnDeselect(); // Deselect the current animal
            currentAnimal = null;
        }
    }
    // =========================== Animal Tooltip ===========================
    public void ShowAnimalTooltip(AnimalData data, AnimalController animal)
    {
        currentAnimal = animal;
        Debug.Log($"Current Animal: {currentAnimal}");
        animalTooltip.SetActive(true); // Show tooltip

        if ((data.thaiName.Length + data.animalName.Length) <= maxNameLength)
            tooltipText.text = $"{data.thaiName} ({data.animalName})";
        else
            tooltipText.text = $"{data.thaiName}\n({data.animalName})";
    }

    // ========================= Animal Information =========================
    public void ShowAnimalInfo(AnimalData data, AnimalController animal)
    {
        currentAnimal = animal;
        animalTooltip.SetActive(false); // Hide tooltip

        if ((data.thaiName.Length + data.animalName.Length) <= maxNameLength)
            animalNameText.text = $"{data.thaiName} ({data.animalName})";
        else
            animalNameText.text = $"{data.thaiName}\n({data.animalName})";

        scientificNameText.text = $"<i>{data.scientificName}</i>";
        familyText.text = data.family;

        // ================== Conservation Icons Management =================
        int statusIndex = (int)data.conservationStatus;
        bool isNA = statusIndex == 0; // Check if status is Not Available (NA)

        for (int i = 0; i < conservationIcons.Length; i++)
        {
            conservationIcons[i].SetActive(isNA ? i == 0 : i != 0); // Show only NA if status is NA, otherwise show all except NA
            conservationIcons[i].transform.localScale = Vector3.one; // Reset all icons scale to 1*1*1
        }

        if (!isNA && statusIndex < conservationIcons.Length) // Scale up the matching icon
            conservationIcons[statusIndex].transform.localScale = Vector3.one * 1.5f;
        else if (!isNA)
            Debug.LogWarning($"[UIManager] Unknown conservation status: {data.conservationStatus}");
    }
    // ======================================================================
}
