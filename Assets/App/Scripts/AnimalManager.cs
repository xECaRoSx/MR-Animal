using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance { get; private set; }

    private AnimalController[] animalLists;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        animalLists = FindObjectsOfType<AnimalController>();
    }

    public void ShowOnlySelectedAnimal(AnimalController selected)
    {
        foreach (var animal in animalLists)
        {
            bool isSelected = animal == selected;
            animal.SetColliderActive(isSelected);
            animal.gameObject.SetActive(isSelected);

        }
    }

    public void ShowAllAnimals()
    {
        foreach (var animal in animalLists)
        {
            animal.SetColliderActive(true);
            animal.gameObject.SetActive(true);
        }
    }

    public void HideAllAnimals()
    {
        foreach (var animal in animalLists)
        {
            animal.SetColliderActive(false);
            animal.gameObject.SetActive(false);
        }
    }
}
