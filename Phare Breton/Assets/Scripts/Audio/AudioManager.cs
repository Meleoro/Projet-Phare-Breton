using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public List<SoundCategory> soundList;
    public List<AudioSource> audioSources;
    [Range(0,1)]public float masterVolume;
    [SerializeField] private bool mainMenu;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
    }

    private void Start()
    {
        //PlaySoundOneShot(0, 5, 0,AudioSourceSurLeJoueur);
        /*if (mainMenu)
        {
            PlaySoundContinuous(3,7,0,MainMenuAudioSource.instance.audioSource);
        }
        else
        {
            audioSources[0] = PlayerController.instance.selfAudioSource;
            audioSources[1] = PlayerController.instance.musicAudioSource;
            audioSources[2] = PlayerController.instance.secondaryAudioSource;
            PlaySoundContinuous(0,6,2);
        }*/

    }

    public void PlaySoundContinuous(int soundId, int categoryId = 0, int audioSourceId = 0, AudioSource audioSource = null)
    {
        AudioSource currentAudioSource = audioSource != null ? audioSource : audioSources[audioSourceId];
        currentAudioSource.clip = soundList[categoryId].listSoundIdentities[soundId].audioClip;
        currentAudioSource.volume = soundList[categoryId].listSoundIdentities[soundId].volume*masterVolume;
        currentAudioSource.Play();
    }

    public void PlaySoundOneShot(int soundId, int categoryId = 0, int audioSourceId = 0, AudioSource audioSource = null)
    {
        AudioSource currentAudioSource = audioSource != null ? audioSource : audioSources[audioSourceId];
        currentAudioSource.PlayOneShot(soundList[categoryId].listSoundIdentities[soundId].audioClip, soundList[categoryId].listSoundIdentities[soundId].volume*masterVolume);
    }

    public void PlaySoundFadingIn(float timeBeforeFadingIn, float timeToFade, int soundId, int categoryId = 0, int audioSourceId = 0, AudioSource audioSource = null)
    {
        PlaySoundContinuous( soundId,categoryId,audioSourceId,audioSource);
        AudioSource currentAudioSource = audioSource != null ? audioSource : audioSources[audioSourceId];
        float volumeToGet = currentAudioSource.volume;
        currentAudioSource.volume = 0;
        currentAudioSource.DOFade(volumeToGet, timeToFade).SetDelay(timeBeforeFadingIn).SetUpdate(true);
    }

    public void FadeOutAudioSource(float timeBeforeFadingOut, float timeToFade, int audioSourceId = 0, AudioSource audioSource = null)
    {
        AudioSource currentAudioSource = audioSource != null ? audioSource : audioSources[audioSourceId];
        currentAudioSource.DOFade(0, timeToFade).SetDelay(timeBeforeFadingOut).SetUpdate(true);
    }

    public void SetMasterVolume(float newMasterVolume)
    {
        if (masterVolume == 0)
        {
            foreach (var t in audioSources)
            {
                t.volume = newMasterVolume;
            }
        }
        else
        {
            foreach (var t in audioSources)
            {
                t.volume *= newMasterVolume / masterVolume;
            }
        }
        masterVolume = newMasterVolume;
    }
}

[System.Serializable]

public class SoundIdentity
{
    public string name;
    public AudioClip audioClip;
    [Range(0,1)] public float volume = 1;
}


[System.Serializable]

public class SoundCategory
{
    [SerializeField] private string name;
    public List<SoundIdentity> listSoundIdentities;
}
