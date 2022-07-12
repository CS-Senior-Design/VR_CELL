using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

public class OrganelleInteractions : MonoBehaviour
{
    public GameObject _spawnItem1 = null;
    public GameObject _spawnItem2 = null;

    void Awake()
    {
        XRSocketInteractor socket = gameObject.GetComponent<XRSocketInteractor>();
        socket.onSelectEntered.AddListener(Interaction);
    }

    public void Interaction(XRBaseInteractable obj)
    {
        Debug.Log("herererere");
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("EndoProcess"))
        {
            Destroy(item);
        }
        // if the object is a protein then we spawn ribosome halves
        if (_spawnItem1)
        {
            GameObject ribosome30 = Instantiate(
                _spawnItem1,
                new Vector3(0.07f,1.3f,0.99f),
                Quaternion.identity
            );
        }
        if (_spawnItem2)
        {
            // nucleolus
            GameObject ribosome50 = Instantiate(
                _spawnItem2,
                new Vector3(0.60f,1.3f,0.97f),
                Quaternion.identity
            );
        }  
        // once we spawn the object, we should destroy the old ones
        // foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        // {
        //     // when we get to the player, we access the endocontrol script
        //     EndoControl endoScript = player.GetComponent<EndoControl>();
        //     // now we delete everything in the spawnedObjects array in endo control
        //     foreach (GameObject item in endoScript.spawnedObjects.Reverse<GameObject>())
        //     {
        //         GameObject temp = item;
        //         endoScript.spawnedObjects.Remove(item);
        //         temp.SetActive(false);
        //         Debug.Log("Destroying the item");
        //     }
        //     // now add the newly spawned items into the array so they dont get destroyed
        //     if (_spawnItem1)
        //         endoScript.spawnedObjects.Add(_spawnItem1);
        //     if (_spawnItem2)
        //         endoScript.spawnedObjects.Add(_spawnItem2);
        // }   
    }
}
