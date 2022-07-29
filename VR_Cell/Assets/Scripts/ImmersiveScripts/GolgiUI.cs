using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GolgiUI : MonoBehaviour
{
    private int _curStep = 0;
    private int _totalSteps = 4;

    private float _animationSpeed = 0.8f;

    public GameObject _textArea;
    public GameObject _nextButton;
    public GameObject _nextButtonText;
    public GameObject _arrow;
    public XRSocketInteractor _socket;
    public GameObject _targetObjectInside;
    public GameObject _targetObjectOutside;
    public GameObject _proteinGPanimation;
    public GameObject _proteinSpawned;
    public GameObject _vesicle;
    public GameObject _vesicleGrab;
    private GameObject _vesicleSpawned;
    private GameObject _selectedObject;
    private Vector3 _startingPositionProtein;
    private Vector3 _endingPositionProtein;
    private Vector3 _startingPositionVesicle;
    private Vector3 _endingPositionVesicle;
    private string _stepCheat = "You thought I wouldn't notice that didn't you? Well I did, and now you have to restart!";
    private string _step0 = "My job is to package and sort proteins and lipids made in the ER so that they are ready for their final destinations!";
    private string _step1 = "I can send these packages to the cell membrane, lysosomes, or just to be secreted out of the cell!";
    private string _step2 = "Get me a protein from the rough ER, and I can package it for you!\nJust place it over on your right!";
    private string _step3 = "Take this to the cell membrane!\nI'll be waiting for your next package!";
    // Start is called before the first frame update
    void Start()
    {
        hideArrow();
        // set the text of the canvas to step 0 text
        _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step0;
        // the button should say 'Next'
        _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
        // add the listener to the socket for when something gets placed inside
        _socket.selectEntered.AddListener(SelectedObject);
        // the socket needs to be inactive
        _socket.gameObject.SetActive(false);
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
                // the socket needs to be inactive
                _socket.gameObject.SetActive(false);
                break;
            }

            case 1:
            {
                // set the text of the canvas to step 0 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step1;
                // the button should say 'Next'
                _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
                // the socket needs to be inactive
                _socket.gameObject.SetActive(false);
                break;
            }

            // go get protein text
            case 2:
            {
                // set the text of the canvas to step 1 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step2;
                // hide the next button because they need to put the ribosome in place
                _nextButton.SetActive(false);
                // the socket needs to be active
                _socket.gameObject.SetActive(true);
                // show the arrow
                showArrow();
                break;
            }

            // Animation and Go send this vesicle to the membrane
            case 3: 
            {
                // the socket needs to be inactive
                //_socket.gameObject.SetActive(false);
                // spawn the animation protein
                _proteinSpawned = Instantiate(_proteinGPanimation, _startingPositionProtein, Quaternion.identity);
                // spawn the mrna and protein
                _vesicleSpawned = Instantiate(_vesicle, _startingPositionVesicle, Quaternion.identity);

                // hide it to begin with
                _vesicleSpawned.SetActive(false);
                // animation of mRNA going through the ribosome and then a packaged protein coming out
                StartCoroutine(GolgiAnimation());
                // set the text of the canvas to step 3 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step3;
                // change the button to say 'restart'
                _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Restart";
                // show the button again
                _nextButton.SetActive(true);
                break;
            }

            // they tried to cheat and placed something else in the socket
            case -1:
            {
                // set the text of the canvas to step 0 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _stepCheat;
                _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Try Again";
                // the socket needs to be inactive
                _socket.gameObject.SetActive(false);
                break;
            }

            default:
            {
                break;
            }
        }
    }

    IEnumerator GolgiAnimation()
    {
        Debug.Log("before while");
        while ((_proteinSpawned.transform.position - _endingPositionProtein).sqrMagnitude > 0.008f)
        {
            Debug.Log("Moving the protein into the golgi.");
            _proteinSpawned.transform.position = Vector3.Lerp(_proteinSpawned.transform.position, _endingPositionProtein, Time.deltaTime * _animationSpeed);
            yield return null;
        }
        Debug.Log("after while");
        // destroy the protein
        Destroy(_proteinSpawned);
        // start the vesicle animation
        StartCoroutine(VesicleAnimation());
    }

    IEnumerator VesicleAnimation()
    {
        // set the spawned protein to active
        _vesicleSpawned.SetActive(true);
        while ((_vesicleSpawned.transform.position - _endingPositionVesicle).sqrMagnitude > 0.008f)
        {
            Debug.Log("Moving the vesicle out of the golgi.");
            _vesicleSpawned.transform.position = Vector3.Lerp(_vesicleSpawned.transform.position, _endingPositionVesicle, Time.deltaTime * _animationSpeed);
            yield return null;
        }
        // once the protein is done we can destroy it and spawn a grabbable version in its place
        Vector3 vesiclePosition = _vesicleSpawned.transform.position;
        Destroy(_vesicleSpawned);
        GameObject vesicle = Instantiate(_vesicleGrab, vesiclePosition, Quaternion.identity);
        // make it visible
        vesicle.SetActive(true);
    }

    public void nextStep()
    {
        // if they finished
        if (_curStep == _totalSteps)
        {
            Debug.Log("finished Golgi portion");
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

    IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // destroy the selected object
        Destroy(_selectedObject);
        // hide the socket
        _socket.gameObject.SetActive(false);
        nextStep();
    }

    // when something gets placed in the socket
    public void SelectedObject(SelectEnterEventArgs args)
    {
        // hide the arrow
        hideArrow();
        // get the name of the inserted item
        string name = args.interactableObject.transform.name;
        // if they placed a ribosome in the socket
        if (name.Contains("vesicle") == true || name.Contains("Vesicle") == true)
        {
            Debug.Log("Vesicle GP placed in the socket");
            // store the selected object
            _selectedObject = args.interactableObject.transform.gameObject;
            // snap the protein to the socket
            _selectedObject.transform.position = _socket.transform.position;
            // get the position of the protein
            Vector3 proteinPosition = args.interactableObject.transform.position;
            // hide the socket
            //_socket.gameObject.SetActive(false);
    
            // set all the starting and ending positions based on the socket position
            _startingPositionProtein = proteinPosition;
            _endingPositionProtein = _targetObjectInside.transform.position;
            _startingPositionVesicle = _targetObjectInside.transform.position;
            _endingPositionVesicle = _targetObjectOutside.transform.position;

            // wait 1 second before moving on
            StartCoroutine(WaitForSeconds(1.0f));
        }
        // if they tried to put something else in the socket
        else
        {
            Debug.Log("You cheating scum");
            // destroy the added object
            Destroy(args.interactableObject.transform.gameObject);
            cheatAttempt();
        }        
    }

    public void cheatAttempt()
    {
        _curStep = -1;
        displayCanvas();
    }

    public void hideArrow()
    {
        _arrow.SetActive(false);
    }

    public void showArrow()
    {
        _arrow.SetActive(true);
    }
}
