using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script gets attached to the player and is called when the player 
// hits a teleportation anchor to orient them properly based on the organelle they are in front of

public class PlayerOrientation : MonoBehaviour
{
    public void setOrientation(string value)
    {
        if (value == "nucleolus")
        {
            Debug.Log("here");
            transform.Rotate(-9.501f, -173.954f, 0.071f);
        }
    }
}
