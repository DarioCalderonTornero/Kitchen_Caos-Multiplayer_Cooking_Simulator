using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance {  get; private set; }  
    float volume = 0.5f;
    private AudioSource audioSource;

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";


    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();  
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
        audioSource.volume = volume;

    }
    public void ChangeVolume()
    {
        volume += .1f;
        if (volume > 1.0f)
        {
            volume = 0f;
        }
        audioSource.volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
