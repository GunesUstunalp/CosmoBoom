using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    private float prevAudioVolume = 1;
    void Start()
    {
        //TODO: Get saved audio settings
    }

    
    public void SwitchMute()
    {
        if (AudioListener.volume == 0)
            AudioListener.volume = prevAudioVolume;
        else
        {
            prevAudioVolume = AudioListener.volume;
            AudioListener.volume = 0;
        }
    }
}
