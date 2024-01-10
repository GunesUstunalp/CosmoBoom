using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class Tile : MonoBehaviour
{
    public const int Width = 50;
    public int speed = 900;
    
    [SerializeField] public bool canFall = true;
    [SerializeField] protected AudioClip poppingSound;
    [field: SerializeField] public Sprite defaultSprite{ get; private set;}

    public TileType tileType;
    public Vector2 positionInGrid;

    protected ParticleSystem tParticleSystem; // Holds the particle system of the object, used to communicate with it
    protected GoalManager goalManager; //Holds the gameManager of the scene, used to communicate with it
    protected AudioSource goalAudioSource; //Holds the audio source of the goalPanel, used to communicate with it
    protected Grid grid; // Holds the grid of the scene, used to communicate with it
    protected AudioSource gridAudioSource; //Holds the audio source of the grid, used to communicate with it
    public Image tileImage { get; private set; } // Holds the sprite of the object, used to communicate with it
    protected LevelRules levelRules; //Holds the levelRules of the scene, used to communicate with it

    private Vector3 targetPosition;
    private bool moving = false;
    private bool movingToGoalPos = false;
    protected GoalTile goalTile; //Holds the goalTile associated with this tile, only used if this tile is moving to goal position
    //private TileType goalType; //used to store the target goal type when moving to goal position

    private void Awake()
    {
        tParticleSystem = GetComponent<ParticleSystem>();
        goalManager = GameObject.Find("/ScreenCanvas/TopUICanvas/GoalPanel").GetComponent<GoalManager>();
        goalAudioSource = GameObject.Find("/ScreenCanvas/TopUICanvas/GoalPanel").GetComponent<AudioSource>();
        grid = GameObject.Find("/ScreenCanvas/GridPanel").GetComponent<Grid>();
        gridAudioSource = GameObject.Find("/ScreenCanvas/GridPanel").GetComponent<AudioSource>();
        levelRules = GameObject.Find("/LevelRules").GetComponent<LevelRules>();

        tileImage = transform.Find("TileSprite").GetComponent<Image>();
        tileImage.sprite = defaultSprite;
    }

    public void Clicked()
    {
        if (levelRules.moves <= 0)
            return;

        if (ClickedTypeSpecificEffect()) 
        {
            grid.RearrangeTiles();
            grid.NotifiedOfClick();
            levelRules.moves--;
            goalManager.UpdateMovesText();
            if(levelRules.moves < 1)
                goalManager.CheckForDefeat();
        }
    }

    public abstract bool ClickedTypeSpecificEffect(); //returns true if the click consumed a move, false otherwise

    public abstract void OnNeighbourClicked(TileType originType, List<Tile> toBePopped, List<Tile> checkedTiles, ColorType originColorType = 0);

    public virtual void UpdateEachClick()
    {
        //Do nothing, to be overridden in certain tile types e.g. colorTiles
    }

    public virtual void Pop()
    {
        gridAudioSource.clip = poppingSound;
        gridAudioSource.Play(0); //play popping sound
        tParticleSystem.Play();
        tileImage.enabled = false;
        gameObject.GetComponent<Button>().enabled = false;

        grid.TileMap[(int)positionInGrid.x, (int)positionInGrid.y] = null;

        goalTile = goalManager.GetGoalTileOfTile(this);
        if (goalTile != null)
        {
            tileImage.enabled = true;
            int x = (int) positionInGrid.x;
            int y = (int) positionInGrid.y;
            //Tile tileTemp = grid.SpawnTile(x,y,TileType.TemporaryAnimation,grid.FindLocalPosByGridPos(x,y));
            //tileTemp.tileImage.sprite = tileImage.sprite;

            //tileTemp.MoveToGoalPosition(transform.InverseTransformPoint(goalManager.GetPositionOfGoal(tileType)));
            MoveToGoalPosition(grid.transform.InverseTransformPoint(goalTile.transform.position));
        }
        
        Invoke("DestroyObject", 1);
    }

    public void MoveToPosition(Vector3 pos)
    {
        targetPosition = pos;
        moving = true;
    }

    public void MoveToGoalPosition(Vector3 pos)
    {
        targetPosition = pos;
        moving = true;
        movingToGoalPos = true;
    }
    
    protected void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (moving)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01)
            {
                if (movingToGoalPos)
                {
                    
                    goalAudioSource.Play(0); //plays the cube collect sound
                    goalManager.SubtractGoalNumberByOne(goalTile);
                    Destroy(gameObject);
                }

                moving = false;
                transform.localPosition = targetPosition;
            }
        }
    }
    
    public void SetSpeed(int newSpeed)
    {
        speed = newSpeed;
    }
}