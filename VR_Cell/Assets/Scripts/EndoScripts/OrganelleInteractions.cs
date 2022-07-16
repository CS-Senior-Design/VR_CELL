using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

// This scrip will be placed on the socket component of the objects that have a socket
public class OrganelleInteractions : MonoBehaviour
{
    void Awake()
    {
        // get the socket component of the object
        XRSocketInteractor socket = gameObject.GetComponent<XRSocketInteractor>();
        // call the Interaction function when an object gets placed in the socket
        socket.onSelectEntered.AddListener(Interaction);
    }

    public void Interaction(XRBaseInteractable obj)
    {
        EndoControl endoScript = null;
        // get the object with the EndoController script
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoController"))
        {
            endoScript = item.GetComponent<EndoControl>();
        }   
        // if we don't find the script then print the reason
        if (endoScript == null)
        {
            Debug.Log("Object with EndoController script not tagged with endocontroller tag!!");
        }
        // if we find the script then depending on what step we are on we may want to move to the next panel from here
        else
        {           
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
            else if (endoScript._step == 8)
            {
                endoScript.nextStep();
            }
        }
    }
}
