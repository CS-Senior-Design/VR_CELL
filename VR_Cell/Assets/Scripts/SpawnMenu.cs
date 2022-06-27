using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnMenu : MonoBehaviour
{
    public GameObject wristUI;
    public bool activeWristUI = true;
    List<GameObject> spawnedObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        DisplayWristUI();
        
    }

    public void spawnOrganelle(GameObject organelle)
    {
        GameObject item = Instantiate(
            organelle,
            new Vector3(0,1.3f,-2),
            Quaternion.identity
        );
        spawnedObjects.Add(item);
    }

    public void spawnOrganelleHelper()
    {
        
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
        }
        else if (!activeWristUI)
        {
            wristUI.SetActive(true);
            activeWristUI = true;
        }
    }
}
