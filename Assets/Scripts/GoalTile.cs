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

    public void SubtractGoalNumberTextByOne()
    {
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text =
            (int.Parse(gameObject.GetComponentInChildren<TextMeshProUGUI>().text) - 1).ToString();
    }
}
