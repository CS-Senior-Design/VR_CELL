using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketEvents : MonoBehaviour
{
    public GameObject item = null;
    private Vector3 initialScale;
    //public GameObject inventory;
    
    // get the gameobject that has the wristMenu script attached
    // public GameObject objectWithInventoryManager;

    void Awake()
    {
        XRSocketInteractor socket = gameObject.GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(SelectedObject);
        socket.selectExited.AddListener(unselected);
        GameObject.FindGameObjectWithTag("inventoryUI").SetActive(false);
        // socket.onSelectExit.AddListener(ColorChange);
    }

    public void SelectedObject(SelectEnterEventArgs obj)
    {
        Debug.Log(obj.interactableObject.transform.name);
        item = obj.interactableObject.transform.gameObject;

        // store the objects initial scale
        initialScale = item.transform.localScale;

        // scale the item down so it fits better
        item.transform.localScale = new Vector3(1, 1, 1);

        // objectWithInventoryManager.GetComponent<InventoryManager>().organelles.Add(obj.gameObject);
        // Debug.Log(objectWithInventoryManager.GetComponent<InventoryManager>().organelles);
        
    }

    public void unselected(SelectExitEventArgs obj)
    {
        Debug.Log("Objecte unselected");
        Debug.Log(obj.interactableObject.transform.name);
        item.transform.localScale = initialScale;
        // only set the object that just came out of the socket to null if we took it out manually 
        if (GameObject.FindGameObjectWithTag("inventoryUI").activeSelf)
        {  
            item = null;
        }
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
