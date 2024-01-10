using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public GameObject colorTileYellowPrefab;
    public GameObject colorTileRedPrefab;
    public GameObject colorTileBluePrefab;
    public GameObject colorTileGreenPrefab;
    public GameObject colorTilePurplePrefab;
    public GameObject colorTilePinkPrefab;
    public GameObject rocketTilePrefab;
    public GameObject boxTilePrefab;

    public Tile[,] TileMap; //Used during runtime to determine game logic
    private GoalManager goalManager; //Holds the gameManager of the scene, used to communicate with it
    private LevelRules levelRules; //Holds the levelRules of the scene, used to communicate with it
    public int gridWidth, gridHeight;

    public Dictionary<TileType, GameObject> TilePrefabDict;
    public Dictionary<ColorType, GameObject> ColorTilePrefabDict;
    
    private void Awake()
    {
        TilePrefabDict = new Dictionary<TileType, GameObject>()
        {
            { TileType.RocketTile, rocketTilePrefab },
            { TileType.BoxTile, boxTilePrefab },
        };

        ColorTilePrefabDict = new Dictionary<ColorType, GameObject>()
        {
            { ColorType.Yellow, colorTileYellowPrefab },
            { ColorType.Red, colorTileRedPrefab },
            { ColorType.Blue, colorTileBluePrefab },
            { ColorType.Green, colorTileGreenPrefab },
            { ColorType.Purple, colorTilePurplePrefab },
            { ColorType.Pink, colorTilePinkPrefab }
        };

        goalManager = GameObject.Find("/ScreenCanvas/TopUICanvas/GoalPanel").GetComponent<GoalManager>();
        levelRules = GameObject.Find("/LevelRules").GetComponent<LevelRules>();

        gridWidth = levelRules.gridWidth;
        gridHeight = levelRules.gridHeight;

        GenerateGrid();
        
        if (CheckForDeadLock())
        {
            ResolveDeadlock();
        }
    }
    
    private void GenerateGrid()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gridWidth * Tile.Width + 25, gridHeight * Tile.Width + 15); //To fit borders around grid

        TileMap = new Tile[gridWidth, gridHeight];

        if (!levelRules.useCustomTileMap) //If custom tile map is not given, generate random tiles
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    TileMap[x, y] = SpawnTile(x, y, TileType.PossibleRandom, FindLocalPosByGridPos(x, y));
                }
            }

            return;
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if(levelRules.tilePrefabMap[x].tilesInColumn[y] != null)
                    TileMap[x, y] = SpawnTile(x, y, levelRules.tilePrefabMap[x].tilesInColumn[y], FindLocalPosByGridPos(x, y));
            }
        }
    }

    public Vector3 FindLocalPosByGridPos(int gridPosX, int gridPosY)
    {
        return new Vector3(-1 * (Tile.Width * ((gridWidth - 1) / 2f)) + (gridPosX * Tile.Width), (Tile.Width * ((gridHeight - 1) / 2f)) + (gridPosY * -1 * Tile.Width));
    }

    public Tile SpawnTile(int gridPosX, int gridPosY, GameObject tilePrefab, Vector3 localPos, ColorType colorType = 0)
    {
        GameObject spawnedTileObj = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedTileObj.transform.SetParent(gameObject.transform, true);
        spawnedTileObj.GetComponent<RectTransform>().localScale = Vector3.one;
        spawnedTileObj.name = $"Tile {gridPosX} {gridPosY}";
        spawnedTileObj.GetComponent<RectTransform>().localPosition = localPos;

        Tile spawnedTile = spawnedTileObj.GetComponent<Tile>();
        spawnedTile.positionInGrid = new Vector2(gridPosX, gridPosY);

        return spawnedTile;
    }
    
    public Tile SpawnTile(int gridPosX, int gridPosY, TileType tileType, Vector3 localPos, ColorType colorType = 0)
    {
        if (tileType == TileType.PossibleRandom)
        {
            tileType = levelRules.GetRandomPossibleTileType();
            
            if (tileType == TileType.ColorTile)
            {
                colorType = levelRules.GetRandomPossibleColorType();
            }
        }
        
        GameObject tilePrefab;
        if (tileType == TileType.ColorTile)
        {
            tilePrefab = ColorTilePrefabDict[colorType];
        }
        else
        {
            tilePrefab = TilePrefabDict[tileType];
        }
        
        GameObject spawnedTileObj = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        spawnedTileObj.transform.SetParent(gameObject.transform, true);
        spawnedTileObj.GetComponent<RectTransform>().localScale = Vector3.one;
        spawnedTileObj.name = $"Tile {gridPosX} {gridPosY}";
        spawnedTileObj.GetComponent<RectTransform>().localPosition = localPos;

        Tile spawnedTile = spawnedTileObj.GetComponent<Tile>();
        spawnedTile.positionInGrid = new Vector2(gridPosX, gridPosY);

        return spawnedTile;
    }

    public void RearrangeTiles()
    {
        //First existing tiles should fall to place
        FallExistingTilesToPlace();
        
        //Then add random tiles to empty locations
        FallRandomTilesFromAbove();
        
        //Check for deadlock
        if (CheckForDeadLock())
        {
            ResolveDeadlock();
        }
    }
    
    private bool CheckForDeadLock()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                Tile tile = TileMap[x, y];
                
                if(tile.tileType == TileType.RocketTile) //Add bomb and disco ball when they are implemented
                {
                    return false;
                }

                if (tile.tileType == TileType.ColorTile)
                {
                    List<Vector2> neighbours = FindNeighbourCoordinates(tile.positionInGrid);
                    foreach (Vector2 neighbour in neighbours)
                    {
                        Tile neighbourTile = TileMap[(int)neighbour.x, (int)neighbour.y];
                        if (neighbourTile != null && neighbourTile.tileType == TileType.ColorTile)
                        {
                            if (((ColorTile)tile).colorType == ((ColorTile)neighbourTile).colorType)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }

        return true;
    }

    private void ResolveDeadlock()
    {
        Dictionary<ColorType, Vector2> colorTypePosDict = new Dictionary<ColorType, Vector2>(); //Holds the position of the first colorTile encountered of each color

        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                Tile tile = TileMap[x, y];
                if (tile != null && tile.tileType == TileType.ColorTile)
                {
                    ColorTile colorTile = (ColorTile) tile;
                    if (colorTypePosDict.ContainsKey(colorTile.colorType))
                    {
                        Tile twinTile = TileMap[(int)colorTypePosDict[colorTile.colorType].x, (int)colorTypePosDict[colorTile.colorType].y];
                        List<Vector2> neighboursOfTwinTile = FindNeighbourCoordinates(twinTile.positionInGrid);
                        
                        Tile tileToSwapWith = TileMap[(int)neighboursOfTwinTile[0].x, (int)neighboursOfTwinTile[0].y];
                        Vector2 tileToSwapWithPos = new Vector2(neighboursOfTwinTile[0].x, neighboursOfTwinTile[0].y);

                        TileMap[(int)tileToSwapWithPos.x, (int)tileToSwapWithPos.y] = tile;
                        TileMap[x, y] = tileToSwapWith;

                        tile.positionInGrid = tileToSwapWithPos;
                        tile.MoveToPosition(FindLocalPosByGridPos((int)tileToSwapWithPos.x, (int)tileToSwapWithPos.y));
                        
                        if (tileToSwapWith != null)
                        {
                            tileToSwapWith.positionInGrid = new Vector2(x, y);
                            tileToSwapWith.MoveToPosition(FindLocalPosByGridPos(x, y));
                        }

                        return;
                    }
                    else
                    {
                        colorTypePosDict.Add(colorTile.colorType, tile.positionInGrid);
                    }
                }
            }
        }
        
        
    }

    private void FallExistingTilesToPlace()
    {
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (TileMap[x, y] == null)
                {
                    TileMap[x, y] = FindTheTileToFallToPosition(x, y);
                    if (TileMap[x, y])
                    {
                        TileMap[x, y].positionInGrid = new Vector2(x, y);
                        TileMap[x, y].MoveToPosition(FindLocalPosByGridPos(x,y));
                    }
                }
            }
        }
    }

    private void FallRandomTilesFromAbove()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            List<int> emptyYPositions = new List<int>();
            
            for (int y = 0; y < gridHeight; y++)
            {
                if (TileMap[x, y] == null)
                {
                    emptyYPositions.Add(y);
                }
                else
                {
                    break;
                }
            }

            for(int i = 0; i < emptyYPositions.Count; i++)
            {
                int y = emptyYPositions[i];
                TileType tileTypeToSpawn = levelRules.GetRandomPossibleTileType();
                if (tileTypeToSpawn == TileType.ColorTile)
                    TileMap[x, y] = SpawnTile(x, y, tileTypeToSpawn, FindLocalPosByGridPos(x, y - 10), levelRules.GetRandomPossibleColorType());
                else
                    TileMap[x, y] = SpawnTile(x, y, tileTypeToSpawn, FindLocalPosByGridPos(x, y - 10));

                TileMap[x, y].MoveToPosition(FindLocalPosByGridPos(x,y));
            }
        }
    }

    private Tile FindTheTileToFallToPosition(int posX, int posY)
    {
        for (int y = posY - 1; y >= 0; y--)
        {
            if (TileMap[posX, y] && TileMap[posX, y].canFall)
            {
                Tile fallingTile = TileMap[posX, y];
                TileMap[posX, y] = null;
                return fallingTile;
            }else if (TileMap[posX, y] && !TileMap[posX, y].canFall) // e.g a box tile
            {
                return null;
            }
        }
        return null;
    }
    
    public List<Vector2> FindNeighbourCoordinates(Vector2 origin)
    {
        List<Vector2> neighboursOfOrigin = new List<Vector2>();
        
        if(origin.x > 0)
            neighboursOfOrigin.Add(new Vector2(origin.x - 1, origin.y));
        
        if(origin.x < gridWidth - 1)
            neighboursOfOrigin.Add(new Vector2(origin.x + 1, origin.y));
        
        if(origin.y > 0)
            neighboursOfOrigin.Add(new Vector2(origin.x, origin.y - 1));
        
        if(origin.y < gridHeight - 1)
            neighboursOfOrigin.Add(new Vector2(origin.x, origin.y + 1));
        
        return neighboursOfOrigin;
    }

    public void NotifiedOfClick()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if (TileMap[x, y] != null)
                {
                    TileMap[x, y].UpdateEachClick();
                }
            }
        }
    }
}
