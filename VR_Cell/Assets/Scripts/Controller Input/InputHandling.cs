using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR;

/* 
Script for getting input from the VR controller. 
If you want something to happen on a specific button/value then add the code under the respective method for that button/value. 
*/

class ItemInfo
{
    public GameObject item;
    public Vector3 initialScale;
}

public class InputHandling : MonoBehaviour
{
    // public variables to add on the editor
    [Header("Which scene?")]
    // variables to track whether we are in immersive or lab scene
    [SerializeField] public bool _immersive;
    // variables to store the rayInteractors to swap between interactable and teleporting
    private GameObject _rayInteractorNormal;
    private GameObject _rayInteractorTeleport;

    // global references to the controllers
    private UnityEngine.XR.InputDevice _leftHandController;
    private UnityEngine.XR.InputDevice _rightHandController;
    // global references to the controllers in their array so we can determine if they are connected
    private List<UnityEngine.XR.InputDevice> _leftHandControllerArr = new List<UnityEngine.XR.InputDevice>();
    private List<UnityEngine.XR.InputDevice> _rightHandControllerArr = new List<UnityEngine.XR.InputDevice>();
    // global variable to track if the controllers are connected
    private bool _controllersConnected = false;
    // global references to every button state so that we can detect changes
    // left controller button states
    private bool _leftTriggerButtonState = false;
    private bool _leftGripButtonState = false;
    private bool _leftMenuButtonState = false;
    private bool _leftPrimary2DAxisClickState = false;
    private bool _leftPrimaryButtonState = false;
    private bool _leftPrimary2DAxisTouchState = false;
    private float _leftTriggerValue = 0.0f;
    // right controller button states
    private bool _rightTriggerButtonState = false;
    private bool _rightGripButtonState = false;
    private bool _rightMenuButtonState = false;
    private bool _rightPrimary2DAxisClickState = false;
    private bool _rightPrimaryButtonState = false;
    private bool _rightPrimary2DAxisTouchState = false;
    private float _rightTriggerValue = 0.0f;   
    // variable to be able to change at what point the trigger actuates
    private float _triggerActuationValue = 0.01f;
    // variable to track if the joystick is at the home position or not
    private bool _is2DAxisRightHome = true;
    private bool _is2DAxisLeftHome = true;
    // variable to track if we are using oculus or vive controller
    private bool _isViveController = false;
    // variable to store the reference to the gameobject with the Player tag (xr origin)
    private GameObject _player;
    // variable to change how far back the playr is teleported when they step back
    private float _defaultStepBackDistance = 0.5f;
    // how fast the player moves forward
    private float _continuousMovementSensitivity = 10.0f;

    // wrist menu (immersive) global variables
    private GameObject _wristUIPanel;
    private GameObject _inventoryPanel;
    private GameObject _fastTravelPanel;
    private bool _isWristMenuActive = false;
    private bool _canGoBack = false;
    
    // inventory menu global variables
    private List<GameObject> _inventorySockets = new List<GameObject>();
    // dictionary to store the items in each socket in the right order
    Dictionary < int, ItemInfo > _itemsInSockets = new Dictionary < int, ItemInfo > ();
    // variable to track the last used socket index
    private int _lastUsedSocket = 0;
    // variable to track how many sockets there are total in the inventory
    private int _totalInventorySockets;

    // store the teleport tube in a global variable
    private GameObject _teleportTube;
    // store the teleport tube's point in a global 
    private GameObject _teleportTubePoint;
    // original scale of the teleport tube
    private Vector3 _teleportTubeOriginalScale;
    // store the right hand controller in a global variable
    private GameObject _rightHand;
    // variable to change how quickly the teleport tube grows
    private float _teleportTubeSensitivity = 2f;
    
    // variable to track if the user wants continuous movement or not
    [Header("Movement")]
    [Tooltip("Check if you want to be able to move around without having to teleport.")]
    [SerializeField] public bool _continuousMovement = false;

    // variables for testing with only 1 controller
    // if you are using both controllers then set them both to false
    // if you are using no controllers then set them both to true
    [Header("Testing with 1 Controller")]
    [Tooltip("Check if you are only using the right controller. Leave both unchecked if you're using both controllers.")]
    [SerializeField] public bool _isRightOnly = false;
    [Tooltip("Check if you are only using the left controller. Leave both unchecked if you're using both controllers.")]
    [SerializeField] public bool _isLeftOnly = false;

    void Awake()
    {
        // get the player at the start of the program so we can do things like snap turning.
        _player = GameObject.FindGameObjectWithTag("Player");
        // get the ray interactors
        _rayInteractorNormal = GameObject.FindGameObjectWithTag("rightRayInteractor");
        _rayInteractorTeleport = GameObject.FindGameObjectWithTag("rightRayInteractorTeleport");
        // hide the _rayInteractorTeleport at the start of the program
        _rayInteractorTeleport.SetActive(false);

        // keeping track of wrist menu game objects
        _wristUIPanel = GameObject.FindGameObjectWithTag("WristUI");
        _inventoryPanel = GameObject.FindGameObjectWithTag("inventoryPanel");
        _fastTravelPanel = GameObject.FindGameObjectWithTag("fastTravelUI");

        // get all the sockets from the inventory into the list
        foreach(GameObject socket in GameObject.FindGameObjectsWithTag("inventorySocket"))
        {
            // add the socket to the list
            _inventorySockets.Add(socket);
            // add the listeners to the socket
            socket.GetComponent<XRSocketInteractor>().selectEntered.AddListener(SocketFilled);
            socket.GetComponent<XRSocketInteractor>().selectExited.AddListener(SocketEmptied);
        }
        
        // hide all the sockets at the start
        foreach (GameObject socket in _inventorySockets)
            socket.SetActive(false);

        // get how many total sockets are in the inventory
        _totalInventorySockets = _inventorySockets.Count;

        // make all wrist panels hidden at the start
        _wristUIPanel.SetActive(false);
        _inventoryPanel.SetActive(false);
        _fastTravelPanel.SetActive(false);

        // get the right hand by tag
        _rightHand = GameObject.FindGameObjectWithTag("rightHand");
        // get the teleport tube by tag
        _teleportTube = GameObject.FindGameObjectWithTag("teleportTube");
        // get the teleport tube point by tag
        _teleportTubePoint = GameObject.FindGameObjectWithTag("teleportTubePoint");
        // hide the teleport tube at the start
        _teleportTube.SetActive(false);
        // store the original scale of the teleport tube
        _teleportTubeOriginalScale = _teleportTube.transform.localScale;
    }

    // check for input on every frame
    void Update()
    {
        GetControllers();
        checkTriggerButton();
        checkGripButton();
        checkTriggerValue();
        checkMenuButton();
        check2DAxis();
        checkPrimaryButton();

        // if the user chooses to use continuous movement
        if (_continuousMovement == true)
        {
            // if they are holding down the right trigger do continuous movement forward
            if (_rightTriggerValue >= 0.05f && _immersive)
            {
                continuousMovementForward();
            }
            
            // if they are holding down the left trigger do continuous movement backward
            if (_leftTriggerValue >= 0.05f && _immersive)
            {
                continuousMovementBackward();
            }
        }
        // if the user prefers to teleport
        else
        {
            // if they are holding down the right trigger then show the teleport tube
            if (_rightTriggerButtonState == true && _immersive)
            {
                tubeGrow();
            }
        }
    }

    public void tubeGrow()
    {
        // show the teleport tube
        _teleportTube.SetActive(true);
        // get the new scale
        Vector3 newScale = new Vector3(_teleportTube.transform.localScale.x, _teleportTube.transform.localScale.y + _teleportTubeSensitivity, _teleportTube.transform.localScale.z);
        
        // grow the teleport tube
        _teleportTube.transform.localScale = newScale;
    }

    public void tubeTeleport()
    {
        // get the position of the teleport tube point
        Vector3 teleportTubePointPosition = _teleportTubePoint.transform.position;
        // move the player to this position
        _player.transform.position = teleportTubePointPosition;
        // make tube the normal scale again
        _teleportTube.transform.localScale = _teleportTubeOriginalScale;
        // hide the teleport tube
        _teleportTube.SetActive(false);
    }

    public void SocketEmptied(SelectExitEventArgs obj)
    {
        // only count deselecting if the socket is active
        if ( _inventorySockets[_lastUsedSocket].activeSelf == true )
        {
            // deparent the object
            obj.interactableObject.transform.parent = null;
            // change object to normal scale
            _itemsInSockets[_lastUsedSocket].item.transform.localScale = _itemsInSockets[_lastUsedSocket].initialScale;
            // remove the item from the dictionary
            _itemsInSockets.Remove(_lastUsedSocket);  
            
            // display the correct name if it's full and the number socket it is
            GameObject numberText = GameObject.FindGameObjectWithTag("slotNumberText");
            numberText.GetComponent<TMPro.TextMeshProUGUI>().text = (_lastUsedSocket + 1).ToString();
            if (_itemsInSockets.ContainsKey(_lastUsedSocket))
            {
                GameObject organelleName = GameObject.FindGameObjectWithTag("organelleNameTextSocket");
                organelleName.GetComponent<TMPro.TextMeshProUGUI>().text = getOrganelleName(_itemsInSockets[_lastUsedSocket].item.transform.name);
            }
            else
            {
                GameObject organelleName = GameObject.FindGameObjectWithTag("organelleNameTextSocket");
                organelleName.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }  
        }
    }

    public void SocketFilled(SelectEnterEventArgs obj)
    {
        // If the object does not have the socket as the parent already
        if (obj.interactableObject.transform.parent != _inventorySockets[_lastUsedSocket])
        {    
            GameObject item = obj.interactableObject.transform.gameObject;

            // store the objects initial scale
            Vector3 initialScale = item.transform.localScale;  

            // create the new item object
            ItemInfo tempItem = new ItemInfo();
            tempItem.item = item;
            tempItem.initialScale = initialScale; 

            // scale the item down so it fits better
            item.transform.localScale = new Vector3(2, 2, 2); 
            // set the socket as the parent
            item.transform.parent = _inventorySockets[_lastUsedSocket].transform;
            // put the object in the dictionary
            _itemsInSockets[_lastUsedSocket] = tempItem;

            // display the correct name if it's full and the number socket it is
            GameObject numberText = GameObject.FindGameObjectWithTag("slotNumberText");
            numberText.GetComponent<TMPro.TextMeshProUGUI>().text = (_lastUsedSocket + 1).ToString();
            if (_itemsInSockets.ContainsKey(_lastUsedSocket))
            {
                GameObject organelleName = GameObject.FindGameObjectWithTag("organelleNameTextSocket");
                organelleName.GetComponent<TMPro.TextMeshProUGUI>().text = getOrganelleName(_itemsInSockets[_lastUsedSocket].item.transform.name);
            }
        }
        else
        {
            return;
        }
    }

    public string getOrganelleName(string fullName)
    {
        Debug.Log(fullName);
        if (fullName.Contains("nucleolus") || fullName.Contains("Nucleolus"))
            return "Nucleolus";
        else if (fullName.Contains("ribosome") || fullName.Contains("Ribosome"))
        {
            return "Ribosome";
        }
        else if (fullName.Contains("mitochondria") || fullName.Contains("Mitochondria"))
        {
            return "Mitochondria";
        }
        else if (fullName.Contains("30"))
        {
            return "Ribosome 40";
        }
        else if (fullName.Contains("50"))
        {
            return "Ribosome 60";
        }
        else if (fullName.Contains("glycoprotein") || fullName.Contains("Glycoprotein"))
        {
            return "Glycoprotein";
        }
        else if (fullName.Contains("rough") || fullName.Contains("Rough"))
        {
            return "Rough ER";
        }
        else if (fullName.Contains("smooth") || fullName.Contains("Smooth"))
        {
            return "Smooth ER";
        }
        else if (fullName.Contains("golgi") || fullName.Contains("Golgi"))
        {
            return "Golgi";
        }
        else if (fullName.Contains("vesicle") || fullName.Contains("Vesicle"))
        {
            return "Vesicle";
        }
        else if (fullName.Contains("protein") || fullName.Contains("Protein"))
        {
            return "Protein";
        }
        else if (fullName.Contains("mrna") || fullName.Contains("MRNA") || fullName.Contains("mRNA") || fullName.Contains("Mrna"))
        {
            return "mRNA";
        } 
        else
            return "HOW???";
    }

    public void nextInventorySocket()
    {
        Debug.Log("Changing to socket: " + _lastUsedSocket);
        //hideItemInSocket(_lastUsedSocket);
        // current socket needs to be turned off when we go next
        _inventorySockets[_lastUsedSocket].SetActive(false);

        // if we press next while on the last socket just go back to the first one
        if (_lastUsedSocket == _totalInventorySockets - 1)
            _lastUsedSocket = 0;
        // otherwise just increment
        else
            _lastUsedSocket++;
        
        // set the new current socket to active
        _inventorySockets[_lastUsedSocket].SetActive(true);

        // display the correct name if it's full and the number socket it is
        GameObject numberText = GameObject.FindGameObjectWithTag("slotNumberText");
        numberText.GetComponent<TMPro.TextMeshProUGUI>().text = (_lastUsedSocket + 1).ToString();
        if (_itemsInSockets.ContainsKey(_lastUsedSocket))
        {
            GameObject organelleName = GameObject.FindGameObjectWithTag("organelleNameTextSocket");
            organelleName.GetComponent<TMPro.TextMeshProUGUI>().text = getOrganelleName(_itemsInSockets[_lastUsedSocket].item.transform.name);
        }
        else
        {
            GameObject organelleName = GameObject.FindGameObjectWithTag("organelleNameTextSocket");
            organelleName.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }
        //showItemInSocket(_lastUsedSocket);
    }

    public void prevInventorySocket()
    {
        Debug.Log("Changing to socket: " + _lastUsedSocket);
        //hideItemInSocket(_lastUsedSocket);
        // current socket needs to be turned off when we go next
        _inventorySockets[_lastUsedSocket].SetActive(false);

        if (_lastUsedSocket == 0)
            _lastUsedSocket = _totalInventorySockets - 1;
        else
            _lastUsedSocket--;

        // set the new current socket to active
        _inventorySockets[_lastUsedSocket].SetActive(true);

        // display the correct name if it's full and the number socket it is
        GameObject numberText = GameObject.FindGameObjectWithTag("slotNumberText");
        numberText.GetComponent<TMPro.TextMeshProUGUI>().text = (_lastUsedSocket + 1).ToString();
        if (_itemsInSockets.ContainsKey(_lastUsedSocket))
        {
            GameObject organelleName = GameObject.FindGameObjectWithTag("organelleNameTextSocket");
            organelleName.GetComponent<TMPro.TextMeshProUGUI>().text = getOrganelleName(_itemsInSockets[_lastUsedSocket].item.transform.name);
        }
        else
        {
            GameObject organelleName = GameObject.FindGameObjectWithTag("organelleNameTextSocket");
            organelleName.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }
        //showItemInSocket(_lastUsedSocket);
    }

    public void hideItemInSocket(int socketIndex)
    {
        if (_itemsInSockets.ContainsKey(socketIndex))
            _itemsInSockets[socketIndex].item.SetActive(false);
    }

    public void showItemInSocket(int socketIndex)
    {
        if (_itemsInSockets.ContainsKey(socketIndex))
            _itemsInSockets[socketIndex].item.SetActive(true);
    }

    public void hideInventory()
    {
        // hide the current active socket and its item
        //hideItemInSocket(_lastUsedSocket);
        _inventorySockets[_lastUsedSocket].SetActive(false);
        _inventoryPanel.SetActive(false);
    }

    public void wristMenuToggle()
    {
        // if wrist menu is not active and user is not in a sub wrist menu
        if (!_isWristMenuActive && !_canGoBack)
        {
            // activate main menu
            _wristUIPanel.SetActive(true);

            // hide sub menus
            _inventoryPanel.SetActive(false);
            _fastTravelPanel.SetActive(false);

            // toggle variable keeps track of active state
            _canGoBack = _isWristMenuActive = true;
        }

        // if user is in submenu hide every panel
        else if (!_isWristMenuActive && _canGoBack)
        {
            _wristUIPanel.SetActive(false);
            hideInventory();
            _fastTravelPanel.SetActive(false);
            _canGoBack = false;
        }

        // if user is on main wrist menu hide menu
        else
        {
            _wristUIPanel.SetActive(false);
            _isWristMenuActive = false;
        }
    }

    public void showFastTravelMenu()
    {
        _wristUIPanel.SetActive(false);
        _isWristMenuActive = false;
        _fastTravelPanel.SetActive(true);
        _canGoBack = true;
    }

    public void showInventoryMenu()
    {
        _wristUIPanel.SetActive(false);
        _isWristMenuActive = false;
        _inventoryPanel.SetActive(true);
        _canGoBack = true;

        // show the last used socket
        Debug.Log("Count = " + _inventorySockets.Count);
        _inventorySockets[_lastUsedSocket].SetActive(true);
        Debug.Log("Last used socket " + _lastUsedSocket);

        // display the correct name if it's full and the number socket it is
        GameObject numberText = GameObject.FindGameObjectWithTag("slotNumberText");
        numberText.GetComponent<TMPro.TextMeshProUGUI>().text = (_lastUsedSocket + 1).ToString();
        if (_itemsInSockets.ContainsKey(_lastUsedSocket))
        {
            GameObject organelleName = GameObject.FindGameObjectWithTag("organelleNameTextSocket");
            organelleName.GetComponent<TMPro.TextMeshProUGUI>().text = getOrganelleName(_itemsInSockets[_lastUsedSocket].item.transform.name);
        }
        else
        {
            GameObject organelleName = GameObject.FindGameObjectWithTag("organelleNameTextSocket");
            organelleName.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }
    }

    public void wristMenuBack()
    {
        // only go back if you are in the wrist main menu or submenu
        if (_canGoBack)
        {
            if (_isWristMenuActive)
            {
                _wristUIPanel.SetActive(false);
                _inventoryPanel.SetActive(false);
                _fastTravelPanel.SetActive(false);

                _isWristMenuActive = false;
                _canGoBack = false;
            }

            else
            {
                _wristUIPanel.SetActive(true);
                _inventoryPanel.SetActive(false);
                _fastTravelPanel.SetActive(false);

                _isWristMenuActive = true;
            }
        }
    }

    // called from the update function
    public void continuousMovementForward()
    {
        float sensitivity = _rightTriggerValue * _continuousMovementSensitivity;
        _player.transform.position +=  Camera.main.transform.forward * sensitivity * Time.deltaTime;
    }
    
    // called from the update function
    public void continuousMovementBackward()
    {
        float sensitivity = _leftTriggerValue * _continuousMovementSensitivity;
        _player.transform.position +=  -Camera.main.transform.forward * sensitivity * Time.deltaTime;
    }

    public void teleportBackwards()
    {
        Debug.Log("moving backwards!");
        float x = _player.transform.position.x - Camera.main.transform.forward.x * _defaultStepBackDistance;
        float z = _player.transform.position.z - Camera.main.transform.forward.z * _defaultStepBackDistance;
        Vector3 tempPosition = new Vector3(x, _player.transform.position.y, z);
        _player.transform.position = tempPosition;
        /*
        RaycastHit[] hits;
        hits = Physics.RaycastAll(tempPosition, Vector3.down, 10.0f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "teleportArea" || hit.transform.tag == "teleportAnchor")
            {
                _player.transform.position = tempPosition;
                break;
            }
        }
        */
        /*
        if (Physics.Raycast(tempPosition, -Vector3.up, out RaycastHit hit))
        {
            Debug.Log(hit.transform.tag);
            if (hit.transform.tag.Contains("teleportArea"))
                _player.transform.position = tempPosition;
        }
        */
    }

    public void rotatePlayer(float rotation)
    {
        // find the MainCamera and rotate it
        //GameObject mainCamera = GameObject.FindGameObjectWithTag("CameraOffset");
        _player.transform.Rotate(0, rotation, 0);
    }

    // switch from normal interactor to teleportation interactor
    public void openTeleport()
    {
        _rayInteractorNormal.SetActive(false);
        _rayInteractorTeleport.SetActive(true);
    }

    // switch from teleportation interactor to normal interactor
    public void cancelTeleport()
    {
        _rayInteractorNormal.SetActive(true);
        _rayInteractorTeleport.SetActive(false);
    }

    // wrapper to get both controllers depending on if you toggled _isLeftOnly or _isRightOnly
    public void GetControllers()
    {
        // if we are using both controllers
        if (_isLeftOnly == false && _isRightOnly == false)
        {
            getLeftHandController();
            getRightHandController();
        }
        // if we are only using the right one
        else if (_isRightOnly == true && _isLeftOnly == false)
            getRightHandController();
        // if we are only using the left one
        else if (_isLeftOnly == true && _isRightOnly == false)
            getLeftHandController();
    }

    // get the left controller if we haven't already
    public void getLeftHandController()
    {
        // if we haven't already gotten the left controller
        if (_leftHandControllerArr.Count == 0)
        {
            // get the left handed controller
            var leftHandedControllers = new List<UnityEngine.XR.InputDevice>();
            // store the characteristics of a left handed controller
            var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller;
            // put the list of controllers that match the characteristics into the list we created
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandedControllers);
            // if there is only 1 right handed device
            if (leftHandedControllers.Count == 1)
            {
                // store it in the global variable
                _leftHandController = leftHandedControllers[0];
                // add it to the lefthand controller array so that we can check if we have a left controller
                _leftHandControllerArr.Add(_leftHandController);
                // print the controller name
                Debug.Log("Left Hand Controller: " + _leftHandController.name);
                // check if the name contains the word vive in either upper case or lowercase
                if (_leftHandController.name.Contains("Vive") || _leftHandController.name.Contains("vive"))
                {
                    _isViveController = true;
                }
                else
                {
                    _isViveController = false;
                }
                // toggle variable to show that we have a connected controller
                _controllersConnected = true;
            }
            // if we have more than one left controller (currently will just print and not store any)
            else if (leftHandedControllers.Count > 1)
            {
                Debug.LogError("More than one left handed controller detected!");
            }
            // if we have no left controller yet
            else
            {
                Debug.LogError("No left handed controller detected!");
            }
        }
    }

    // get the right hand controller if we haven't already
    public void getRightHandController()
    {
        if (_rightHandControllerArr.Count == 0)
        {
            var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
            var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);
            if (rightHandedControllers.Count == 1)
            {
                _rightHandController = rightHandedControllers[0];
                _rightHandControllerArr.Add(_rightHandController);
                Debug.Log("Right Hand Controller: " + _rightHandController.name);
                if (_rightHandController.name.Contains("Vive") || _rightHandController.name.Contains("vive"))
                {
                    _isViveController = true;
                }
                else
                {
                    _isViveController = false;
                }
                _controllersConnected = true;
            }
            else if (rightHandedControllers.Count > 1)
            {
                Debug.LogError("More than one right handed controller detected!");
            }
            else
            {
                Debug.LogError("No right handed controller detected!");
            }
        }
    }

    /* Below are the methods that get triggered when each button is pressed or released. */
    public void RightTriggerPressed()
    {
        Debug.Log("Right Trigger Button Pressed");
        _rightTriggerButtonState = true;
    }

    public void RightTriggerReleased()
    {
        Debug.Log("Right Trigger Button Released");
        _rightTriggerButtonState = false;
        // move the player to the tube position
        tubeTeleport();
    }

    public void LeftTriggerPressed()
    {
        Debug.Log("Left Trigger Button Pressed");
        _leftTriggerButtonState = true;
    }

    public void LeftTriggerReleased()
    {
        Debug.Log("Left Trigger Button Released");
        _leftTriggerButtonState = false;
    }

    public void RightGripPressed()
    {
        Debug.Log("Right Grip Button Pressed");
        _rightGripButtonState = true;
        cancelTeleport();
    }

    public void RightGripReleased()
    {
        Debug.Log("Right Grip Button Released");
        _rightGripButtonState = false;
    }

    public void LeftGripPressed()
    {
        Debug.Log("Left Grip Button Pressed");
        _leftGripButtonState = true;
    }

    public void LeftGripReleased()
    {
        Debug.Log("Left Grip Button Released");
        _leftGripButtonState = false;
    }

    public void RightTriggerChanged(float value)
    {
        Debug.Log("Right Trigger Value: " + value);
        _rightTriggerValue = value;
    }

    public void LeftTriggerChanged(float value)
    {
        Debug.Log("Left Trigger Value: " + value);
        _leftTriggerValue = value;
    }

    public void RightMenuPressed()
    {
        Debug.Log("Right Menu Button Pressed");
        _rightMenuButtonState = true;
    }

    public void RightMenuReleased()
    {
        Debug.Log("Right Menu Button Released");
        _rightMenuButtonState = false;
    }

    public void LeftMenuPressed()
    {
        Debug.Log("Left Menu Button Pressed");
        _leftMenuButtonState = true;

        // wrist menu pops up
        wristMenuToggle();
    }

    public void LeftMenuReleased()
    {
        Debug.Log("Left Menu Button Released");
        _leftMenuButtonState = false;
    }

    public void RightPrimaryPressed()
    {
        Debug.Log("Right Primary Button Pressed");
        _rightPrimaryButtonState = true;
    }

    public void RightPrimaryReleased()
    {
        Debug.Log("Right Primary Button Released");
        _rightPrimaryButtonState = false;
    }

    public void LeftPrimaryPressed()
    {
        Debug.Log("Left Primary Button Pressed");
        _leftPrimaryButtonState = true;
    }

    public void LeftPrimaryReleased()
    {
        Debug.Log("Left Primary Button Released");
        _leftPrimaryButtonState = false;
    }

    public void RightPrimary2DAxisClickPressed()
    {
        Debug.Log("Right Primary 2D Axis Click Button Pressed");
        _rightPrimary2DAxisClickState = true;
    }

    public void RightPrimary2DAxisClickReleased()
    {
        Debug.Log("Right Primary 2D Axis Click Button Released");
        _rightPrimary2DAxisClickState = false;
        
        // swap the teleport ray to normal one on releasing the right primary2DAxis (dpad)
        cancelTeleport();
    }

    public void RightPrimary2DAxisHome()
    {
        Debug.Log("Right Primary 2D Axis Home Position");
        _rightPrimary2DAxisClickState = false;
        _is2DAxisRightHome = true;

        // swap the teleport ray to normal one on releasing the right primary2DAxis (joystick)
        cancelTeleport();
    }

    public void RightPrimary2DAxisDown()
    {
        Debug.Log("Right Primary 2D Axis Down");
        _is2DAxisRightHome = false;

        // teleport backwards 
        teleportBackwards();
    }

    public void RightPrimary2DAxisUp()
    {
        Debug.Log("Right Primary 2D Axis Up");
        _is2DAxisRightHome = false;

        // swap the normal ray to the teleport ray on pressing Up on the right primary2DAxis (joystick or dpad)
        openTeleport();
    }

    public void RightPrimary2DAxisLeft()
    {
        Debug.Log("Right Primary 2D Axis Left");
        _is2DAxisRightHome = false;

        // rotate the player 90 degrees to the left
        rotatePlayer(-45.0f);
    }

    public void RightPrimary2DAxisRight()
    {
        Debug.Log("Right Primary 2D Axis Right");
        _is2DAxisRightHome = false;

        // rotate the player 90 degrees to the right
        rotatePlayer(45.0f);
    }

    public void LeftPrimary2DAxisClickPressed()
    {
        Debug.Log("Left Primary 2D Axis Click Button Pressed");
        _leftPrimary2DAxisClickState = true;
    }

    public void LeftPrimary2DAxisClickReleased()
    {
        Debug.Log("Left Primary 2D Axis Click Button Released");
        _leftPrimary2DAxisClickState = false;
    }

    public void LeftPrimary2DAxisHome()
    {
        Debug.Log("Left Primary 2D Axis Home Position");
        _leftPrimary2DAxisClickState = false;
        _is2DAxisLeftHome = true;
    }

    public void LeftPrimary2DAxisUp()
    {
        Debug.Log("Left Primary 2D Axis Up");
        _is2DAxisLeftHome = false;
    }

    public void LeftPrimary2DAxisLeft()
    {
        Debug.Log("Left Primary 2D Axis Left");
        _is2DAxisLeftHome = false;

        // wrist menu back button
        wristMenuBack();
    }

    public void LeftPrimary2DAxisRight()
    {
        Debug.Log("Left Primary 2D Axis Right");
        _is2DAxisLeftHome = false;
    }

    public void LeftPrimary2DAxisDown()
    {
        Debug.Log("Left Primary 2D Axis Down");
        _is2DAxisLeftHome = false;
    }

    public void check2DAxis()
    {
        Vector2 primary2DAxisValue;
        bool isPressed;
        
        // if the user moves up on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.y > 0.2f) && _is2DAxisRightHome == true)
        {
            RightPrimary2DAxisUp();
        }
        // if the user moves left on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.x < -0.2f) && _is2DAxisRightHome == true)
        {
            RightPrimary2DAxisLeft();
        }
        // if the user moves right on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.x > 0.2f) && _is2DAxisRightHome == true)
        {
            RightPrimary2DAxisRight();
        }
        // if the user moves down on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.y < -0.2f) && _is2DAxisRightHome == true)
        {
            RightPrimary2DAxisDown();
        }
        // if they reset the joystick to the home position
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue).magnitude < 0.1f && _is2DAxisRightHome == false)
        {
            RightPrimary2DAxisHome();
        }
        // if the user touches presses the 2D axis and they have a vive
        if (_isViveController && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out isPressed) && isPressed && _rightPrimary2DAxisClickState == false)
        {
            _rightPrimary2DAxisClickState = true;
            Vector2 value;
            // determine what direction the user is touching the 2D axis
            if (_rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out value))
            {
                if (value.y > 0.5f)
                    RightPrimary2DAxisUp();
                else if (value.x < -0.5f)
                    RightPrimary2DAxisLeft();
                else if (value.x > 0.5f)
                    RightPrimary2DAxisRight();
                else if (value.y < -0.5f)
                    RightPrimary2DAxisDown();
            }
        }
        // if the user releases the 2D axis and have a vive
        else if (_isViveController && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out isPressed) && isPressed == false && _rightPrimary2DAxisClickState == true)
        {
            RightPrimary2DAxisClickReleased();
        }

        // now check the left 2D axis

        // if the user moves up on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.y > 0.2f) && _is2DAxisLeftHome == true)
        {
            LeftPrimary2DAxisUp();
        }
        // if the user moves left on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.x < -0.2f) && _is2DAxisLeftHome == true)
        {
            LeftPrimary2DAxisLeft();
        }
        // if the user moves right on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.x > 0.2f) && _is2DAxisLeftHome == true)
        {
            LeftPrimary2DAxisRight();
        }
        // if the user moves down on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.y < -0.2f) && _is2DAxisLeftHome == true)
        {
            LeftPrimary2DAxisDown();
        }
        // if they reset the joystick to the home position
        if (_isViveController == false && _controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue).magnitude < 0.1f && _is2DAxisLeftHome == false)
        {
            LeftPrimary2DAxisHome();
        }
        // if the user touches presses the 2D axis and they have a vive
        if (_isViveController && _controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out isPressed) && isPressed && _leftPrimary2DAxisClickState == false)
        {
            _leftPrimary2DAxisClickState = true;
            Vector2 value;
            // determine what direction the user is touching the 2D axis
            if (_leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out value))
            {
                if (value.y > 0.5f)
                    LeftPrimary2DAxisUp();
                else if (value.x < -0.5f)
                    LeftPrimary2DAxisLeft();
                else if (value.x > 0.5f)
                    LeftPrimary2DAxisRight();
                else if (value.y < -0.5f)
                    LeftPrimary2DAxisDown();
            }
        }
        // if the user releases the 2D axis and have a vive
        else if (_isViveController && _controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out isPressed) && isPressed == false && _leftPrimary2DAxisClickState == true)
        {
            LeftPrimary2DAxisClickReleased();
        }
    }

    public void checkTriggerButton()
    {
        bool isPressed;
        // check if right trigger is pressed
        if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out isPressed) && isPressed && _rightTriggerButtonState == false)
        {
            //StartCoroutine(teleportCoroutine());
            RightTriggerPressed();
        }
        // check if right trigger is released
        else if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out isPressed) && isPressed == false && _rightTriggerButtonState == true)
        {
            //_isPressed = false;
            //TurnOffTeleport();
            RightTriggerReleased();
        }
        // check if left trigger is pressed
        if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out isPressed) && isPressed && _leftTriggerButtonState == false)
        {
            LeftTriggerPressed();
        }
        // check if left trigger is released
        else if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out isPressed) && isPressed == false && _leftTriggerButtonState == true)
        {
            LeftTriggerReleased();
        }
    }

    public void checkTriggerValue()
    {
        float triggerValue;
        // if the right trigger is pressed past a certain threshold
        if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue) && triggerValue >= _triggerActuationValue)
        {
            RightTriggerChanged(triggerValue);
        }
        // if the right trigger is released past a certain threshold
        else if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue) && triggerValue < _triggerActuationValue)
        {
            _rightTriggerValue = 0.0f;
        }
        // if the left trigger is pressed slightly but not all the way
        if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue) && triggerValue >= _triggerActuationValue)
        {
            LeftTriggerChanged(triggerValue);
        }
        // if the left trigger is released past a certain threshold
        else if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue) && triggerValue < _triggerActuationValue)
        {
            _leftTriggerValue = 0.0f;
        }
    }

    public void checkGripButton()
    {
        bool isPressed;
        // check if right grip is pressed
        if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out isPressed) && isPressed && _rightGripButtonState == false)
        {
            RightGripPressed();
        }
        // check if right grip is released
        else if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out isPressed) && isPressed == false && _rightGripButtonState == true)
        {
            RightGripReleased();
        }
        // check if left grip is pressed
        if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out isPressed) && isPressed && _leftGripButtonState == false)
        {
            LeftGripPressed();
        }
        // check if left grip is released
        else if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out isPressed) && isPressed == false && _leftGripButtonState == true)
        {
            LeftGripReleased();
        }
    }

    public void checkMenuButton()
    {
        bool isPressed;
        // if the right menu button is pressed
        if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out isPressed) && isPressed && _rightMenuButtonState == false)
        {
            RightMenuPressed();
        }
        // if the right menu button is released
        else if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out isPressed) && isPressed == false && _rightMenuButtonState == true)
        {
            RightMenuReleased();
        }

        // if the left menu button is pressed
        if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out isPressed) && isPressed && _leftMenuButtonState == false)
        {
            LeftMenuPressed();
        }
        // if the left menu button is released
        else if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out isPressed) && isPressed == false && _leftMenuButtonState == true)
        {
            LeftMenuReleased();
        }
    }

    public void checkPrimaryButton()
    {
        bool isPressed;
        // if the right primary button is pressed
        if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out isPressed) && isPressed && _rightPrimaryButtonState == false)
        {
            RightPrimaryPressed();
        }
        // if the right primary button is released
        else if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out isPressed) && isPressed == false && _rightPrimaryButtonState == true)
        {
            RightPrimaryReleased();
        }
        // if the left primary button is pressed
        if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out isPressed) && isPressed && _leftPrimaryButtonState == false)
        {
            LeftPrimaryPressed();
        }
        // if the left primary button is released
        else if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out isPressed) && isPressed == false && _leftPrimaryButtonState == true)
        {
            LeftPrimaryReleased();
        }
    }
}
