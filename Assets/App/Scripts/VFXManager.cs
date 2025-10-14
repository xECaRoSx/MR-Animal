using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public enum VFXTriggerType
{
    OnEnterInfoState,
    OnPlayAnimation
}

[System.Serializable]
public class VFXEntry
{
    public string vfxName;
    public GameObject vfxObject;
    public VFXTriggerType trigger;
    public bool autoDisable = true;
}

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("List of VFX in Scene")]
    public List<VFXEntry> vfxList = new List<VFXEntry>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    /// <summary>
    /// Play all effects that are triggered by the specified type.
    /// </summary>
    public void PlayVFX(VFXTriggerType triggerType)
    {
        foreach (var entry in vfxList)
        {
            if (entry.trigger == triggerType)
                ActivateVFX(entry);
                Debug.Log($"[VFXManager] Play: {entry}");
        }
    }

    /// <summary>
    /// Play a specific effect by its name.
    /// </summary>
    public void PlayVFXByName(string name)
    {
        var entry = vfxList.Find(v => v.vfxName == name);
        if (entry != null)
            ActivateVFX(entry);
        else
            Debug.LogWarning($"[VFXManager] No VFX found with name: {name}");
    }

    /// <summary>
    /// Activate a specific VFX entry (enable GameObject and play particle system).
    /// </summary>
    private void ActivateVFX(VFXEntry entry)
    {
        if (entry.vfxObject == null)
        {
            Debug.LogWarning($"[VFXManager] Missing GameObject for VFX: {entry.vfxName}");
            return;
        }

        // Reset the particle system to ensure consistent playback
        var particle = entry.vfxObject.GetComponent<ParticleSystem>();
        if (particle != null)
        {
            entry.vfxObject.SetActive(true); 
            particle.Clear();
            particle.Play();
        }
        else
        {
            // For non-particle VFX objects, toggle active state
            entry.vfxObject.SetActive(false);
            entry.vfxObject.SetActive(true);
        }

        // Automatically disable the object after it finishes playing (for one-shot effects)
        if (entry.autoDisable && particle != null && !particle.main.loop)
        {
            Instance.StartCoroutine(DisableAfterSeconds(entry.vfxObject, particle.main.duration));
        }
    }

    /// <summary>
    /// Stop all currently active VFX (used when leaving InfoState).
    /// </summary>
    public void StopAllVFX()
    {
        foreach (var entry in vfxList)
        {
            if (entry.vfxObject != null && entry.vfxObject.activeSelf)
            {
                var particle = entry.vfxObject.GetComponent<ParticleSystem>();
                if (particle != null)
                    particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

                entry.vfxObject.SetActive(false);
            }
        }
        Debug.Log("[VFXManager] All VFX stopped.");
    }

    /// <summary>
    /// Coroutine to disable the GameObject after a certain duration.
    /// </summary>
    private System.Collections.IEnumerator DisableAfterSeconds(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            obj.SetActive(false);
    }
}