using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private Sprite yellowCubeSprite;
    [SerializeField] private Sprite redCubeSprite;
    [SerializeField] private Sprite blueCubeSprite;
    [SerializeField] private Sprite greenCubeSprite;
    [SerializeField] private Sprite purpleCubeSprite;
    [SerializeField] private Sprite duckSprite;
    [SerializeField] private Sprite balloonSprite;
    [SerializeField] private Sprite rocketRightSprite;

    [SerializeField] private GoalTile goalTilePrefab;

    private GoalTile[] goalTiles;
    private int[] xOffsetsForTwoGoals = new[] { -40, 40 };
    private int[] xOffsetsForThreeGoals = new[] { -60, 0, 60 };
    private LevelRules levelRules; //Holds the levelRules of the scene, used to communicate with it
    
    void Start()
    {
        levelRules = GameObject.Find("/LevelRules").GetComponent<LevelRules>();
        int numberOfGoals = levelRules.goalInputs.Length;
        goalTiles = new GoalTile[numberOfGoals];

        for (int i = 0; i < numberOfGoals; i++)
        {
            var spawnedGoalTile = Instantiate(goalTilePrefab, new Vector3(0, 0), Quaternion.identity);
            goalTiles[i] = spawnedGoalTile;
            spawnedGoalTile.goalType = levelRules.goalInputs[i].goalType;
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
            
            
            switch (levelRules.goalInputs[i].goalType)
            {
                case TileType.Yellow:
                    spawnedGoalTile.GetComponentInChildren<Image>().sprite = yellowCubeSprite;
                    break;
                case TileType.Red:
                    spawnedGoalTile.GetComponentInChildren<Image>().sprite = redCubeSprite;
                    break;
                case TileType.Blue:
                    spawnedGoalTile.GetComponentInChildren<Image>().sprite = blueCubeSprite;
                    break;
                case TileType.Green:
                    spawnedGoalTile.GetComponentInChildren<Image>().sprite = greenCubeSprite;
                    break;
                case TileType.Purple:
                    spawnedGoalTile.GetComponentInChildren<Image>().sprite = purpleCubeSprite;
                    break;
                case TileType.Duck:
                    spawnedGoalTile.GetComponentInChildren<Image>().sprite = duckSprite;
                    break;
                case TileType.Balloon:
                    spawnedGoalTile.GetComponentInChildren<Image>().sprite = balloonSprite;
                    break;
                case TileType.Rocket:
                    spawnedGoalTile.GetComponentInChildren<Image>().sprite = rocketRightSprite;
                    break;
            }
        }
    }

    public bool IsTileGoal(TileType tileType)
    {
        for (int i = 0; i < goalTiles.Length; i++)
        {
            if (goalTiles[i].goalType == tileType && goalTiles[i].goalNumber > 0)
            {
                goalTiles[i].goalNumber--;
                return true;
            }
        }

        return false;
    }

    public Vector3 GetPositionOfGoal(TileType tileType)
    {
        for (int i = 0; i < goalTiles.Length; i++)
        {
            if (goalTiles[i].goalType == tileType)
            {
                return goalTiles[i].transform.position;
            }
        }

        return new Vector3(0,0,0);
    }

    public void SubtractGoalNumberByOne(TileType tileType)
    {
        for (int i = 0; i < goalTiles.Length; i++)
        {
            if (goalTiles[i].goalType == tileType)
            {
                goalTiles[i].SubtractGoalNumberTextByOne();
            }
        }
    }
}
