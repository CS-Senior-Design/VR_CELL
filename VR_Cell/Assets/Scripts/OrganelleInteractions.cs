using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
    }
}
