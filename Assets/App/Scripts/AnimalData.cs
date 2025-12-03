using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "MRAnimal/AnimalData")]
public class AnimalData : ScriptableObject
{
    [Header("Infomations")]
    public string animalName;
    public string thaiName;
    public string scientificName;
    public string family;
    public ConservationStatus conservationStatus;

    [Header("Prefab & Animaions")]
    public AnimalController animalPrefab;

    public AnimationClip idleAnimation;
    public List<AnimationClip> animationList;

    [Header("Audio")]
    public AudioClip animalSound;
    public AudioClip animalInfoVO;

    [Tooltip("Tick if this animal must always spawn (used when useRandomAnimals = false or as 'fixed' seeds').")]
    public bool alwaysSpawn = false;
}
public enum ConservationStatus
{
    NotAvailableNA,
    ExtinctEX,
    ExtinctInTheWildEW,
    CriticallyEndangeredCR,
    EndangeredEN,
    VulnerableVU,
    NearThreatenedNT,
    LeastConcernLC
}