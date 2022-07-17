using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayUI : MonoBehaviour
{
    private GameObject _currentActiveUI = null;
    private bool _generatingRibosomes = false;

    public void SetActiveUI(GameObject ui)
    {
        if (ui.tag == "nucleolusUI")
        {
            // only display the nucleolus UI if we are not currently generating ribosomes
            if (!_generatingRibosomes)
            {
                if (_currentActiveUI != null)
                {
                    _currentActiveUI.SetActive(false);
                }
                _currentActiveUI = ui;
                _currentActiveUI.SetActive(true);
            }
        }
    }    

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
