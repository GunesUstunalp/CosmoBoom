using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelRules : MonoBehaviour
{
    [field: SerializeField] public int moves { get; set; }
    [field: SerializeField] public int gridWidth { get; private set; }
    [field: SerializeField] public int gridHeight { get; private set; }
    [field: SerializeField] private TileType[] possibleTileTypesToFall;
    [field: SerializeField] public bool useCustomTileMap { get; private set; }
    [Serializable]
    public struct TileTypeColumn
    {
        [SerializeField] public TileType[] tilesInColumn;
    }
    [field: SerializeField] public LevelRules.TileTypeColumn[] tileTypeMap { get; private set; } //To be used to set the grid if useCustomTileMap option is used

    public TileType GetRandomPossibleTileType()
    {
        return possibleTileTypesToFall[Random.Range(0, possibleTileTypesToFall.Length)];
    }
    
    public void GenerateRandomTileTypeMap()
    {
        tileTypeMap = new TileTypeColumn[gridWidth];
        for (int x = 0; x < gridWidth; x++)
        {
            tileTypeMap[x].tilesInColumn = new TileType[gridHeight];
            for (int y = 0; y < gridHeight; y++)
            {
                tileTypeMap[x].tilesInColumn[y] = GetRandomPossibleTileType();
            }
        }
    }
}
