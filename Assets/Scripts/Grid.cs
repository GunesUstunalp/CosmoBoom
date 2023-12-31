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
    [SerializeField] private Tile tilePrefab;

    [SerializeField] private AudioClip balloonPoppingSound;
    [SerializeField] private AudioClip cubePoppingSound;
    [SerializeField] private AudioClip duckPoppingSound;
    [SerializeField] private AudioClip cubeCollectingSound;
    
    [SerializeField] private Sprite yellowCubeSprite;
    [SerializeField] private Sprite redCubeSprite;
    [SerializeField] private Sprite blueCubeSprite;
    [SerializeField] private Sprite greenCubeSprite;
    [SerializeField] private Sprite purpleCubeSprite;
    [SerializeField] private Sprite duckSprite;
    [SerializeField] private Sprite balloonSprite;
    [SerializeField] private Sprite rocketRightSprite;
    [SerializeField] private Sprite rocketLeftSprite;

    private Tile[,] tileMap; //Used during runtime to determine game logic
    private GoalManager goalManager; //Holds the gameManager of the scene, used to communicate with it
    private LevelRules levelRules; //Holds the levelRules of the scene, used to communicate with it
    private bool isAnimationPlaying = false; //true if rocket animation still playing, prevents tile clicking
    private int gridWidth, gridHeight;
    
    private void Start()
    {
        goalManager = GameObject.Find("/ScreenCanvas/TopUICanvas/GoalPanel").GetComponent<GoalManager>();
        levelRules = GameObject.Find("/LevelRules").GetComponent<LevelRules>();

        gridWidth = levelRules.gridWidth;
        gridHeight = levelRules.gridHeight;
        
        if (!levelRules.useCustomTileMap)
        {
            levelRules.GenerateRandomTileTypeMap();
        }
        
        GenerateGrid();
        CheckBottomRowForDucks(1); //if there are ducks present at the bottom row at the game's beginning
    }
    
    private void GenerateGrid()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gridWidth * Tile.Width + 25, gridHeight * Tile.Width + 15); //To fit borders around grid

        tileMap = new Tile[gridWidth, gridHeight];
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                tileMap[x, y] = SpawnTile(x, y, levelRules.tileTypeMap[x].tilesInColumn[y], FindLocalPosByGridPos(x, y));
            }
        }
    }

    private Vector3 FindLocalPosByGridPos(int gridPosX, int gridPosY)
    {
        return new Vector3(-1 * (Tile.Width * ((gridWidth - 1) / 2f)) + (gridPosX * Tile.Width), (Tile.Width * ((gridHeight - 1) / 2f)) + (gridPosY * -1 * Tile.Width));
    }

    private Tile SpawnTile(int gridPosX, int gridPosY, TileType tileType, Vector3 localPos)
    {
        var spawnedTile = Instantiate(tilePrefab, new Vector3(0, 0), Quaternion.identity);
        spawnedTile.transform.SetParent(gameObject.transform, true);
        spawnedTile.GetComponent<RectTransform>().localScale = Vector3.one;
        spawnedTile.name = $"Tile {gridPosX} {gridPosY}";
        
        spawnedTile.GetComponent<RectTransform>().localPosition = localPos;
        spawnedTile.positionInGrid = new Vector2(gridPosX, gridPosY);

        var particleSystem = spawnedTile.GetComponent<ParticleSystem>().main;

        if (tileType == TileType.PossibleRandom)
            tileType = levelRules.GetRandomPossibleTileType();
        
        switch (tileType)
        {
            case TileType.Yellow:
                spawnedTile.GetComponent<Image>().sprite = yellowCubeSprite;
                spawnedTile.tileType = TileType.Yellow;
                particleSystem.startColor = Color.yellow;
                spawnedTile.PoppingSound = cubePoppingSound;
                break;
            case TileType.Red:
                spawnedTile.GetComponent<Image>().sprite = redCubeSprite;
                spawnedTile.tileType = TileType.Red;
                particleSystem.startColor = Color.red;
                spawnedTile.PoppingSound = cubePoppingSound;
                break;
            case TileType.Blue:
                spawnedTile.GetComponent<Image>().sprite = blueCubeSprite;
                spawnedTile.tileType = TileType.Blue;
                particleSystem.startColor = Color.blue;
                spawnedTile.PoppingSound = cubePoppingSound;
                break;
            case TileType.Green:
                spawnedTile.GetComponent<Image>().sprite = greenCubeSprite;
                spawnedTile.tileType = TileType.Green;
                particleSystem.startColor = Color.green;
                spawnedTile.PoppingSound = cubePoppingSound;
                break;
            case TileType.Purple:
                spawnedTile.GetComponent<Image>().sprite = purpleCubeSprite;
                spawnedTile.tileType = TileType.Purple;
                particleSystem.startColor = Color.magenta;
                spawnedTile.PoppingSound = cubePoppingSound;
                break;
            case TileType.Duck:
                spawnedTile.GetComponent<Image>().sprite = duckSprite;
                spawnedTile.tileType = TileType.Duck;
                particleSystem.startColor = Color.yellow;
                spawnedTile.PoppingSound = duckPoppingSound;
                break;
            case TileType.Balloon:
                spawnedTile.GetComponent<Image>().sprite = balloonSprite;
                spawnedTile.tileType = TileType.Balloon;
                particleSystem.startColor = Color.magenta;
                spawnedTile.PoppingSound = balloonPoppingSound;
                break;
            case TileType.Rocket:
                spawnedTile.GetComponent<Image>().sprite = rocketRightSprite;
                spawnedTile.tileType = TileType.Rocket;
                particleSystem.startColor = Color.clear;
                spawnedTile.AddComponent<RocketTile>();
                break;
        }

        return spawnedTile;
    }

    public void TileClicked(Tile tile)
    {
        if (levelRules.moves <= 0 || isAnimationPlaying)
            return;
        
        int x = (int)tile.positionInGrid.x;
        int y = (int)tile.positionInGrid.y;

        if (tile.tileType == TileType.Rocket) //if it is a rocket
        {
            List<Tile> neighboursToPop = new List<Tile>();
            FindNeighboursOfRocketToPop(tile, neighboursToPop);
            foreach (Tile tileToPop in neighboursToPop)
            {
                PopTile(tileToPop);
            }
            
            Invoke("UpdateGridAfterTileClick", 0.5f);
        }
        else //if it is a cube
        {
            List<Tile> neighboursToPop = FindNeighboursOfCubeToPop(tile);

            int matchSize = 0;
            for (int i = 0; i < neighboursToPop.Count; i++)
            {
                if (neighboursToPop[i].tileType == tile.tileType)
                    matchSize++;
            }

            if (matchSize < 2)
                return;

            foreach (Tile tileToPop in neighboursToPop)
            {
                PopTile(tileToPop);
            }
            
            if (matchSize > 4) //spawn rocket
            {
                tileMap[x,y] = SpawnTile(x, y, TileType.Rocket, FindLocalPosByGridPos(x,y));
            }

            UpdateGridAfterTileClick();
        }
        levelRules.moves--;
        goalManager.UpdateMovesText();
        if(levelRules.moves < 1)
            goalManager.CheckForDefeat();
    }

    private void UpdateGridAfterTileClick()
    {
        RearrangeTiles();
        CheckBottomRowForDucks(1);
        isAnimationPlaying = false;
    }

    private void RearrangeTiles()
    {
        //First existing tiles should fall to place
        FallExistingTilesToPlace();
        
        //Then add random tiles to empty locations
        FallRandomTilesFromAbove(0);
    }

    private void FallExistingTilesToPlace()
    {
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (tileMap[x, y] == null)
                {
                    tileMap[x, y] = FindTheTileToFallToPosition(x, y);
                    if (tileMap[x, y])
                    {
                        tileMap[x, y].positionInGrid = new Vector2(x, y);
                        tileMap[x, y].MoveToPosition(FindLocalPosByGridPos(x,y));
                    }
                }
            }
        }
    }

    private void FallRandomTilesFromAbove(int duckOffset)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = gridHeight - 1; y >= 0; y--)
            {
                if (tileMap[x, y] == null)
                {
                    tileMap[x,y] = SpawnTile(x,y, levelRules.GetRandomPossibleTileType(), FindLocalPosByGridPos(x,y - 10 - duckOffset));
                    tileMap[x, y].MoveToPosition(FindLocalPosByGridPos(x,y));
                }
            }
        }
    }

    private Tile FindTheTileToFallToPosition(int posX, int posY)
    {
        for (int y = posY - 1; y >= 0; y--)
        {
            if (tileMap[posX, y])
            {
                Tile fallingTile = tileMap[posX, y];
                tileMap[posX, y] = null;
                return fallingTile;
            }
        }
        return null;
    }

    private void PlayRocketAnimation(Tile tile)
    {
        int x = (int)tile.positionInGrid.x;
        int y = (int)tile.positionInGrid.y;
        RocketTile.RocketOrientation orientation = tile.gameObject.GetComponent<RocketTile>().getRocketOrientation();
        
        Tile rocketTemp1 = SpawnTile(x,y,TileType.TemporaryAnimation,FindLocalPosByGridPos(x,y));
        Tile rocketTemp2 = SpawnTile(x,y,TileType.TemporaryAnimation,FindLocalPosByGridPos(x,y));
        rocketTemp1.GetComponent<Image>().sprite = rocketRightSprite;
        rocketTemp2.GetComponent<Image>().sprite = rocketLeftSprite;
        rocketTemp1.SetSpeed(1500);
        rocketTemp2.SetSpeed(1500);

        if (orientation == RocketTile.RocketOrientation.LeftRight)
        {
            rocketTemp1.MoveToPosition(FindLocalPosByGridPos(x + 100,y));
            rocketTemp2.MoveToPosition(FindLocalPosByGridPos(x - 100,y));
        }
        else //if the orientation is UpDown
        {
            rocketTemp1.transform.Rotate(Vector3.forward, 90);
            rocketTemp2.transform.Rotate(Vector3.forward, 90);
            rocketTemp1.MoveToPosition(FindLocalPosByGridPos(x ,y - 100));
            rocketTemp2.MoveToPosition(FindLocalPosByGridPos(x,y + 100));
        }
            
        rocketTemp1.Invoke("DestroyObject", 1);
        rocketTemp2.Invoke("DestroyObject", 1);
        isAnimationPlaying = true;
    }
    private void FindNeighboursOfRocketToPop(Tile tile, List<Tile> toBePopped)
    {
        int posX = (int) tile.positionInGrid.x;
        int posY = (int) tile.positionInGrid.y;
        RocketTile.RocketOrientation orientation = tile.gameObject.GetComponent<RocketTile>().getRocketOrientation();

        if (toBePopped.Contains(tile))
            return;
        toBePopped.Add(tile);
        
        PlayRocketAnimation(tile);

        if (orientation == RocketTile.RocketOrientation.LeftRight)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (tileMap[x, posY] != null && tileMap[x, posY] != tile && tileMap[x, posY].tileType != TileType.Duck)
                {
                    if(tileMap[x, posY].tileType == TileType.Rocket)
                        FindNeighboursOfRocketToPop(tileMap[x, posY], toBePopped);
                    else if(!toBePopped.Contains(tileMap[x,posY]))
                        toBePopped.Add(tileMap[x,posY]);
                }
            }
        }
        else //orientation == RocketTile.RocketOrientation.UpDown
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (tileMap[posX, y] != null && tileMap[posX, y] != tile && tileMap[posX, y].tileType != TileType.Duck)
                {
                    if(tileMap[posX, y].tileType == TileType.Rocket)
                        FindNeighboursOfRocketToPop(tileMap[posX, y], toBePopped);
                    else if(!toBePopped.Contains(tileMap[posX,y]))
                        toBePopped.Add(tileMap[posX,y]);
                }
            }
        }
    }
    
    private List<Tile> FindNeighboursOfCubeToPop(Tile tile)
    {
        Vector2 originPos = tile.positionInGrid;
        TileType originType = tile.tileType;

        List<Tile> toBePopped = new List<Tile>();
        toBePopped.Add(tile);
        
        FindNeighboursToPopRecursiveHelper(originPos, originType, toBePopped);

        return toBePopped;
    }

    private void FindNeighboursToPopRecursiveHelper(Vector2 originPos, TileType originType, List<Tile> toBePopped)
    {
        List<Vector2> neighbourCoordinates = FindNeighbourCoordinates(originPos);

        foreach (Vector2 neighbourCoord in neighbourCoordinates)
        {
            Tile neighbour = tileMap[(int)neighbourCoord.x, (int)neighbourCoord.y];
            
            if (neighbour && (neighbour.tileType == originType || neighbour.tileType == TileType.Balloon) && !toBePopped.Contains(neighbour))
            {
                toBePopped.Add(neighbour);
                if(neighbour.tileType != TileType.Balloon) //Balloons do not pop their neighbour balloons
                    FindNeighboursToPopRecursiveHelper(neighbour.positionInGrid, neighbour.tileType, toBePopped);
            }
        }
    }
    
    private List<Vector2> FindNeighbourCoordinates(Vector2 origin)
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
    
    private void CheckBottomRowForDucks(int howManyTimesCalled)
    {
        bool duckFound = false;
        for (int x = 0; x < gridWidth; x++)
        {
            if (tileMap[x, gridHeight - 1] && tileMap[x, gridHeight - 1].tileType == TileType.Duck)
            {
                PopTile(tileMap[x, gridHeight - 1]);
                duckFound = true;
            }
        }

        if (duckFound)
        {
            FallExistingTilesToPlace();
            CheckBottomRowForDucks(howManyTimesCalled + 1);
            FallRandomTilesFromAbove(howManyTimesCalled);
        }
    }

    private void PopTile(Tile tile)
    {
        if (tile == null && tile.gameObject == null)
            return;

        gameObject.GetComponent<AudioSource>().clip = tile.PoppingSound;
        gameObject.GetComponent<AudioSource>().Play(0); //play popping sound
        tile.gameObject.GetComponent<ParticleSystem>().Play();
        tile.gameObject.GetComponent<Image>().enabled = false;
        tile.gameObject.GetComponent<Button>().enabled = false;

        tileMap[(int)tile.positionInGrid.x, (int)tile.positionInGrid.y] = null;

        if (goalManager.IsTileGoal(tile.tileType))
        {
            int x = (int) tile.positionInGrid.x;
            int y = (int) tile.positionInGrid.y;
            Tile tileTemp = SpawnTile(x,y,TileType.TemporaryAnimation,FindLocalPosByGridPos(x,y));
            tileTemp.GetComponent<Image>().sprite = tile.gameObject.GetComponent<Image>().sprite;

            tileTemp.MoveToGoalPosition(transform.InverseTransformPoint(goalManager.GetPositionOfGoal(tile.tileType)), tile.tileType);
        }
        tile.Invoke("DestroyObject", 1);
    }
}
