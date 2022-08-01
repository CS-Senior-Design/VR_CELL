using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RibosomeState : MonoBehaviour
{
    public GameObject fullRibosome;
    private bool currentlyGrabbed = false;
    private XRSocketInteractor _socket = null;

    void Awake()
    {
        // if it's the ribosome half with the socket, we want to set a listener for it
        if(transform.childCount > 0)
        {  
            _socket = transform.GetChild(0).GetComponent<XRSocketInteractor>();
            _socket.selectEntered.AddListener(SocketFilled);
            // make the socket inactive so they don't immediately connect on spawn
            _socket.socketActive = false;
        }
    }

    // if the ribosome's socket gets filled
    public void SocketFilled(SelectEnterEventArgs obj)
    {
        // destroy both the ribosome pieces and replace them with a full ribosome
        Vector3 socketPosition = _socket.transform.position;
        Destroy(gameObject);
        Destroy(obj.interactableObject.transform.gameObject);
        GameObject ribosome = Instantiate(fullRibosome, socketPosition, Quaternion.identity);
        ribosome.transform.localScale = new Vector3(25f, 25f, 25f);
        ribosome.SetActive(true);
    }

    public bool isGrabbed()
    {
        return currentlyGrabbed;
    }

    public void setGrabbed(bool value)
    {
        if (_socket != null)
            _socket.socketActive = true;
        currentlyGrabbed = value;
    }

    public void setRibosomeScale()
    {
        transform.localScale = new Vector3(25f, 25f, 25f);
    }
}
