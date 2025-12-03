using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance { get; private set; }

    [Header("Spawn Settings")]
    public bool randomAnimals = false;
    public int spawnCount = 9;
    public List<Transform> spawnPoints;

    private AnimalController[] allAnimals;
    private List<AnimalController> animalsToSpawn = new List<AnimalController>();

    private bool hasSpawned = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        allAnimals = FindObjectsOfType<AnimalController>();

        foreach (var a in allAnimals)
            a.gameObject.SetActive(false);
    }

    // =============================================================
    // SPAWN SYSTEM
    // =============================================================
    public void SpawnAnimals()
    {
        if (!hasSpawned)
        {
            animalsToSpawn.Clear();

            foreach (var a in allAnimals)
            {
                if (a.animalData != null && a.animalData.alwaysSpawn)
                    animalsToSpawn.Add(a);
            }

            if (animalsToSpawn.Count > spawnCount)
            {
                Debug.LogError("[AnimalManager] alwaysSpawn is more than spawnCount!");
                return;
            }

            if (randomAnimals)
            {
                List<AnimalController> selectable = new List<AnimalController>();

                foreach (var a in allAnimals)
                {
                    if (!animalsToSpawn.Contains(a))
                        selectable.Add(a);
                }

                int needed = spawnCount - animalsToSpawn.Count;

                if (needed > selectable.Count)
                    needed = selectable.Count;

                for (int i = 0; i < needed; i++)
                {
                    int r = Random.Range(0, selectable.Count);
                    animalsToSpawn.Add(selectable[r]);
                    selectable.RemoveAt(r);
                }
            }

            PlaceAnimalsAtPoints();
            hasSpawned = true;
        }

        foreach (var a in animalsToSpawn)
        {
            a.gameObject.SetActive(true);
            a.SetColliderActive(true);
        }

        foreach (var a in allAnimals)
        {
            if (!animalsToSpawn.Contains(a))
            {
                a.gameObject.SetActive(false);
                a.SetColliderActive(false);
            }
        }
    }

    private void PlaceAnimalsAtPoints()
    {
        List<Transform> freePoints = new List<Transform>(spawnPoints);

        foreach (var animal in animalsToSpawn)
        {
            int id = Random.Range(0, freePoints.Count);
            Transform point = freePoints[id];
            freePoints.RemoveAt(id);

            animal.transform.SetPositionAndRotation(point.position, point.rotation);

            animal.SetOriginalTransform(
                animal.transform.localPosition,
                animal.transform.localRotation,
                animal.transform.localScale
);
        }
    }

    private void SpawnAtPoints()
    {
        foreach (var a in allAnimals)
            a.gameObject.SetActive(false);

        if (spawnCount > spawnPoints.Count)
        {
            Debug.LogError("[AnimalManager] spawnCount is more than spawnCount!");
            return;
        }

        List<Transform> freePoints = new List<Transform>(spawnPoints);

        foreach (var animal in animalsToSpawn)
        {
            int id = Random.Range(0, freePoints.Count);
            Transform point = freePoints[id];
            freePoints.RemoveAt(id);

            animal.transform.SetPositionAndRotation(point.position, point.rotation);
            animal.gameObject.SetActive(true);
            animal.SetColliderActive(true);

            if (animal.animator != null)
            {
                try { animal.animator.SetTrigger("Idle"); }
                catch { }
            }
        }
    }

    public void ShowOnlySelectedAnimal(AnimalController selected)
    {
        foreach (var animal in allAnimals)
        {
            bool isSelected = animal == selected;
            animal.SetColliderActive(isSelected);
            animal.gameObject.SetActive(isSelected);
        }
    }

    public void HideAllAnimals()
    {
        foreach (var animal in allAnimals)
        {
            animal.SetColliderActive(false);
            animal.gameObject.SetActive(false);
        }
    }
}
