using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryPopUpWindow : MonoBehaviour
{    
    private LevelRules levelRules; //Holds the levelRules of the scene, used to communicate with it
    private TextMeshProUGUI scoreText; //Holds the scoreText on the VictoryScreen, used to communicate with it
    
    private void Awake()
    {
        levelRules = GameObject.Find("/LevelRules").GetComponent<LevelRules>();
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
    }
    
    public void TriggerVictoryScreen(int score)
    {
        gameObject.SetActive(true);
        scoreText.SetText("Score: " + score);
    }
}
