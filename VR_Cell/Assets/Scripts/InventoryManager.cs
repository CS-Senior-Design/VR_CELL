using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Toggle toggle;
    public GameObject _inventory;

    void Awake() 
    {
        _inventory.SetActive(false);
        // toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(ToggleInventory);

        if (toggle.isOn)
            ToggleInventory(true);

    }

    public void ToggleInventory(bool on)
    {
        if (on)
        {
            _inventory.SetActive(true);
        }
        else
        {
            _inventory.SetActive(false);
        }

    }

    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(ToggleInventory);
    }
}
