using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibosomeState : MonoBehaviour
{
    private bool currentlyGrabbed = false;

    public bool isGrabbed()
    {
        return currentlyGrabbed;
    }

    public void setGrabbed(bool value)
    {
        currentlyGrabbed = value;
    }

    public void setRibosomeScale()
    {
        transform.localScale = new Vector3(100f, 100f, 100f);
    }
}
