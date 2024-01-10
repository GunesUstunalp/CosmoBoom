using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private GoalTile goalTilePrefab;

    private GoalTile[] goalTiles;
    private int[] xOffsetsForTwoGoals = new[] { -40, 40 };
    private int[] xOffsetsForThreeGoals = new[] { -60, 0, 60 };
    private LevelRules levelRules; //Holds the levelRules of the scene, used to communicate with it
    private VictoryPopUpWindow victoryPopUpWindow; //Holds the victoryPopUpWindow of the scene, used to communicate with it
    private MenuPopUpWindow defeatPopUpWindow; //Holds the defeatPopUpWindow of the scene, used to communicate with it
    private TextMeshProUGUI movesText; //Holds the movesText on the TopUI, used to communicate with it
    private Grid grid; // Holds the grid of the scene, used to communicate with it
    
    void Start()
    {
        grid = GameObject.Find("/ScreenCanvas/GridPanel").GetComponent<Grid>();
        levelRules = GameObject.Find("/LevelRules").GetComponent<LevelRules>();
        victoryPopUpWindow = GameObject.Find("/ScreenCanvas").transform.Find("VictoryPopUpWindow").GetComponent<VictoryPopUpWindow>();
        defeatPopUpWindow = GameObject.Find("/ScreenCanvas").transform.Find("DefeatPopUpWindow").GetComponent<MenuPopUpWindow>();
        movesText = GameObject.Find("MovesText").GetComponent<TextMeshProUGUI>();
        int numberOfGoals = levelRules.goalInputs.Length;
        goalTiles = new GoalTile[numberOfGoals];
        UpdateMovesText();

        for (int i = 0; i < numberOfGoals; i++)
        {
            var spawnedGoalTile = Instantiate(goalTilePrefab, new Vector3(0, 0), Quaternion.identity);
            goalTiles[i] = spawnedGoalTile;
            spawnedGoalTile.goalType = levelRules.goalInputs[i].goalType;
            spawnedGoalTile.goalColorType = levelRules.goalInputs[i].goalColorType;
            spawnedGoalTile.goalNumber = levelRules.goalInputs[i].goalNumber;
            spawnedGoalTile.transform.SetParent(gameObject.transform, true);
            spawnedGoalTile.GetComponent<RectTransform>().localScale = Vector3.one;
            spawnedGoalTile.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            
            spawnedGoalTile.name = $"Goal Tile {i}";

            int xOffset = 0;
            if (numberOfGoals == 2)
                xOffset = xOffsetsForTwoGoals[i];
            else if (numberOfGoals == 3)
                xOffset = xOffsetsForThreeGoals[i];
            
            spawnedGoalTile.GetComponent<RectTransform>().localPosition = new Vector3(xOffset, 0f);

            spawnedGoalTile.GetComponentInChildren<TextMeshProUGUI>().text = levelRules.goalInputs[i].goalNumber.ToString();

            Sprite goalSprite;
            if (spawnedGoalTile.goalType == TileType.ColorTile)
            {
                goalSprite = grid.ColorTilePrefabDict[spawnedGoalTile.goalColorType].GetComponent<Tile>().defaultSprite;
            }
            else
            {
                goalSprite = grid.TilePrefabDict[spawnedGoalTile.goalType].GetComponent<Tile>().defaultSprite;
            }
            
            spawnedGoalTile.GetComponentInChildren<Image>().sprite = goalSprite;
        }
    }

    public GoalTile GetGoalTileOfTile(Tile tile)
    {
        for (int i = 0; i < goalTiles.Length; i++)
        {
            if (goalTiles[i].goalType == tile.tileType &&  goalTiles[i].goalNumber > 0)
            {
                if (tile.tileType == TileType.ColorTile)
                {
                    if (((ColorTile)tile).colorType != goalTiles[i].goalColorType)
                        continue;
                }
                
                goalTiles[i].goalNumber--;
                return goalTiles[i];
            }
        }

        return null;
    }

    public void SubtractGoalNumberByOne(GoalTile goalTile)
    {
        goalTile.SubtractGoalNumberTextByOne();
        CheckForWin();
    }

    public bool CheckForWin()
    {
        foreach (GoalTile goalTile in goalTiles)
        {
            if (goalTile.goalNumber > 0)
                return false;
        } 
        
        victoryPopUpWindow.TriggerVictoryScreen(int.Parse(movesText.text)); 
        return true;
    }

    public void CheckForDefeat()
    {
        if (CheckForWin()) //if the player won with their last move
            return;
        
        defeatPopUpWindow.SwitchActive();
    }
    
    public void UpdateMovesText()
    {
        movesText.SetText(levelRules.moves.ToString());
    }
}
