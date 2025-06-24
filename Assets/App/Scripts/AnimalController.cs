using System;
using System.Collections;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public AnimalData animalData;
    public Animator animator;

    [Header("Transform Settings")]
    [SerializeField] private float scaleFactor = 5.5f; // Scale factor for the animal when selected
    [SerializeField] private float transitionDuration = 0.5f; // Duration for the transition animation
    [SerializeField] private Collider[] animalCollider;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    private AnimatorOverrideController overrideController;
    private Coroutine moveRoutine;
    private bool isSelected = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;

        if (animalData.idleAnimation != null)
        {
            overrideController["Idle"] = animalData.idleAnimation;
            animator.Play("Idle");
            Debug.Log($"[Override] Idle clip set for {animalData.animalName}: {animalData.idleAnimation.name}");
        }
        else
        {
            Debug.LogWarning($"[AnimalController] {animalData.animalName} has no idleClip assigned.");
        }

        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalScale = transform.localScale;
    }

    // ==================== XR Interaction Event Hooks =====================
    public void OnFocus()
    {
        if (GameManager.Instance.CurrentState != GameState.AnimalSelectionState) return; 
        UIManager.Instance.ShowAnimalTooltip(animalData, this);
    }

    public void OnSelected()
    {
        if (isSelected) return;

        Debug.Log($"[Select] {animalData.animalName}");
        isSelected = true;

        GameManager.Instance.SetState(GameState.AnimalInfoState);
        AnimalManager.Instance.ShowOnlySelectedAnimal(this);
        UIManager.Instance.ShowAnimalInfo(animalData, this);
        AudioManager.Instance.PlaySFX(animalData.animalSound);

        Vector3 targetPos = new Vector3(0, originalPosition.y, 0);
        Quaternion targetRot = Quaternion.identity;
        Vector3 targetScale = originalScale * scaleFactor;

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(SmoothTransform(targetPos, targetRot, targetScale));
    }

    public void OnDeselect()
    {
        Debug.Log($"Deselected: {animalData.animalName}");
        isSelected = false;

        GameManager.Instance.SetState(GameState.AnimalSelectionState);
        AnimalManager.Instance.ShowAllAnimals();
        UIManager.Instance.ShowSelectionScreen();

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(SmoothTransform(originalPosition, originalRotation, originalScale));

        StopAnimation();
    }

    // ===================== Animation Management =========================
    public void PlayAnimation(int actionIndex)
    {
        if (animalData.animationList == null || actionIndex < 0 || actionIndex >= animalData.animationList.Count)
        {
            Debug.LogWarning($"[AnimalController] Invalid animation index {actionIndex} for {animalData.animalName}");
            return;
        }

        AnimationClip targetClip = animalData.animationList[actionIndex];
        if (targetClip == null)
        {
            Debug.LogWarning($"[AnimalController] Animation at index {actionIndex} is null for {animalData.animalName}");
            return;
        }

        string stateName = $"Action{actionIndex + 1}";
        overrideController[stateName] = targetClip;
        Debug.Log($"[Override] {stateName} clip set for {animalData.animalName}: {targetClip.name}");
        animator.Play(stateName);
        AudioManager.Instance.PlaySFX(animalData.animalSound);

        Debug.Log($"[AnimalController] Playing {animalData.animalName} : {stateName}");
    }

    public void StopAnimation()
    {
        animator.Play("Idle");
    }

    // =================== Collider Management Helper ======================
    public void SetColliderActive(bool isActive)
    {
        foreach (var col in animalCollider)
        {
            if (col != null) col.enabled = isActive;
        }
    }

    // ======================= Transform Tweening ==========================
    private IEnumerator SmoothTransform(Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale, bool returnToSelection = false)
    {
        float elapsedTime = 0f;

        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);
            transform.localRotation = Quaternion.Slerp(startRot, targetRotation, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        
        transform.localPosition = targetPosition; // Ensure exact value at the end
        transform.localRotation = targetRotation;
        transform.localScale = targetScale;
    }
}
