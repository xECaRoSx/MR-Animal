using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class VoiceOverData
{
    public string clipID;
    public AudioClip clip;
}

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

    public List<VoiceOverData> voiceOvers;
    public Dictionary<string, AudioClip> voDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        voDict = new Dictionary<string, AudioClip>();
        foreach (var vo in voiceOvers)
        {
            if (!voDict.ContainsKey(vo.clipID))
                voDict.Add(vo.clipID, vo.clip);
            else
                Debug.LogWarning($"Duplicate VO clipID found: {vo.clipID}");
        }
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

    public void PlayVObyID(string clipID)
    {
        if (string.IsNullOrEmpty(clipID))
        {
            Debug.LogWarning("[AudioManager] PlayVO called with null or empty clipID.");
            return;
        }

        if (!voDict.TryGetValue(clipID, out AudioClip clip))
        {
            Debug.LogWarning($"[AudioManager] VO clipID '{clipID}' not found in dictionary.");
            return;
        }

        // Stop previous VO if playing
        if (VOSource.isPlaying)
            VOSource.Stop();

        VOSource.clip = clip;
        VOSource.Play();
    }

    public void PlayVObyClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] PlayVO called with null AudioClip.");
            return;
        }

        // Stop previous VO if playing
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