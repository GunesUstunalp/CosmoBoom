using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MuteButtonBehavior : MonoBehaviour
{
    [SerializeField] private String textWhileUnmuted = "Mute";
    [SerializeField] private String textWhileMuted = "Unmute";
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
}
