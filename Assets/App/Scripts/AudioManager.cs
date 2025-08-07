using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    public AudioSource BGMSource;

    [Header("Sound Effects")]
    public AudioSource SFXSource;

    [Header("Ambient Sound")]
    public AudioSource ambientSource;

    [Header("Voice Over")]
    public AudioSource VOSource;

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
        PlayBGM_Ambient();
    }

    public void PlayBGM_Ambient()
    {
        BGMSource.Play();
        ambientSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null) SFXSource.PlayOneShot(clip);
        else return;
    }

    public void PlayVO(AudioClip clip)
    {
        if (VOSource.isPlaying)
            VOSource.Stop();

        VOSource.clip = clip;
        VOSource.Play();
    }

    public void StopVO()
    {
        if (VOSource.isPlaying)
            VOSource.Stop();
    }
}
