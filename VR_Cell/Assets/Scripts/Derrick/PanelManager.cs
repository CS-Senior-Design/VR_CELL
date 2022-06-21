using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject panel;

    public void OpenPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}
