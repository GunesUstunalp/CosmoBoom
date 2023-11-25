using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Korkuncluk : MonoBehaviour
{
    private void Awake()
    {
        var goalPanel = GameObject.Find("/ScreenCanvas/TopUICanvas/GoalPanel");
        goalPanel.SetActive(false);
    }
}
