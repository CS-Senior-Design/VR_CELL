using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class InputHandling : MonoBehaviour
{
    // global references to the controllers
    private UnityEngine.XR.InputDevice _leftHandController;
    private UnityEngine.XR.InputDevice _rightHandController;
    // global references to the controllers in their array so we can determine if they are connected
    private List<UnityEngine.XR.InputDevice> _leftHandControllerArr = new List<UnityEngine.XR.InputDevice>();
    private List<UnityEngine.XR.InputDevice> _rightHandControllerArr = new List<UnityEngine.XR.InputDevice>();
    // global variable to track if the controllers are connected
    private bool _controllersConnected = false;
    // global references to every button state so that we can detect changes
    private bool _leftTriggerButtonState = false;
    private bool _leftGripButtonState = false;
    private bool _leftMenuButtonState = false;
    private bool _leftPrimary2DAxisClickState = false;
    private bool _leftPrimaryButtonState = false;
    private bool _leftPrimary2DAxisTouchState = false;

    // now for the right
    private bool _rightTriggerButtonState = false;
    private bool _rightGripButtonState = false;
    private bool _rightMenuButtonState = false;
    private bool _rightPrimary2DAxisClickState = false;
    private bool _rightPrimaryButtonState = false;
    private bool _rightPrimary2DAxisTouchState = false;

    // variable to track if we are using oculus or vive controller
    private bool _isViveController = false;

    // variable for testing with only 1 controller
    // if you are using both then set them both to false
    // if you are using no controllers then set them both to true
    private bool _isRightOnly = true;
    private bool _isLeftOnly = false;

    // variable to track if the joystick is at the home position or not
    private bool _isHome = true;

    // variables to store the rayInteractors to swap between interactable and teleporting
    public GameObject rayInteractorNormal;
    public GameObject rayInteractorTeleport;

    // Start is called before the first frame update
    void Awake()
    {

    }

    public void getLeftHandController()
    {
        // get the left hand controller if we haven't already
        if (_leftHandControllerArr.Count == 0)
        {
            // get the right handed controller
            var leftHandedControllers = new List<UnityEngine.XR.InputDevice>();
            var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller;
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandedControllers);
            // if there is only 1 right handed device (there should only be one)
            if (leftHandedControllers.Count == 1)
            {
                _leftHandController = leftHandedControllers[0];
                _leftHandControllerArr.Add(_leftHandController);
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
                _controllersConnected = true;
            }
            else if (leftHandedControllers.Count > 1)
            {
                Debug.LogError("More than one left handed controller detected!");
            }
            else
            {
                Debug.LogError("No left handed controller detected!");
            }
        }
    }

    public void getRightHandController()
    {
        // get the right hand controller if we haven't already
        if (_rightHandControllerArr.Count == 0)
        {
            // get the right handed controller
            var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
            var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
            UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);
            // if there is only 1 right handed device (there should only be one)
            if (rightHandedControllers.Count == 1)
            {
                _rightHandController = rightHandedControllers[0];
                _rightHandControllerArr.Add(_rightHandController);
                Debug.Log("Right Hand Controller: " + _rightHandController.name);
                // check if the name contains the word vive in either upper case or lowercase
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


    public void RightTriggerPressed()
    {
        Debug.Log("Right Trigger Button Pressed");
        _rightTriggerButtonState = true;
    }

    public void RightTriggerReleased()
    {
        Debug.Log("Right Trigger Button Released");
        _rightTriggerButtonState = false;
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

    public void RightPrimary2DAxisClickPressed()
    {
        Debug.Log("Right Primary 2D Axis Click Button Pressed");
        _rightPrimary2DAxisClickState = true;
    }

    public void cancelTeleport()
    {
        rayInteractorNormal.SetActive(true);
        rayInteractorTeleport.SetActive(false);
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
    }

    public void LeftTriggerChanged(float value)
    {
        Debug.Log("Left Trigger Value: " + value);
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

    public void RightPrimary2DAxisDown()
    {
        Debug.Log("Right Primary 2D Axis Down");
        _isHome = false;
    }

    public void RightPrimary2DAxisClickReleased()
    {
        Debug.Log("Right Primary 2D Axis Click Button Released");
        _rightPrimary2DAxisClickState = false;
        
        rayInteractorTeleport.SetActive(false);
        rayInteractorNormal.SetActive(true);
    }

    public void RightPrimary2DAxisHome()
    {
        Debug.Log("Right Primary 2D Axis Home Position");
        _rightPrimary2DAxisClickState = false;
        
        rayInteractorTeleport.SetActive(false);
        rayInteractorNormal.SetActive(true);
    }

    public void RightPrimary2DAxisUp()
    {
        Debug.Log("Right Primary 2D Axis Up");
        _isHome = false;

        rayInteractorNormal.SetActive(false);
        rayInteractorTeleport.SetActive(true);
    }

    public void RightPrimary2DAxisLeft()
    {
        Debug.Log("Right Primary 2D Axis Left");
        _isHome = false;
    }

    public void RightPrimary2DAxisRight()
    {
        Debug.Log("Right Primary 2D Axis Right");
        _isHome = false;
    }

    public void check2DAxis()
    {
        Vector2 primary2DAxisValue;
        bool isPressed;
        
        // if the user moves up on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.y > 0.2f) && _isHome == true)
        {
            RightPrimary2DAxisUp();
        }
        // if the user moves left on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.x < -0.2f) && _isHome == true)
        {
            RightPrimary2DAxisLeft();
        }

        // if the user moves right on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.x > 0.2f) && _isHome == true)
        {
            RightPrimary2DAxisRight();
        }

        // if the user moves down on the 2D axis and they don't have a vive
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue.y < -0.2f) && _isHome == true)
        {
            RightPrimary2DAxisDown();
        }

        // if they reset the joystick to the home position
        if (_isViveController == false && _controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out primary2DAxisValue) && (primary2DAxisValue - Vector2.zero).magnitude < 0.5f)
        {
            _isHome = true;
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
        // if the right trigger is pressed slightly but not all the way
        if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue) && triggerValue > 0.1f && triggerValue < 1.0f)
        {
            RightTriggerChanged(triggerValue);
        }
        // if the left trigger is pressed slightly but not all the way
        if (_controllersConnected && _leftHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue) && triggerValue > 0.0f && triggerValue < 1.0f)
        {
            LeftTriggerChanged(triggerValue);
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

    // Update is called once per frame
    void Update()
    {
        // Make sure we have the controllers each frame
        // if we are using both
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
        
        // print the available input actions
        if (Input.GetMouseButtonDown(0))
        {
            var featureUsages = new List<InputFeatureUsage>();
            _rightHandController.TryGetFeatureUsages(featureUsages);
            foreach (var featureUsage in featureUsages)
            {
                Debug.Log(featureUsage.name);
            }
        }

        checkTriggerButton();
        checkGripButton();
        checkTriggerValue();
        checkMenuButton();
        check2DAxis();

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
        

        
        //********************** Test code that needs to be tested with the HTC Vive ***********\\
        /*
        primary2DAxisclick == when you press it
        
        primary2dAxistouch
            - on oculus == move it
            - on Vive == touch it
        
        primary2DAxis == value that it is currently at
        */

        // Since we only care about left, right, up, and down on the 2D axis, we can just monitor the position of the joystick/trackpad, and trigger the event when it changes.
        

        //********************** Test code that needs to be tested with the HTC Vibe ***********\\

        /*
        // check if right primary 2D axis click is pressed
        if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out isPressed) && isPressed && _rightPrimary2DAxisClickState == false)
        {
            RightPrimary2DAxisClickPressed();
        }
        // check if right primary 2D axis click is released
        else if (_controllersConnected && _rightHandController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out isPressed) && isPressed == false && _rightPrimary2DAxisClickState == true)
        {
            RightPrimary2DAxisClickReleased();
        }
        */
    }
}
