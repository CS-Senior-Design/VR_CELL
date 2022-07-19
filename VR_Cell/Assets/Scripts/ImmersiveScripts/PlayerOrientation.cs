using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script gets attached to the player and is called when the player 
// hits a teleportation anchor to orient them properly based on the organelle they are in front of

public class PlayerOrientation : MonoBehaviour
{
    public void setOrientation(GameObject ui)
    {
        Debug.Log("here");
        transform.LookAt(ui.transform.position);
    }
}
