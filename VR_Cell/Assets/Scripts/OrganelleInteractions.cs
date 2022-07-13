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
        // get the socket component
        XRSocketInteractor socket = gameObject.GetComponent<XRSocketInteractor>();
        // call the Interaction function when an object gets placed in the socket
        socket.onSelectEntered.AddListener(Interaction);
    }

    public void Interaction(XRBaseInteractable obj)
    {

        // if this is the Golgi interaction, we want to do the animation of the enclosed glycoprotein going across the golgi
        // we will know if this is that interaction because _spawnItem1 will be a tavle with a "golge" tag (literally golge)
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("golge"))
        {
            if (_spawnItem1 == item)
            {
                return;
            }
        }

        // look for any objects in the scene with the "EndoProcess" tag and destroy them
        // this is important so that whenever an interaction happens, the two objects that interact will disappear leaving only the new item/s
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
        {
            Destroy(item);
        }
        // the if statements are only to ensure we never try to instantiate a null game object
        if (_spawnItem1)
        {
            GameObject item1 = Instantiate(
                _spawnItem1,
                new Vector3(0.07f,1.3f,0.99f),
                Quaternion.identity
            );
        }
        if (_spawnItem2)
        {
            // spawn further to the right than the previosu object
            GameObject item2 = Instantiate(
                _spawnItem2,
                new Vector3(0.60f,1.3f,0.97f),
                Quaternion.identity
            );
        }    
    }
}
