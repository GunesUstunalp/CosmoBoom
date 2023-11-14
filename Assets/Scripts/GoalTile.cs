using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class GoalTile : MonoBehaviour
{
    public static int Width = 50;
    [SerializeField] public TileType goalType;
    [SerializeField] public int goalNumber;
    private TextMeshProUGUI goalNumberText;

    private void Start()
    {
        goalNumberText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SubtractGoalNumberTextByOne()
    {
        goalNumberText.text = (int.Parse(goalNumberText.text) - 1).ToString();
    }
}
