using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTeleportController : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction _teleportMenu;
    private bool _isShowing = false;
    
    private void Start()
    {
        _teleportMenu = inputActions.FindActionMap("XRI RightHand").FindAction("TeleportMenu");
        _teleportMenu.Enable();
        _teleportMenu.performed += ToggleTeleportMenu;
    }

    public void ToggleTeleportMenu(InputAction.CallbackContext context)
    {
        Debug.Log("herersneohtu");
        if (_isShowing == true)
        {    
            gameObject.SetActive(false);
            _isShowing = false;
        }
        else
        {
            gameObject.SetActive(true);
            _isShowing = true;
        }
    }
}
