using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RocketTile : MonoBehaviour
{
    public enum RocketOrientation
    {
        UpDown,
        LeftRight
    }

    private RocketOrientation orientation;

    public void setRocketOrientation(RocketOrientation orientationToSet)
    {
        gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        orientation = orientationToSet;
        if (orientation == RocketOrientation.UpDown)
        {
            gameObject.transform.Rotate(Vector3.forward, 90);
        }
    }

    public RocketOrientation getRocketOrientation()
    {
        return orientation;
    }

    private RocketOrientation getRandomRocketOrientation()
    {
        RocketOrientation[] possibleOrientations = {RocketOrientation.UpDown, RocketOrientation.LeftRight};
        return possibleOrientations[Random.Range(0, possibleOrientations.Length)];
    }
    
    private void Start()
    {
        setRocketOrientation(getRandomRocketOrientation());
    }
}
