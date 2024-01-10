using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RocketTile : Tile
{
    [SerializeField] private Sprite reversedSprite;
    
    public enum RocketOrientation
    {
        UpDown,
        LeftRight,
        Random
    }

    [SerializeField] private RocketOrientation orientation;

    private void UpdateSpriteBasedOnOrientation()
    {
        gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        if (orientation == RocketOrientation.UpDown)
        {
            gameObject.transform.Rotate(Vector3.forward, 90);
        }
    }

    public RocketOrientation GetRocketOrientation()
    {
        return orientation;
    }

    private RocketOrientation GetRandomRocketOrientation()
    {
        RocketOrientation[] possibleOrientations = {RocketOrientation.UpDown, RocketOrientation.LeftRight};
        return possibleOrientations[Random.Range(0, possibleOrientations.Length)];
    }
    
    private void Start()
    {
        if (orientation == RocketOrientation.Random)
            orientation = GetRandomRocketOrientation();
        
        UpdateSpriteBasedOnOrientation();
    }

    public override bool ClickedTypeSpecificEffect()
    {
        List<Tile> neighboursToPop = new List<Tile>();
        FindNeighboursToPop(neighboursToPop);

        foreach (Tile tileToPop in neighboursToPop)
        {
            tileToPop.Pop();
        }

        return true;
    }

    public override void OnNeighbourClicked(TileType originType, List<Tile> toBePopped, List<Tile> checkedTiles, ColorType originColorType = 0)
    {
        if (checkedTiles.Contains(this))
            return;
        
        checkedTiles.Add(this);
    }

    private void PlayRocketAnimation()
    {
        int x = (int)positionInGrid.x;
        int y = (int)positionInGrid.y;

        Tile rocketTemp1 = grid.SpawnTile(x,y,TileType.RocketTile,grid.FindLocalPosByGridPos(x,y));
        Tile rocketTemp2 = grid.SpawnTile(x,y,TileType.RocketTile,grid.FindLocalPosByGridPos(x,y));
        ((RocketTile)rocketTemp1).orientation = orientation;
        ((RocketTile)rocketTemp2).orientation = orientation;
        rocketTemp1.tileImage.sprite = defaultSprite;
        rocketTemp2.tileImage.sprite = reversedSprite;
        ((RocketTile)rocketTemp1).UpdateSpriteBasedOnOrientation();
        ((RocketTile)rocketTemp2).UpdateSpriteBasedOnOrientation();
        
        rocketTemp1.SetSpeed(1500);
        rocketTemp2.SetSpeed(1500);
        
        if (orientation == RocketOrientation.LeftRight)
        {
            rocketTemp1.MoveToPosition(grid.FindLocalPosByGridPos(x + 100,y));
            rocketTemp2.MoveToPosition(grid.FindLocalPosByGridPos(x - 100,y));
        }
        else //if the orientation is UpDown
        {
            rocketTemp1.MoveToPosition(grid.FindLocalPosByGridPos(x ,y - 100));
            rocketTemp2.MoveToPosition(grid.FindLocalPosByGridPos(x,y + 100));
        }
            
        rocketTemp1.Invoke("DestroyObject", 1);
        rocketTemp2.Invoke("DestroyObject", 1);
    }
    private void FindNeighboursToPop(List<Tile> toBePopped)
    {
        if(toBePopped.Contains(this))
            return;
        
        int posX = (int) positionInGrid.x;
        int posY = (int) positionInGrid.y;
        
        toBePopped.Add(this);

        if (orientation == RocketOrientation.LeftRight)
        {
            for (int x = 0; x < grid.gridWidth; x++)
            {
                if (grid.TileMap[x, posY] != null && grid.TileMap[x, posY] != this)
                {
                    if (grid.TileMap[x, posY].tileType == TileType.RocketTile)
                        ((RocketTile)grid.TileMap[x, posY]).FindNeighboursToPop(toBePopped);
                    else if (!toBePopped.Contains(grid.TileMap[x, posY]))
                        toBePopped.Add(grid.TileMap[x, posY]);
                }
            }
        }
        else //orientation == RocketOrientation.UpDown
        {
            for (int y = 0; y < grid.gridHeight; y++)
            {
                if (grid.TileMap[posX, y] != null && grid.TileMap[posX, y] != this)
                {
                    if(grid.TileMap[posX, y].tileType == TileType.RocketTile)
                        ((RocketTile)grid.TileMap[posX, y]).FindNeighboursToPop(toBePopped);
                    else if(!toBePopped.Contains(grid.TileMap[posX,y]))
                        toBePopped.Add(grid.TileMap[posX,y]);
                }
            }
        }
    }
    
    public override void Pop()
    {
        PlayRocketAnimation();
        base.Pop();
    }
}
