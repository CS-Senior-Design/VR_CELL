using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ERQuest : MonoBehaviour
{
    private int _curStep = 0;
    
    // get the socket so we can easily activate and deactivate it
    public GameObject _socket;

    // get the text gameobjects so we can change it based on the text
    public GameObject _textArea;
    public GameObject _button;

    // text for each step
    private string _step0 = "Hi, I am the Rough Endoplasmic Retirculum! Could you please help me with something?";

    // Start is called before the first frame update
    void Start()
    {
        // start by setting the socket as inactive
        _socket.SetActive(false);
    }

    public void increaseCurStep()
    {
        _curStep++;
    }

    public int getCurStep()
    {
        return _curStep;
    }

    public void displayCanvas()
    {
        switch (_curStep)
        {
            case 0:
            {
                // set the text of the canvas to step 0 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step0;
                // display the roughER UI
                GameObject.Find("roughERUI").SetActive(true);
                break;
            }

            default:
            {
                break;
            }
        }
    }
}
