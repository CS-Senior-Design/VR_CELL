using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DetectProteinNucleolus : MonoBehaviour
{
    public GameObject ribosome1 = null;
    public GameObject ribosome2;
    void Awake()
    {
        XRSocketInteractor socket = gameObject.GetComponent<XRSocketInteractor>();
        socket.onSelectEntered.AddListener(SelectedObject);
        // socket.onSelectExit.AddListener(ColorChange);
    }

    public void SelectedObject(XRBaseInteractable obj)
    {
        Debug.Log(obj.name);
        GameObject item = Instantiate(
            ribosome1,
            new Vector3(0,0,0),
            Quaternion.identity
        );
        item.transform.localScale += new Vector3(100,100,100);
        GameObject item2 = Instantiate(
            ribosome2,
            new Vector3(0.1f,0,0.1f),
            Quaternion.identity
        );
        item2.transform.localScale += new Vector3(100,100,100);

        // objectWithInventoryManager.GetComponent<InventoryManager>().organelles.Add(obj.gameObject);
        // Debug.Log(objectWithInventoryManager.GetComponent<InventoryManager>().organelles);
        
    }
}
