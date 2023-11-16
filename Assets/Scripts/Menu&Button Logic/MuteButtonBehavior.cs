using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MuteButtonBehavior : MonoBehaviour
{
    [SerializeField] private String textWhileUnmuted = "Mute";
    [SerializeField] private String textWhileMuted = "Unmute";
    private float prevAudioVolume = 1;
    public void Start()
    {
        if (AudioListener.volume != 0)
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = textWhileUnmuted;
        }
        else
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = textWhileMuted;
        }
    }

    public void SwitchText()
    {
        if(gameObject.GetComponentInChildren<TextMeshProUGUI>().text.Equals(textWhileUnmuted))
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = textWhileMuted;
        }
        else
        {
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = textWhileUnmuted;
        }
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
