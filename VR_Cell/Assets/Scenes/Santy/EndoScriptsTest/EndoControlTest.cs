using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
/// Include the name space for TextMesh Pro
using TMPro;


public class EndoControlTest : MonoBehaviour
{
    // steo 0 is the welcome screen and empty table
    [Header("Organelles To Spawn")]
    public int _step = 0;
    private int _totalSteps = 9;
    public GameObject _protein;
    public GameObject _nucleolus;
    public GameObject _ribosome30;
    public GameObject _ribosome50;
    public GameObject _ribosomefull;
    public GameObject _mrna;
    public GameObject _glycoprotein;
    public GameObject _rougher;
    public GameObject _vesiclegp;
    public GameObject _golgi;


    // store the spawned objects in global variables
    private GameObject _proteinSpawned;
    private GameObject _nucleolusSpawned;
    private GameObject _ribosome30Spawned;
    private GameObject _ribosome50Spawned;
    private GameObject _ribosomefullSpawned;
    private GameObject _mrnaSpawned;
    private GameObject _glycoproteinSpawned;
    private GameObject _rougherSpawned;
    private GameObject _vesiclegpSpawned;
    private GameObject _spawnedGolgi;

    // variable to track if the animation should play
    private bool _playAnimation;

    // UI global variables
    [Header("UI Variables")]
    public GameObject _textArea;
    public GameObject _backButton;
    public GameObject _nextButton;
    public GameObject _backButtonText;
    public GameObject _nextButtonText;

    // UI panel text variables
    private string _panelText0 = "Step 0 - Welcome to the endomembrane process...!";
    private string _panelText1 = "Step 1 - Put the nucleolus and protein together to create the ribosome pieces!";
    private string _panelText2 = "Step 2 - Put the two ribosome pieces together to create a full ribosome!";
    private string _panelText3 = "Step 3 - Nice job putting the ribosome together! Press next to get an mRNA!";
    private string _panelText4 = "Step 4 - Put the mRNA and ribosome together to create a glycoprotein!";
    private string _panelText5 = "Step 5 - Nice you just created a glycoprotein...press next to interact with the roughER!";
    private string _panelText6 = "Step 5 - Place the glycoprotein through the ER to encapsulate it in a vesicle!";
    private string _panelText7 = "Now that you have a vesicle glycoprotein, press next to take it to the golgi!";
    private string _panelText8 = "Step 8 - Place the vesicle glycoprotein on the cis side of the golgi to continue";
    private string _panelText9 = "Step 9 - Watch as the glycoprotein gets absorbed by the golgi and travels to the trans side of the golgi, leaving in a vesicle to go to the cell membrane or something!";
    private string _startQuizText = "Quiz";

    // make the spawn locations global in case we need to change them
    private Vector3 _spawnLeft = new Vector3(0.0f, 1.3f, 0.99f);
    private Vector3 _spawnRight = new Vector3(2.50f, 1.3f, 0.97f);
    
    // global array to store all objects that are currently spawned so we can access them and disable them when we need to
    public List<GameObject> _spawnedObjects = new List<GameObject>();

    void Start()
    {
        // put welcome text on the panel 0
        _textArea.GetComponent<TextMeshProUGUI>().text = _panelText0;
        
        // hide the back button
        _backButton.SetActive(false);
        // make the next button visible
        _nextButton.SetActive(true);
        
        // spawn all of the organelles and add them to the array
        _proteinSpawned = Instantiate(_protein, _spawnLeft, Quaternion.identity);
        _spawnedObjects.Add(_proteinSpawned);
        _nucleolusSpawned = Instantiate(_nucleolus, _spawnRight, Quaternion.identity);
        _spawnedObjects.Add(_nucleolusSpawned);
        _ribosome30Spawned = Instantiate(_ribosome30, _spawnLeft, Quaternion.identity);
        _spawnedObjects.Add(_ribosome30Spawned);
        _ribosome50Spawned = Instantiate(_ribosome50, _spawnRight, Quaternion.identity);
        _spawnedObjects.Add(_ribosome50Spawned);
        _ribosomefullSpawned = Instantiate(_ribosomefull, _spawnLeft, Quaternion.identity);
        _spawnedObjects.Add(_ribosomefullSpawned);
        _mrnaSpawned = Instantiate(_mrna, _spawnRight, Quaternion.identity);
        _spawnedObjects.Add(_mrnaSpawned);
        _glycoproteinSpawned = Instantiate(_glycoprotein, _spawnLeft, Quaternion.identity);
        _spawnedObjects.Add(_glycoproteinSpawned);
        _rougherSpawned = Instantiate(_rougher, _spawnRight, Quaternion.identity);
        _spawnedObjects.Add(_rougherSpawned);
        _vesiclegpSpawned = Instantiate(_vesiclegp, _spawnRight, Quaternion.identity);
        _spawnedObjects.Add(_vesiclegpSpawned);
        _spawnedGolgi = Instantiate(_golgi, _spawnLeft, Quaternion.identity);
        _spawnedObjects.Add(_spawnedGolgi);
        // Rotate the golgi
        _spawnedGolgi.transform.Rotate(90.0f, 0.0f, -90.0f, Space.Self);

        // remove the (clone) from the name of the objects
        foreach(GameObject obj in _spawnedObjects)
        {
            obj.name = obj.name.Replace("(Clone)", "");
        }
        
        // make them all inactive
        _proteinSpawned.SetActive(false);
        _nucleolusSpawned.SetActive(false);
        _ribosome30Spawned.SetActive(false);
        _ribosome50Spawned.SetActive(false);
        _ribosomefullSpawned.SetActive(false);
        _mrnaSpawned.SetActive(false);
        _glycoproteinSpawned.SetActive(false);
        _rougherSpawned.SetActive(false);
        _vesiclegpSpawned.SetActive(false);
        _spawnedGolgi.SetActive(false);
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            prevStep();
        }
        if (Input.GetMouseButtonDown(1))
        {
            nextStep();
        }
    }
    
    // punction to move between all steps
    public void Process( bool isForward )
    {
        switch(_step)
        {
            // welcome screen
            // don't need back button
            // need next button
            case 0:
            {
                // only need to account for going backwards, since there is no step before this
                if (!isForward)
                {
                    // hide all "EndoProcess" game objects from step 1
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                    // we can change panel to screen 0 here
                    _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText0;
                    // hide the back button
                    _backButton.SetActive(false);
                    // make the next button visible
                    _nextButton.SetActive(true);
                }
                break;
            }

            // spawn the protein and nucleolus
            // dont need the next button because putting the objects together moves to step 2
            // putting the protein and nucleolus together should spawn the ribosome pieces
            case 1:
            {
                if (isForward)
                {
                    Debug.Log("Forward");
                    // show the back button if moving forward
                    _backButton.SetActive(true);
                    // hide the next button if moving forward
                    _nextButton.SetActive(false);
                }
                else
                {
                    Debug.Log("Backwards");
                    // hide all "EndoProcess" game objects from step 2
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                }
                // we can change panel to screen 1 here
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText1;

                // make protein and nucleolus visible
                _proteinSpawned.SetActive(true);
                _nucleolusSpawned.SetActive(true);
                break;
            }

            // the user just spawned the two ribosome pieces so we need to prompt them to put them together
            // still don't need a next button because putting the two ribosome pieces together moves to step 3
            case 2:
            {
                if (isForward)
                {
                    Debug.Log("Forward");
                    // hide all "EndoProcess" game objects from step 1
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Backwards");
                    // hide all "EndoProcess" game objects from step 3
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                    // hide the next button if moving backwards
                    _nextButton.SetActive(false);
                }
                // make ribosome30 and ribosome50 visible whether moving forward or backwards
                _ribosome30Spawned.SetActive(true);
                _ribosome50Spawned.SetActive(true);
                // we can change panel to screen 2 here
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText2;
                
                break;
            }
            
            // this panel will just say "nice you just created a full ribosome...press next to continue (to spawn the mRNA)"
            // we need a next button now
            case 3:
            {
                if (isForward)
                {
                    Debug.Log("Forward");
                    // hide all objects from step 2 if we are going forward
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Backwards");
                    // hide all objects from step 4 if we are going backwards
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                }
                // make the ribosome full visible whether moving forwards or backwards
                _ribosomefullSpawned.SetActive(true);
                // show the next button whether we are moving forward or backwards
                _nextButton.SetActive(true);
                // we can change panel to screen 3 here
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText3;
                break;
            }

            // spawn the mRNA
            // the panel should say "put the mRNA together with the ribosome to create a glycoprotein"
            // we don't need a next button here since putting the ribosome and mRNA together moves us forward
            case 4:
            {
                if (isForward)
                {
                    Debug.Log("Forward");
                }
                else
                {
                    Debug.Log("Backwards");
                    // hide all objects from step 5 if we are going backwards
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                    // make the ribosome full visible
                    _ribosomefullSpawned.SetActive(true);
                }
                // make the mRNA visible whether moving forward or backwards
                _mrnaSpawned.SetActive(true);
                // hide the next button whether we are moving forward or backwards
                _nextButton.SetActive(false);
                // we can change panel to screen 4 here
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText4;
                break;
            }

            // they just put the mRNA and ribosome together and now they have a glycoprotein
            // text should say "nice you just created a glycoprotein...press next to interact with the roughER!"
            // need next button
            case 5:
            {
                if (isForward)
                {
                    Debug.Log("Forward");
                    // hide all objects from step 4 if we are going forwards
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Backwards");
                    // hide all objects from step 6 if we are going backwards
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                }
                // make the glycoprotein visible whether moving forward or backwards
                _glycoproteinSpawned.SetActive(true);
                // show the next button whether we are moving forward or backwards
                _nextButton.SetActive(true);
                // we can change panel to screen 5 here
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText5;
                break;
            }

            // they just created a glycoprotein
            // text should say "place the glycoprotein in the Rough ER to continue"
            // don't need a next button since the interaction moves us forward
            case 6:
            {
                if (isForward)
                {
                    Debug.Log("Forward");
                }
                else
                {
                    Debug.Log("Backwards");
                    // hide all objects from step 6 if we are going backwards
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                    // make the glycoprotein visible
                    _glycoproteinSpawned.SetActive(true);
                }
                // make the roughER visible whether moving forward or backwards
                _rougherSpawned.SetActive(true);
                // show the next button whether we are moving forward or backwards
                _nextButton.SetActive(false);
                // we can change panel to screen 6 here
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText6;               
                break;
            }

            // the user just put together the roughER with the glycoprotein and now they have a vesicle glycoprotein
            // text should say "Now that you have a vesicle glycoprotein, press next to take it to the golgi!"
            // need next button
            case 7:
            {
                if (isForward)
                {
                    Debug.Log("Forward");
                    // hide all objects from step 6 if we are going forwards
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Backwards");
                    // hide all objects from step 6 if we are going backwards
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                }
                // make the vesicle glycoprotein visible whether moving forward or backwards
                _vesiclegpSpawned.SetActive(true);
                // need the next button whether we are moving forward or backwards
                _nextButton.SetActive(true); 
                // we can change panel to screen 7 here
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText7;
                break;
            }

            // the user just put together the roughER with the glycoprotein and now they have a vesicle glycoprotein
            // text should say "put the vesicle glycoprotein on the golgi !"
            // need to spawn a golgi
            // don't need a next button because the user needs to put the vesicle glycoprotein on the golgi to move forward
            case 8:
            {
                if (isForward)
                {
                    Debug.Log("Forward");
                    // hide all objects from step 7 if we are going forwards
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        item.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("Backwards");
                    // stop the animation
                    _playAnimation = false;
                    // change next button text to start review
                    _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "Next";
                }
                // make the vesicle glycoprotein visible whether moving forward or backwards
                _vesiclegpSpawned.SetActive(true);
                // make the golgi visible
                _spawnedGolgi.SetActive(true);
                // need to hide the next button whether we are moving forward or backwards
                _nextButton.SetActive(false); 
                // we can change panel to screen 8 here
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText8;
                break;
            }

            // They just put the vesicle glycoprotein on the golgi and now the animation should play indefinitely
            case 9:
            {
                // since this is the last step we don't need to account for going backwards
                Debug.Log("Forward");  
                // hide the vesiclegp
                _vesiclegpSpawned.SetActive(false);
                // show the next button
                _nextButton.SetActive(true);
                // we can change panel to screen 9 here
                _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _panelText9;
                
                // change next button text to "start Review" or somethinig similar
                _nextButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = _startQuizText;

                // play the animation on loop
                _playAnimation = true;
                StartCoroutine(AnimationLoop());
                break;
            }

            default:
            {
                Debug.Log("Can't move any more forward or backwards");
                break;
            }
        }
    }

    // animationloop coroutine
    IEnumerator AnimationLoop()
    {
        while (_playAnimation == true)
        {
            // run the animation for testing purposes
            _spawnedGolgi.GetComponent<Animation>().StartAnimation();
            yield return null;
        }
    }

    public void nextStep()
    {
        
        // if we press the next button while on the last steep then we want to start the quiz
        if (_step == _totalSteps)
        {
            Debug.Log("Start Quiz"); 
            _playAnimation = false;   
            return;
        }
        _step++;
        Process(true);
    }

    public void prevStep()
    {
        if (_step == 0)
            return;
        _step--;
        Process(false);
    }
}
