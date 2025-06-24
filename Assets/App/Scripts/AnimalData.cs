using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData", menuName = "MRAnimal/AnimalData")]
public class AnimalData : ScriptableObject
{
    public string animalName;
    public string thaiName;
    public string scientificName;
    public string family;
    public ConservationStatus conservationStatus;

    public GameObject animalPrefab;

    public AnimationClip idleAnimation;
    public List<AnimationClip> animationList;

    public AudioClip animalSound;
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