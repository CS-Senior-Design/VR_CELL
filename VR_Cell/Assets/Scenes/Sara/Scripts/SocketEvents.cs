using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketEvents : MonoBehaviour
{
    public GameObject item = null;
    // get the gameobject that has the wristMenu script attached
    // public GameObject objectWithInventoryManager;

    void Awake()
    {
        XRSocketInteractor socket = gameObject.GetComponent<XRSocketInteractor>();
        socket.onSelectEntered.AddListener(SelectedObject);
        // socket.onSelectExit.AddListener(ColorChange);
    }

    public void SelectedObject(XRBaseInteractable obj)
    {
        Debug.Log(obj.name);
        item = obj.gameObject;

        // objectWithInventoryManager.GetComponent<InventoryManager>().organelles.Add(obj.gameObject);
        // Debug.Log(objectWithInventoryManager.GetComponent<InventoryManager>().organelles);
        
    }
    

    public void hide()
    {
        if (item != null)
            item.SetActive(false);
    }

    public void show()
    {
        if (item != null)
            item.SetActive(true);
    }
}
