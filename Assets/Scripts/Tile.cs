using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public static int Width = 50;
    public TileType tileType;
    public Vector2 positionInGrid;
    private Vector3 targetPosition;
    private bool moving = false;
    private bool movingToGoalPos = false;
    private TileType goalPosType;
    private int speed = 900;
    
    public void CheckEligibleToClick()
    {
        if (tileType != TileType.Balloon && tileType != TileType.Duck)
            gameObject.transform.parent.GetComponent<Grid>().TileClicked(this);
    }

    public void MoveToPosition(Vector3 pos)
    {
        targetPosition = pos;
        moving = true;
    }

    public void MoveToGoalPosition(Vector3 pos, TileType goalType)
    {
        goalPosType = goalType;
        targetPosition = pos;
        moving = true;
        movingToGoalPos = true;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void SetSpeed(int speedToSet)
    {
        speed = speedToSet;
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
                    var goalPanel = gameObject.transform.parent.parent.Find("TopUICanvas").Find("GoalPanel");
                    var goalManager = goalPanel.GetComponent<GoalManager>();
                    goalPanel.GetComponent<AudioSource>().Play(0); //plays the cube collect sound
                    goalManager.SubtractGoalNumberByOne(goalPosType);
                    Destroy(gameObject);
                }

                moving = false;
                transform.localPosition = targetPosition;
            }
        }
    }
}