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
    public GameObject _outgoingvesicle;

    // global array to store all objects that are currently spawned
    public List<GameObject> spawnedObjects = new List<GameObject>();
    
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
                    // destroy all "EndoProcess" game objects from step 1
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        Destroy(item);
                    }
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
                    Debug.Log("Change Panel forward");
                    // we can change panel to screen 1 here
                }
                else
                {
                    // destroy all "EndoProcess" game objects from step 2
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        Destroy(item);
                    }
                }
                // spawn step 1 stuff (protein and nucleolus)
                // protein
                GameObject protein = Instantiate(
                    _protein,
                    new Vector3(0.07f,1.3f,0.99f),
                    Quaternion.identity
                );
                // add it to the spawnedObjects array
                spawnedObjects.Add(protein);
                // nucleolus
                GameObject nucleolus = Instantiate(
                    _nucleolus,
                    new Vector3(0.60f,1.3f,0.97f),
                    Quaternion.identity
                );
                spawnedObjects.Add(nucleolus);

                // next button should be greyed out here

                // When they connect the two objects above the UI should move forward with a greyed out next button that doest click

                break;
            }

            // need an mRNA to connect to the full ribosome that is already spawned
            case 2:
            {
                if (isForward)
                {
                    Debug.Log("Change panel forward");
                }
                else
                {
                    // destroy all "EndoProcess" game objects from step 3
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        Destroy(item);
                    }
                }
                // spawn mRNA
                GameObject mrna = Instantiate(
                    _mrna,
                    new Vector3(0.60f,1.3f,0.97f),
                    Quaternion.identity
                );
                break;
            }

            // spawn a rough ER
            case 3:
            {
                if (isForward)
                {
                    Debug.Log("Change panel forward");
                }
                else
                {
                    // destroy all "EndoProcess" game objects from step 4
                    foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
                    {
                        Destroy(item);
                    }
                }
                // rough ER
                GameObject rougher = Instantiate(
                    _rougher,
                    new Vector3(0.60f,1.3f,0.97f),
                    Quaternion.identity
                );
                break;
            }

            // spwn golgi
            
            case 4:
            {
                if (isForward)
                {
                    Debug.Log("Change panel forward");
                }
                // golgi
                GameObject golgi = Instantiate(
                    _golgi,
                    new Vector3(0.60f,1.3f,0.97f),
                    Quaternion.identity
                );
                break;
            }

            // Final Panel telling the person they're finished
            case 5:
            {
                Debug.Log("You're done");
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
