using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportToggle : MonoBehaviour
{
    [Header("Right Hand Teleportation Controller")]
    // button to activate the teleportation ray
    [SerializeField] private InputActionProperty activate;
    // button to cancel the teleportation ray if it's active
    [SerializeField] private InputActionProperty cancel;

    // the ray interactor that we want to change
    [SerializeField] private XRRayInteractor rayInteractor;
    // the teleportation layers that the ray should interact with to teleport (teleport area, teleport anchor, etc.)
    [SerializeField] private InteractionLayerMask teleportationLayers;
    // the locomotion system in the scene
    [SerializeField] private TeleportationProvider provider;
    
    // whether the teleportation ray is active or not
    private bool _isActive = false;
    // detects whether the activate button is pressed
    private bool _isPressed = false; 
    // if the ray is on an invalid object
    private bool _isValidTarget = true;
    // the initial layers that the rayInteractor was set to interact with
    private InteractionLayerMask initialInteractionLayers;
    // a list to store the available objects that the rayInteractor can interact with
    private List<IXRInteractable> interactables = new List<IXRInteractable>();

    

    // Start is called before the first frame update
    void Start()
    {
        // enable the reading of the activate button
        activate.action.Enable();
        // enable the reading of the cancel button
        cancel.action.Enable();

        // call the OnTeleportActivate when the activate button is pressed and held
        activate.action.started += OnTeleportActivate;
        activate.action.performed += OnTeleportSelect;
        // call the OnTeleportCancel when the cancel button is pressed
        cancel.action.performed += OnTeleportCancel;
        // store the starting interaction layers in a global variable
        initialInteractionLayers = rayInteractor.interactionLayers;
        
    }

    void OnTeleportSelect(InputAction.CallbackContext context)
    {
        _isPressed = false;
    }

    // // Update is called once per frame
    void Update()
    {
        if (_isActive == false)
            return;
        
        if (activate.action.triggered)
        {
            return;
        }

        rayInteractor.GetValidTargets(interactables);

        // if the ray is pointing at non-interactable objects
        if (interactables.Count == 0)
        {
            // set _isValidTarget to false
            _isValidTarget = false;
            return;
        }
        // if the ray is pointing at interactable objects then set it to true
        else 
            _isValidTarget = true;

        rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit);

        TeleportRequest request = new TeleportRequest();

        // if pointing at a teleportation area
        if (interactables[0].interactionLayers == 4)
        {
            request.destinationPosition = hit.point;
        }
        // if pointing at a teleporation anchor
        else if (interactables[0].interactionLayers == 16)
        {
            request.destinationPosition = hit.transform.GetChild(0).transform.position;
        }
        // if the ray is on an invalid object
        else
        {
            _isValidTarget = false;
        }
        // if the activate button is released while in teleport mode it means we want to teleport
        if (_isPressed == false)
        {
            provider.QueueTeleportRequest(request);
            TurnOffTeleport();
        }
    }

    private void OnTeleportActivate(InputAction.CallbackContext context)
    {
        // if the currently pointed at area is not a valid teleport target, we don't want to teleport
        if (_isValidTarget == false)
        {
            return;
        }
        // if it is a valid target then we want to initiate teleport stuff
        else
        {
            if (_isPressed == false)
                _isPressed = true;
            else
                _isPressed = false;
            // toggle _isActive to true
            _isActive = true;
            // change the ray interactor to the teleport settings
            rayInteractor.lineType = XRRayInteractor.LineType.ProjectileCurve;
            rayInteractor.interactionLayers = teleportationLayers;
        }
    }

    private void OnTeleportCancel(InputAction.CallbackContext context)
    {
        TurnOffTeleport();
    }

    private void TurnOffTeleport()
    {
        // the active button was "released"
        _isPressed = false;
        // the teleportation ray should be turned off
        _isActive = false;
        rayInteractor.lineType = XRRayInteractor.LineType.StraightLine;
        rayInteractor.interactionLayers = initialInteractionLayers;
    }
}