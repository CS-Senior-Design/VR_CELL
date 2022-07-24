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

    // get the text gameobjects so we can change the texts of the UI
    public GameObject _textArea;
    public GameObject _ribosomeText;
    public GameObject _button;
    public GameObject _buttonText;

    // text for each step
    private string _stepCheat = "You thought I wouldn't notice that didn't you? Well I did, and now you have to restart!";
    private string _step0 = "Hi, I am the Rough Endoplasmic Retirculum! I finish folding proteins and package them out to the Golgi! Press next to help me!";
    private string _step1 = "Thanks! Go to the nucleolus and get me a fresh ribosome! Put it over by that marker on your left!";
    private string _step2 = "Thanks! Now take this packaged protein over to the golgi for shipping!";
    private string _ribosomeTextString = "Place Ribosome Here";

    // animation variables
    private float _animationSpeed = 0.8f;
    public GameObject _mRNA;
    public GameObject _packagedProtein;
    public GameObject _packagedProteinGrab;
    private GameObject _mrnaSpawned;
    private GameObject _packagedProteinSpawned;
    private Vector3 _startingPositionRNA;
    private Vector3 _endingPositionRNA;
    private Vector3 _startingPositionProtein;
    private Vector3 _endingPositionProtein;


    // Start is called before the first frame update
    void Awake()
    {
        // set the text of the canvas to step 0 text
        _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step0;
        // the button should say 'Next'
        _buttonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
        // add the listener to the socket for when something gets placed inside
        _socket.selectEntered.AddListener(SelectedObject);
        // the socket needs to be inactive
        _socket.gameObject.SetActive(false);
        // make the ribosome text inactive
        _ribosomeText.SetActive(false);
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
            _buttonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
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
            // set all the starting and ending positions based on the socket position
            _startingPositionRNA = new Vector3(ribosomePosition.x - 1.5f, ribosomePosition.y, ribosomePosition.z);
            _endingPositionRNA = new Vector3(ribosomePosition.x + 0.08f, ribosomePosition.y, ribosomePosition.z);
            _startingPositionProtein = new Vector3(ribosomePosition.x + 0.1f, ribosomePosition.y, ribosomePosition.z - 1.0f);
            _endingPositionProtein = new Vector3(ribosomePosition.x - 1.0f, ribosomePosition.y, ribosomePosition.z - 1.0f);
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
        _mrnaSpawned.SetActive(true);
        while ((_mrnaSpawned.transform.position - _endingPositionRNA).sqrMagnitude > 0.008f)
        {
            _mrnaSpawned.transform.position = Vector3.Lerp(_mrnaSpawned.transform.position, _endingPositionRNA, Time.deltaTime * _animationSpeed);
            yield return null;
        }
        // once the mRNA is done we can destroy it
        Destroy(_mrnaSpawned);
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
                _buttonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
                // the socket needs to be inactive
                _socket.gameObject.SetActive(false);
                // make the ribosome text inactive
                _ribosomeText.SetActive(false);
                break;
            }

            // go get ribosome text
            case 1:
            {
                // set the text of the canvas to step 1 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step1;
                // hide the next button because they need to put the ribosome in place
                _button.SetActive(false);
                // the socket needs to be active
                _socket.gameObject.SetActive(true);
                // show where to get the ribosome
                _ribosomeText.GetComponent<TMPro.TextMeshProUGUI>().text = _ribosomeTextString;
                // make the ribosome text active
                _ribosomeText.SetActive(true);
                // display the roughER UI
                break;
            }

            // Animation and Go send this packaged protein to the Golgi text
            case 2: 
            {
                // the socket needs to be inactive
                _socket.gameObject.SetActive(false);
                // make the ribosome text inactive
                _ribosomeText.SetActive(false);
                // spawn the mrna and protein
                _mrnaSpawned = Instantiate(_mRNA, _startingPositionRNA, Quaternion.identity);
                _packagedProteinSpawned = Instantiate(_packagedProtein, _startingPositionProtein, Quaternion.identity);
                // hide them both to begin
                _mrnaSpawned.SetActive(false);
                _packagedProteinSpawned.SetActive(false);
                // animation of mRNA going through the ribosome and then a packaged protein coming out
                StartCoroutine(ERAnimation());
                // set the text of the canvas to step 2 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _step2;
                // change the button to say 'restart'
                _buttonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Restart";
                // show the button again
                _button.SetActive(true);
                // display the roughER UI
                break;
            }

            // they tried to cheat and placed something else in the socket
            case -1:
            {
                // make the ribosome text inactive
                _ribosomeText.SetActive(false);
                // set the text of the canvas to step 0 text
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _stepCheat;
                _buttonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Try Again";
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
}
