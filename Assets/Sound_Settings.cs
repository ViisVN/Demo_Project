using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using QFSW.QC.Actions;
using UnityEngine;

public class Sound_Settings : MonoBehaviour
{
    public static Sound_Settings Instance;
   [SerializeField]  AudioSource effectSound, musicSound;
   public AudioClip toggle;
    void Awake()
    {
        if(Instance!=null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void PlayEffectSound(AudioClip clip)
    {
        effectSound.PlayOneShot(clip);
    }

    public void MuteEffectSound()
    {
        effectSound.mute =!effectSound.mute;
    }
    public void MuteMusicSound()
    {
        musicSound.mute = !musicSound.mute;
    }
    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

}
