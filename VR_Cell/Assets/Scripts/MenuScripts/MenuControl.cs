using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControl : MonoBehaviour
{
    public GameObject _immersiveText;
    public GameObject _labText;
    public GameObject _quitText;
    public GameObject _cellMembrane;
    public GameObject _cell;

    private Color _cellMembraneMaterial;


    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // function to call on cell hover
    public void cellHover()
    {
        // scale the cell up a little
        _cell.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

        // highlight the cell a little
        _cellMembraneMaterial = _cellMembrane.GetComponent<Renderer>().material.color;
        _cellMembrane.GetComponent<Renderer>().material.color = new Color(224f, 215f, 61f);

        // immersive title shows up
        _immersiveText.SetActive(true);
    }

    public void cellHoverExit()
    {
        // scale the cell down a little
        _cell.transform.localScale = new Vector3(1f, 1f, 1f);

        // unhighlight the cell
        _cellMembrane.GetComponent<Renderer>().material.color = _cellMembraneMaterial;

        // immersive title disappears
        _immersiveText.SetActive(false);
    }
}
