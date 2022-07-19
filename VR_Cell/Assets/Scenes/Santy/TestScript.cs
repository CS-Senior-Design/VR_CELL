using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestScript : MonoBehaviour
{
    public XRSocketInteractor socket;

    void Awake()
    {
        socket.selectEntered.AddListener(SelectedObject);
    }

    public void SelectedObject(SelectEnterEventArgs args)
    {
        Debug.Log(args.interactableObject.transform.name);

        // objectWithInventoryManager.GetComponent<InventoryManager>().organelles.Add(obj.gameObject);
        // Debug.Log(objectWithInventoryManager.GetComponent<InventoryManager>().organelles);
        
    }
}
