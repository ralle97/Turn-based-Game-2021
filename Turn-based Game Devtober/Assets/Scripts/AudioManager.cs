﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    //public AudioMixerGroup audioMixerGroup;

    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0f;

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;

    public void SetSource(AudioSource src)
    {
        source = src;
        source.clip = clip;
        source.loop = loop;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void Pause()
    {
        source.Pause();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    #region Singleton

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);

            for (int i = 0; i < sounds.Length; i++)
            {
                GameObject go = new GameObject("Sound_" + i + "_" + sounds[i].name);
                go.transform.SetParent(this.transform);
                sounds[i].SetSource(go.AddComponent<AudioSource>());
                //sounds[i].source.outputAudioMixerGroup = sounds[i].audioMixerGroup;
            }
        }
    }

    #endregion

    [SerializeField]
    private Sound[] sounds;

    public void PlaySound(string soundName)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name.Equals(soundName))
            {
                sound.Play();
                return;
            }
        }

        Debug.LogWarning("AudioManager: Sound not found in list, " + soundName);
    }

    public void StopSound(string soundName)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name.Equals(soundName))
            {
                sound.Stop();
                return;
            }
        }

        Debug.LogWarning("AudioManager: Sound not found in list, " + soundName);
    }

    public void PauseSound(string soundName)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name.Equals(soundName))
            {
                sound.Pause();
                return;
            }
        }

        Debug.LogWarning("AudioManager: Sound not found in list, " + soundName);
    }
}
