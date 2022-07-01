using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Toggle toggle;
    public GameObject _inventory;
    public GameObject socket1;
    public GameObject socket2;
    public GameObject socket3;
    public GameObject socket4;
    // public List<GameObject> organelles = new List<GameObject>();

    void Awake() 
    {
        _inventory.SetActive(false);
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(ToggleInventory);

        if (toggle.isOn)
            ToggleInventory(true);

    }

    public void ToggleInventory(bool on)
    {

        if (on)
        {
            Debug.Log("Inventory on");
            _inventory.SetActive(true);

            socket1.GetComponent<SocketEvents>().show();
            socket2.GetComponent<SocketEvents>().show();
            socket3.GetComponent<SocketEvents>().show();
            socket4.GetComponent<SocketEvents>().show();

            // make all objects in the sockets invisible
            // for (int i = 0; i < organelles.Count; i++)
            // {
            //     organelles[i].SetActive(true);
            // }
        }
        else
        {
            _inventory.SetActive(false);
            socket1.GetComponent<SocketEvents>().hide();
            socket2.GetComponent<SocketEvents>().hide();
            socket3.GetComponent<SocketEvents>().hide();
            socket4.GetComponent<SocketEvents>().hide();
            
            // make all objects in the sockets visible again
            // for (int i = 0; i < organelles.Count; i++)
            // {
            //     organelles[i].SetActive(false);
            // }
        }

    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(ToggleInventory);
    }
}
