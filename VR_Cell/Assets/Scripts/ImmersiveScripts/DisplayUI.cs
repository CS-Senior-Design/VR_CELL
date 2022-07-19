using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayUI : MonoBehaviour
{
    private bool _generatingRibosomes = false;

    public void setGeneratingRibosomes(bool value)
    {
        _generatingRibosomes = value;
    }

    public void showUI(GameObject ui)
    {
        if (_generatingRibosomes == true)
        {
            return;
        }
        else    
        {
            ui.SetActive(true);
        }

    }

    public void hideUI(GameObject ui)
    {
        ui.SetActive(false);
    }
}
