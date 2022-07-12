using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class EndoControl : MonoBehaviour
{
    // steo 0 is the welcome screen and empty table
    public int _step = 0;
    private int _totalSteps = 3;
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
    public GameObject _golgicis;
    public GameObject _golgitrans;
    public GameObject _outgoingvesicle;
    
    // item2.transform.localScale += new Vector3(100,100,100)

    // Start is called before the first frame update
    public void Process( bool isForward)
    {
        switch(_step)
        {
            case 0:
            {
                // don't have to spawn anything
                // need to destroy objects from step 1 if they come from step 1
                if (!isForward)
                {
                    // destroy step 1 objects
                    Debug.Log("destroy step1 1 objects");
                    // we can change panel to screen 0 here
                }
                Debug.Log("work0");
                break;
            }

            case 1:
            {
                if (isForward)
                {
                    // don't nede to destroy anything
                    Debug.Log("dont destroy anything");
                    // we can change panel to screen 1 here
                }
                else
                {
                    // destroy step 2 stuf
                    Debug.Log("destroy step 2 stuff");
                }
                // spawn step 1 stuff (protein and nucleolus)
                // protein
                GameObject protein = Instantiate(
                    _protein,
                    new Vector3(0.07f,1.3f,0.99f),
                    Quaternion.identity
                );
                // nucleolus
                GameObject nucleolus = Instantiate(
                    _nucleolus,
                    new Vector3(0.60f,1.3f,0.97f),
                    Quaternion.identity
                );

                // // add socket interactor to one of them
                // XRSocketInteractor proteinSocket = protein.AddComponent<XRSocketInteractor>() as XRSocketInteractor;

                // // add the organelle interaction script to socket interactor
                // OrganelleInteractions script = proteinSocket.gameObject.AddComponent<OrganelleInteractions>() as OrganelleInteractions;

                // // set the variables to spawn
                // script._spawnItem1 = _protein;

                // next button should be greyed out here

                // When they connect the two objects above the UI should move forward with a greyed out next button that doest click

                break;
            }

            case 2:
            {
                Debug.Log("work2");
                break;
            }

            case 3:
            {
                Debug.Log("work3");
                break;
            }

            default:
            {
                Debug.Log("nonono");
                break;
            }
        }
    }

    public void nextStep()
    {
        if (_step == _totalSteps)
            return;
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
