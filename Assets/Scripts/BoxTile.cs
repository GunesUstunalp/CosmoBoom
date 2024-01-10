using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTile : Tile
{
    [SerializeField] private int health;
    [SerializeField] private Sprite conditionalSpriteHealth2;
    [SerializeField] private Sprite conditionalSpriteHealth1;
    
    private Sprite[] conditionalSprites;
    
    private void Start()
    {
        conditionalSprites = new Sprite[2];
        conditionalSprites[0] = conditionalSpriteHealth1;
        conditionalSprites[1] = conditionalSpriteHealth2;
        
        tileImage.sprite = conditionalSprites[health - 1];
    }
    
    public override bool ClickedTypeSpecificEffect()
    {
        //Do nothing
        return false;
    }
    
    public override void OnNeighbourClicked(TileType originType, List<Tile> toBePopped, List<Tile> checkedTiles, ColorType originColorType = 0)
    { 
        if (checkedTiles.Contains(this))
            return;
        
        checkedTiles.Add(this);
        toBePopped.Add(this);
    }

    public override void Pop()
    {
        health--;
        if (health <= 0)
        {
            base.Pop();
        }
        else
        {
            tileImage.sprite = conditionalSprites[health - 1];
        }
    }
}
