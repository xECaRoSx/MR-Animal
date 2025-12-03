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
        animator = GetComponentInChildren<Animator>();

        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;

        // Only play idle if exists
        if (animalData.idleAnimation != null)
        {
            overrideController["Idle"] = animalData.idleAnimation;
            animator.Play("Idle");
        }
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
        GameManager.Instance.OnAnimalFound(this);
        AnimalManager.Instance.ShowOnlySelectedAnimal(this);
        UIManager.Instance.ShowAnimalInfo(animalData, this);

        // --- Play SFX if exists ---
        if (animalData.animalSound != null)
            AudioManager.Instance.PlaySFX(animalData.animalSound);

        // --- Play VO if exists ---
        if (animalData.animalInfoVO != null)
            AudioManager.Instance.PlayVObyClip(animalData.animalInfoVO);

        // --- Transform movement ---
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
        UIManager.Instance.ShowSelectionScreen();
        UIManager.Instance.statusUI.SetActive(true);

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(SmoothTransform(originalPosition, originalRotation, originalScale));

        StopAnimation();
    }

    // ===================== Animation Management =========================
    public void PlayAnimation(int actionIndex)
    {
        if (animalData.animationList == null ||
        actionIndex < 0 ||
        actionIndex >= animalData.animationList.Count)
        {
            Debug.LogWarning($"[AnimalController] No valid animation for {animalData.animalName}");
            return;
        }

        AnimationClip targetClip = animalData.animationList[actionIndex];
        if (targetClip == null)
        {
            Debug.LogWarning($"[AnimalController] Animation clip missing for index {actionIndex} on {animalData.animalName}");
            return;
        }

        string stateName = $"Action{actionIndex + 1}";
        overrideController[stateName] = targetClip;

        animator.Play(stateName);

        // Play SFX only if available
        if (animalData.animalSound != null)
            AudioManager.Instance.PlaySFX(animalData.animalSound);

        VFXManager.Instance.PlayVFX(VFXTriggerType.OnPlayAnimation);

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

    public void SetOriginalTransform(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        originalPosition = pos;
        originalRotation = rot;
        originalScale = scale;
    }
}
