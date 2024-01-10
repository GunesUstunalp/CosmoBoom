using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ColorTile : Tile
{
    [field: SerializeField] public ColorType colorType { get; private set; }
    [SerializeField] private Sprite conditionalSpriteA;
    [SerializeField] private Sprite conditionalSpriteB;
    [SerializeField] private Sprite conditionalSpriteC;

    private void Start()
    {
        UpdateSprite();
    }

    public override bool ClickedTypeSpecificEffect()
    {
        List<Tile> neighboursToPop = FindNeighboursToPop();
        
        int matchSize = 0;
        for (int i = 0; i < neighboursToPop.Count; i++)
        {
            if (neighboursToPop[i].tileType == tileType && ((ColorTile)neighboursToPop[i]).colorType == colorType)
                matchSize++;
        }

        if (matchSize < 2)
            return false;
            
        foreach (Tile tileToPop in neighboursToPop)
        {
            tileToPop.Pop();
        }

        int x = (int) positionInGrid.x;
        int y = (int) positionInGrid.y;
        
        if(matchSize > levelRules.conditionCThreshold)
        {
            grid.TileMap[x,y] = grid.SpawnTile(x, y, TileType.RocketTile, grid.FindLocalPosByGridPos(x,y));
        }
        else if(matchSize > levelRules.conditionBThreshold)
        {
            grid.TileMap[x,y] = grid.SpawnTile(x, y, TileType.RocketTile, grid.FindLocalPosByGridPos(x,y));
        }
        else if (matchSize > levelRules.conditionAThreshold)
        {
            grid.TileMap[x,y] = grid.SpawnTile(x, y, TileType.RocketTile, grid.FindLocalPosByGridPos(x,y));
        }

        return true;
    }
    
    private List<Tile> FindNeighboursToPop()
    {
        List<Tile> toBePopped = new List<Tile>();
        List<Tile> checkedTiles = new List<Tile>();
        OnNeighbourClicked(tileType, toBePopped, checkedTiles, colorType);
        return toBePopped;
    }

    public override void OnNeighbourClicked(TileType originType, List<Tile> toBePopped, List<Tile> checkedTiles, ColorType originColorType = 0)
    {
        if (checkedTiles.Contains(this))
            return;
        
        checkedTiles.Add(this);

        if (tileType != originType || colorType != originColorType)
            return;
        
        if(!toBePopped.Contains(this))
            toBePopped.Add(this);

        List<Vector2> neighbourCoordinates = grid.FindNeighbourCoordinates(positionInGrid);
        
        foreach (Vector2 neighbourCoord in neighbourCoordinates)
        {
            Tile neighbour = grid.TileMap[(int)neighbourCoord.x, (int)neighbourCoord.y];
            
            if (neighbour && !toBePopped.Contains(neighbour))
            {
                neighbour.OnNeighbourClicked(originType, toBePopped, checkedTiles, originColorType);
            }
        }
    }
    
    public override void UpdateEachClick()
    {
        base.UpdateEachClick();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        List<Tile> neighboursToPop = FindNeighboursToPop();
        
        int matchSize = 0;
        for (int i = 0; i < neighboursToPop.Count; i++)
        {
            if (neighboursToPop[i].tileType == tileType)
                matchSize++;
        }
        
        if(matchSize > levelRules.conditionCThreshold)
        {
            tileImage.sprite = conditionalSpriteC;
        }
        else if(matchSize > levelRules.conditionBThreshold)
        {
            tileImage.sprite = conditionalSpriteB;
        }
        else if (matchSize > levelRules.conditionAThreshold)
        {
            tileImage.sprite = conditionalSpriteA;
        }
        else
        {
            tileImage.sprite = defaultSprite;
        }
    }
}
