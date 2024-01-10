using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelRules : MonoBehaviour
{
    [Serializable]
    public struct GoalInput
    {
        [SerializeField] public TileType goalType;
        [SerializeField] public ColorType goalColorType;
        [SerializeField] public int goalNumber;
    }

    [SerializeField] public int conditionAThreshold = 4; //It is the number of tiles needed to create a rocket when popped
    [SerializeField] public int conditionBThreshold = 6; //It is the number of tiles needed to create a bomb when popped
    [SerializeField] public int conditionCThreshold = 8; //It is the number of tiles needed to create a disco ball when popped

    [field: SerializeField] public GoalInput[] goalInputs { get; private set; }
    [field: SerializeField] public int moves { get; set; }
    [field: SerializeField] public int gridWidth { get; private set; }
    [field: SerializeField] public int gridHeight { get; private set; }
    [field: SerializeField] private TileType[] possibleTileTypesToFall;
    [field: SerializeField] private ColorType[] possibleColorTypesToFall;
    [field: SerializeField] public bool useCustomTileMap { get; private set; }
    [Serializable]
    public struct TileTypeColumn
    {
        [SerializeField] public GameObject[] tilesInColumn;
    }
    [field: SerializeField] public LevelRules.TileTypeColumn[] tilePrefabMap { get; private set; } //To be used to set the grid if useCustomTileMap option is used

    public TileType GetRandomPossibleTileType()
    {
        return possibleTileTypesToFall[Random.Range(0, possibleTileTypesToFall.Length)];
    }
    
    public ColorType GetRandomPossibleColorType()
    {
        return possibleColorTypesToFall[Random.Range(0, possibleColorTypesToFall.Length)];
    }
}
