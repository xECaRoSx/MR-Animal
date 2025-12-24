using UnityEngine;

public class TitleEE : MonoBehaviour
{
    [Header("Tap Settings")]
    [SerializeField] private float tapThreshold = 1.5f;
    [SerializeField] private int tapsToTrigger = 8;

    [Header("Tap SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] tapClips;

    [Header("Visual Gimmick")]
    [SerializeField] private Animator titleAnimator;
    [SerializeField] private string triggerName = "Tilt";

    private int tapCount;
    private float lastTapTime;

    public void RegisterTap()
    {
        if (Time.time - lastTapTime > tapThreshold)
            tapCount = 0;

        tapCount++;
        lastTapTime = Time.time;

        PlayRandomTapSound();

        if (tapCount >= tapsToTrigger)
        {
            TriggerEasterEgg();
            tapCount = 0;
        }
    }

    private void TriggerEasterEgg()
    {
        if (titleAnimator != null)
            titleAnimator.SetTrigger(triggerName);
    }

    private void PlayRandomTapSound()
    {
        if (audioSource == null || tapClips == null || tapClips.Length == 0)
            return;

        int index = Random.Range(0, tapClips.Length);
        audioSource.PlayOneShot(tapClips[index], 0.8f);
    }
}
