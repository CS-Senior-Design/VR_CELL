using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuControl : MonoBehaviour
{
    public GameObject _immersiveText;
    public GameObject _labText;
    public GameObject _labBoard;
    public GameObject _quitText;
    public GameObject _cellMembrane;
    public GameObject _cell;

    private Color _cellMembraneMaterial;

    public GameObject _boardText;
    public GameObject _boardTitleText;

    private Color _boardMaterial;

    // Start is called before the first frame update
    void Start()
    {
        //get original cell membrane material
        _cellMembraneMaterial = _cellMembrane.GetComponent<Renderer>().material.color;
        //get original board material
        _boardMaterial = _labBoard.GetComponent<Renderer>().material.color;
    }
    
    // function to call on cell hover
    public void cellHover()
    {
        //change the board title text
        _boardTitleText.GetComponent<TMPro.TextMeshProUGUI>().text = "Immersive Mode";
        //Change the board text 
        _boardText.GetComponent<TMPro.TextMeshProUGUI>().text = "Jump inside the cell and explore!";



        // scale the cell up a little
        _cell.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

        // highlight the cell a little
        _cellMembrane.GetComponent<Renderer>().material.color = new Color(255,255,0);

        // immersive title shows up
        _immersiveText.SetActive(true);
    }

    public void cellHoverExit()
    {
        _boardTitleText.GetComponent<TMPro.TextMeshProUGUI>().text = "Pick a mode!";
        //Change the board text 
        _boardText.GetComponent<TMPro.TextMeshProUGUI>().text = "Choose between Immersive Mode and Lab Mode.";

        // scale the cell down a little
        _cell.transform.localScale = new Vector3(1f, 1f, 1f);

        // unhighlight the cell
        _cellMembrane.GetComponent<Renderer>().material.color = _cellMembraneMaterial;

        // immersive title disappears
        _immersiveText.SetActive(false);
    }

    public void boardHover()
    {
        //change the board title text
        _boardTitleText.GetComponent<TMPro.TextMeshProUGUI>().text = "Lab Mode";
        //Change the board text
        _boardText.GetComponent<TMPro.TextMeshProUGUI>().text = "Explore the lab and learn about the cell!";
        //scale the board up a little
        _labBoard.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        //highlight the board a little
        _labBoard.GetComponent<Renderer>().material.color = new Color(255,255,0);
        //lab title shows up
        _labText.SetActive(true);
    }

    public void boardHoverExit()
    {
        _boardTitleText.GetComponent<TMPro.TextMeshProUGUI>().text = "Pick a mode!";
        //Change the board text 
        _boardText.GetComponent<TMPro.TextMeshProUGUI>().text = "Choose between Immersive Mode and Lab Mode.";
        //scale the board down a little
        _labBoard.transform.localScale = new Vector3(1f, 1f, 1f);
        //unhighlight the board
        _labBoard.GetComponent<Renderer>().material.color = _boardMaterial;
        //lab title disappears
        _labText.SetActive(false);
    }

}
