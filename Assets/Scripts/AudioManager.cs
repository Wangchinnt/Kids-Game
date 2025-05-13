using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public List<AudioClip> musics;
    [SerializeField]private Dictionary<string, AudioClip> _musicDictionary;

    public List<AudioClip> sounds; 
    [SerializeField]private Dictionary<string, AudioClip> _soundDictionary;
    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {   
            Destroy(gameObject);
            return;
        }

        _audioSource = gameObject.AddComponent<AudioSource>();
        _soundDictionary = new Dictionary<string, AudioClip>();
        _musicDictionary = new Dictionary<string, AudioClip>();
        // add all music to dictionary
        foreach (var clip in musics)
        {
            _musicDictionary[clip.name] = clip;
        }
        foreach (var clip in sounds)
        {
            _soundDictionary[clip.name] = clip;
        }
      
    }

    private void Start()
    {
        // if current scene is Main Menu, play Main Menu Background Music
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenuScene")  
            PlayMusic("Main Menu Background Music");
    }

    public void PlaySound(string soundName)
    {
        if (_soundDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            _audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found!");
        }
    }
    public void PlayMusic(string musicName)
    {
        if (_musicDictionary.TryGetValue(musicName, out AudioClip clip))
        {
            _audioSource.clip = clip;
            _audioSource.loop = true;
            _audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music {musicName} not found!");
        }
    }
    public void StopMusic()
    {
        _audioSource.Stop();
    }
}