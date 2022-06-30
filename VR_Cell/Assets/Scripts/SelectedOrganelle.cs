using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedOrganelle : MonoBehaviour
{
    public void organelleHovered(int organelle)
    {
        Debug.Log(organelle);
    }

    public void organelleSelected(GameObject organelle)
    {
        Debug.Log("HERE");
    }
}
