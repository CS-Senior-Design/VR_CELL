using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ERQuest : MonoBehaviour
{
    // current step of the process
    private int _curStep = 0;
    // total number of steps in the process
    private int _totalSteps = 2;
    
    // get the socket so we can easily activate and deactivate it and check what's inside it
    public XRSocketInteractor _socket;
    // variable to store the selected object
    private GameObject _selectedObject;

    public GameObject _arrow;

    // get the text gameobjects so we can change the texts of the UI
    public GameObject _textArea;
    public GameObject _nextButton;
    public GameObject _nextButtonText;

    // text for each step
    private string _stepCheat = "You thought I wouldn't notice that didn't you? Well I did, and now you have to restart!";
    private string _step0 = "My function is to produce, fold, and check for the quality of new proteins!";
    private string _step1 = "Bring me a fresh ribosome and I'll make you a new protein!\n\nPlace it here on the wall ->";
    private string _step2 = "Thanks! Now grab the protein and take it to the golgi!";


    // store the original location of the mRNA
    private Vector3 _mRNAOriginalLocation;

    public GameObject _targetObjectInside;
    public GameObject _targetObjectOutside;
    // animation variables
    private float _animationSpeed = 0.8f;
    public GameObject _mRNA;
    public GameObject _packagedProtein;
    public GameObject _packagedProteinGrab;
    private GameObject _packagedProteinSpawned;
    private Vector3 _startingPositionRNA;
    private Vector3 _endingPositionRNA;
    private Vector3 _startingPositionProtein;
    private Vector3 _endingPositionProtein;


    // Start is called before the first frame update
    void Awake()
    {
        // store the original location of the mRNA
        _mRNAOriginalLocation = _mRNA.transform.position;
        // set the text of the canvas to step 0 text
        _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step0;
        // the button should say 'Next'
        _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
        // add the listener to the socket for when something gets placed inside
        _socket.selectEntered.AddListener(SelectedObject);
        // the socket needs to be inactive
        _socket.gameObject.SetActive(false);
        hideArrow();
    }

    public void hideArrow()
    {
        _arrow.SetActive(false);
    }

    public void showArrow()
    {
        _arrow.SetActive(true);
    }

    public void nextStep()
    {
        // if they finished
        if (_curStep == _totalSteps)
        {
            Debug.Log("finished ER quest");
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

    public int getCurStep()
    {
        return _curStep;
    }

    public void cheatAttempt()
    {
        _curStep = -1;
        displayCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            nextStep();
        }
    }

    // when something gets placed in the socket
    public void SelectedObject(SelectEnterEventArgs args)
    {
        // get the name of the inserted item
        string name = args.interactableObject.transform.name;
        // if they placed a ribosome in the socket
        if (name.Contains("ribosome") == true || name.Contains("Ribosome") == true)
        {
            Debug.Log("Ribosome placed in the socket");
            // store the selected object
            _selectedObject = args.interactableObject.transform.gameObject;
            // get the position of the ribosome
            Vector3 ribosomePosition = args.interactableObject.transform.position;
            // make sure the empty game objects are active
            _targetObjectOutside.SetActive(true);
            // set all the starting and ending positions based on the socket position
            _startingPositionRNA = _mRNA.transform.position;
            _endingPositionRNA = _targetObjectInside.transform.position;
            _startingPositionProtein = _targetObjectInside.transform.position;
            _endingPositionProtein = _targetObjectOutside.transform.position;
            nextStep();
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

    IEnumerator ERAnimation()
    {
        // set the spawned mrna to active
        _mRNA.SetActive(true);
        while ((_mRNA.transform.position - _endingPositionRNA).sqrMagnitude > 0.008f)
        {
            _mRNA.transform.position = Vector3.Lerp(_mRNA.transform.position, _endingPositionRNA, Time.deltaTime * _animationSpeed);
            yield return null;
        }
        // put the mRNA back to the original location
        _mRNA.transform.position = _mRNAOriginalLocation;
        // start the protein animation
        StartCoroutine(ProteinAnimation());
    }

    IEnumerator ProteinAnimation()
    {
        // set the spawned protein to active
        _packagedProteinSpawned.SetActive(true);
        while ((_packagedProteinSpawned.transform.position - _endingPositionProtein).sqrMagnitude > 0.008f)
        {
            _packagedProteinSpawned.transform.position = Vector3.Lerp(_packagedProteinSpawned.transform.position, _endingPositionProtein, Time.deltaTime * _animationSpeed);
            yield return null;
        }
        // once the protein is done we can destroy it and spawn a grabbable version in its place
        Vector3 proteinPosition = _packagedProteinSpawned.transform.position;
        Destroy(_packagedProteinSpawned);
        GameObject protein = Instantiate(_packagedProteinGrab, proteinPosition, Quaternion.identity);
        // make it visible
        protein.SetActive(true);
        // hide the empty game object
        _targetObjectOutside.SetActive(false);
        // destroy the ribosome
        Destroy(_selectedObject);
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
                hideArrow();
                break;
            }

            // go get ribosome text
            case 1:
            {
                // set the text of the canvas to step 1 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step1;
                // hide the next button because they need to put the ribosome in place
                _nextButton.SetActive(false);
                // the socket needs to be active
                _socket.gameObject.SetActive(true);
                showArrow();
                break;
            }

            // Animation and Go send this packaged protein to the Golgi text
            case 2: 
            {
                // the socket needs to be inactive
                _socket.gameObject.SetActive(false);
                // spawn the mrna and protein
                _packagedProteinSpawned = Instantiate(_packagedProtein, _startingPositionProtein, Quaternion.identity);
                // hide them both to begin
                _mRNA.SetActive(false);
                _packagedProteinSpawned.SetActive(false);
                // animation of mRNA going through the ribosome and then a packaged protein coming out
                StartCoroutine(ERAnimation());
                // set the text of the canvas to step 2 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step2;
                // change the button to say 'restart'
                _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Restart";
                // show the button again
                _nextButton.SetActive(true);
                hideArrow();
                // display the roughER UI
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
                hideArrow();
                break;
            }

            default:
            {
                break;
            }
        }
    }
}
