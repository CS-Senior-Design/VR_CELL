using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WristMenu : MonoBehaviour
{
    public GameObject wristUI;
    public bool activeWristUI = true;
    private GameObject item = null;

    // Start is called before the first frame update
    void Start()
    {
        DisplayWristUI();
        
    }

    public void spawnOrganelle(GameObject organelle)
    {
        if (item == null)
        {
            item = Instantiate(
                organelle,
                new Vector3(0,1.3f,-2),
                Quaternion.identity
            );
        }
        else
        {
            Destroy(item);
            item = Instantiate(
                organelle,
                new Vector3(0,1.3f,-2),
                Quaternion.identity
            );
        }
        
    }

    public void MenuPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DisplayWristUI();
        }

    }

    public void DisplayWristUI()
    {
        Debug.Log("Hello");
        if (activeWristUI)
        {
            wristUI.SetActive(false);
            activeWristUI = false;
            // make gameobjects in the inventory invisible
        }
        else if (!activeWristUI)
        {
            wristUI.SetActive(true);
            activeWristUI = true;
        }
    }
}
