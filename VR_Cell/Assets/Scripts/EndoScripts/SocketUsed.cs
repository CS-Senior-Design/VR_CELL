using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketUsed : MonoBehaviour
{
    public void UpdateStep()
    {
        EndoControl endoScript = GameObject.FindGameObjectWithTag("EndoController").GetComponent<EndoControl>();
        // get the object with the EndoController script
        
        // if we don't find the script then print the reason
        if (endoScript == null)
        {
            Debug.Log("Object with EndoController script not tagged with endocontroller tag!!");
        }
        // if we find the script then depending on what step we are on we may want to move to the next panel from here
        else
        {
            switch (endoScript._step)
            {
                case 1: // putting the nucleolus and protein together -> move to step 2
                    endoScript.nextStep();
                    break;
                case 2: // combining two ribosome parts together -> move to step 3
                    endoScript.nextStep();
                    break;
                case 4: // putting the mRNA and ribosome together -> move to step 5
                    endoScript.nextStep();
                    break;
                case 6: // putting the glycoprotein on the RER -> move to step 7
                    endoScript.nextStep();
                    break;
                case 8: // putting the vesicle glycoprotein on the Golgi -> move to step 8
                    endoScript.nextStep();
                    break;
            }
        }
    }
}
