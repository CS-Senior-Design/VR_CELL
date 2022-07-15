using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

// This scrip will be placed on the socket component of the objects that have a socket
public class OrganelleInteractions : MonoBehaviour
{
    public GameObject _spawnItem1 = null;
    public GameObject _spawnItem2 = null;

    void Awake()
    {
        XRSocketInteractor socket = gameObject.GetComponent<XRSocketInteractor>();
        // call the Interaction function when an object gets placed in the socket
        socket.onSelectEntered.AddListener(Interaction);
    }

    public void Interaction(XRBaseInteractable obj)
    {

        // // if _spawnItem1 is null then we are on the golgi socket and the obj is the glycoprotein with vesicle
        // // we need to just hide the vesicle
        // if (_spawnItem1 == null)
        // {
        //     // hide the glycoprotein object
        //     obj.gameObject.SetActive(false);
        //     return;
        // }

        // // look for any objects in the scene with the "EndoProcess" tag and hide them
        // // this is important so that whenever an interaction happens, the two objects that interact will disappear leaving only the new item/s
        // foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
        // {
        //     item.SetActive(false);
        // }

        EndoControlTest endoScript = null;
        // get the object with the EndoController script
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoController"))
        {
            endoScript = item.GetComponent<EndoControlTest>();
        }   
        // if we don't find the script then print the reason
        if (endoScript == null)
        {
            Debug.Log("Object with EndoController script not tagged with endocontroller tag!!");
        }
        // if we find the script then depending on what step we are on we may want to move to the next panel from here
        else
        {
            // // use the array of spawned items to find the correct organelle(s) to set active
            // foreach(GameObject item in endoScript._spawnedObjects)
            // {
            //     if (_spawnItem1 != null && item.name == _spawnItem1.name)
            //     {
            //         item.SetActive(true);
            //     }
            //     if (_spawnItem2 != null && item.name == _spawnItem2.name)
            //     {
            //         item.SetActive(true);
            //     }
            // }
            
            // if this interaction was putting the nucleolus and protein together then move to step 2
            if (endoScript._step == 1)
            {
                endoScript.nextStep();
            }
            // if this interaction was putting the two ribosomes together then move to step 3
            else if (endoScript._step == 2)
            {
                endoScript.nextStep();
            }
            // if this interaction was putting the mRNA and ribosome together then move to step 5
            else if (endoScript._step == 4)
            {
                endoScript.nextStep();
            }
            // if this interaction is putting the glycoprotein on the Rough ER then move to step 7
            else if (endoScript._step == 6)
            {
                endoScript.nextStep();
            }
            // if this interaction is putting the vesicle glycoprotein on the Golgi then move to step 8
            else if (endoScript._step == 7)
            {
                endoScript.nextStep();
            }
        }
    }
}
