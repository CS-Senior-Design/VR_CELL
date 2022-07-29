using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MembraneUI : MonoBehaviour
{
    private int _curStep = 0;
    private int _totalSteps = 2;

    private float _animationSpeed = 0.8f;

    private InputHandling _inputScript;
    private GameObject _player;

    public GameObject _textArea;
    public GameObject _nextButton;
    public GameObject _nextButtonText;
    public GameObject _membraneFastTravelSocket;

    public GameObject _lysosome;
    private GameObject _vesicle;
    public GameObject _lysosomeUp;
    private Vector3 _lysosomeUpPosition;
    public GameObject _lysosomeDown;
    private Vector3 _lysosomeDownPosition;
    public GameObject _vesicleStart;
    private Vector3 _vesicleStartPosition;
    public GameObject _vesicleEnd;
    private Vector3 _vesicleEndPosition;
    
    private string _step0 = "My job is to separate the inside of the cell from the outside environment! I control what comes in and out...";
    private string _step1 = "Now that I think about it... I don't remember letting you in here.";
    
    void Awake()
    {
        // get the playerParent
        GameObject playerParent = GameObject.FindGameObjectWithTag("playerParent");
        _inputScript = playerParent.GetComponent<InputHandling>();
        // get the player
        _player = GameObject.FindGameObjectWithTag("Player");
        // set the text of the canvas to step 0 text
        _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step0;
        // the button should say 'Next'
        _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
        //_nextButton.GetComponent<Button>().onClick.AddListener(buttonClicked);
        // get the position vectors from the game objects
        _lysosomeDownPosition = _lysosomeDown.transform.position;
        _lysosomeUpPosition = _lysosomeUp.transform.position;
        _vesicleStartPosition = _vesicleStart.transform.position;
        _vesicleEndPosition = _vesicleEnd.transform.position;
    }

    public void nextStep()
    {
        // if they finished
        if (_curStep == _totalSteps)
        {
            // restart it
            // set the text of the canvas to step 0 text
            _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step0;
            // the button should say 'Next'
            _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
            // current step is 0
            _curStep = 0;
        }
        else
        {
            _curStep++;
            displayCanvas();
        }
    }

    public void buttonClicked()
    {
        nextStep();
    }

    IEnumerator AnimatePlayer()
    {
        // hide all the wrist menus in case they are up
        _inputScript.hideWristMenus();

        _player.transform.position = _membraneFastTravelSocket.transform.position;
        
        // wait for 5 seconds while the player reads the text
        yield return new WaitForSeconds(5.0f);
        // start by slowly rotating the player 180 degrees
        Vector3 targetAngle = new Vector3(0f, 180f, 0f);
        Vector3 currentAngle = _player.transform.eulerAngles;

        _inputScript.setSnapTurn(true);
        int i = 0;
        float snap = 10f;
        int limit = (int)170.0f / (int)snap;
        while (i < limit)
        {
            _inputScript.rotatePlayer(snap);
            i++;
            yield return null;
        }
        // teleport player to be on top of the membrane socket
        Camera.main.transform.position = _membraneFastTravelSocket.transform.position;
        _inputScript.setSnapTurn(false);
        // now slowly bring up a lysosome
        // instantiate it
        GameObject lysosome = Instantiate(_lysosome, _lysosomeDownPosition, Quaternion.identity);
        // make it visible
        lysosome.SetActive(true);
        // move it up
        while ((lysosome.transform.position - _lysosomeUpPosition).sqrMagnitude > 5.0f)
        {
            lysosome.transform.position = Vector3.Lerp(lysosome.transform.position, _lysosomeUpPosition, Time.deltaTime * _animationSpeed/4);
            yield return null;
        }
        // once it's up, animate bring the vesicle in
        _vesicle = lysosome.transform.GetChild(2).gameObject;
        while ((_vesicle.transform.position - _vesicleEndPosition).sqrMagnitude > 1.0f)
        {
            _vesicle.transform.position = Vector3.Lerp(_vesicle.transform.position, _vesicleEndPosition, Time.deltaTime * _animationSpeed/2);
            yield return null;
        }
        // once the vesicle is on you, we want to move together with the vesicle back to vesicleStart
        while ((_vesicle.transform.position - _vesicleStartPosition).sqrMagnitude > 1.0f)
        {
            _vesicle.transform.position = Vector3.Lerp(_vesicle.transform.position, _vesicleStartPosition, Time.deltaTime * _animationSpeed/2);
            _player.transform.position = Vector3.Lerp(_player.transform.position, _vesicleStartPosition, Time.deltaTime * _animationSpeed/2);
            yield return null;
        }
    }

    public void displayCanvas()
    {
        switch (_curStep)
        {
            // welcome text
            case 0:
            {
                // set the text of the canvas to step 0 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step0;
                // the button should say 'Next'
                _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
                break;
            }

            // player just got frozen
            case 1:
            {
                // set the text of the canvas to step 0 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step1;
                // the button should dissappear
                _nextButton.SetActive(false);
                // freeze the player
                _inputScript.setCanMove(false);
                // animation start
                StartCoroutine(AnimatePlayer());
                break;
            }

            default:
            {
                break;
            }
        }
    }
}
