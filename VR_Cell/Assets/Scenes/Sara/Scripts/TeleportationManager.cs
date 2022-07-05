using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    [Header("Right Hand Teleportation Controller")]
    [SerializeField] private InputActionProperty activate;
    [SerializeField] private InputActionProperty cancel;
    [SerializeField] private InputActionProperty thumbstick;
    // [SerializeField] private InputActionProperty gripModeActivate;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private InteractionLayerMask teleportationLayers;
    [SerializeField] private TeleportationProvider provider;
    
    private bool _isActive;
    private InteractionLayerMask initialInteractionLayers;
    private List<IXRInteractable> interactables = new List<IXRInteractable>();

    // Start is called before the first frame update
    void Start()
    {
        activate.action.Enable();
        cancel.action.Enable();
        thumbstick.action.Enable();
        // gripModeActivate.action.Enable();

        activate.action.performed += OnTeleportActivate;
        cancel.action.performed += OnTeleportCancel;

        initialInteractionLayers = rayInteractor.interactionLayers;
        
    }

    // // Update is called once per frame
    void Update()
    {
        if (!_isActive)
            return;
        
        if (thumbstick.action.triggered)
            return;

        rayInteractor.GetValidTargets(interactables);

        if (interactables.Count == 0)
        {
            TurnOffTeleport();
            return;
        }

        rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit);

        TeleportRequest request = new TeleportRequest();

        // for teleportation area
        if (interactables[0].interactionLayers == 4)
        {
            Debug.Log("Teleportation Area");
            request.destinationPosition = hit.point;
        }
        // for teleporation anchor
        else if (interactables[0].interactionLayers == 16)
        {
            Debug.Log("Teleportation Anchor");
            request.destinationPosition = hit.transform.GetChild(0).transform.position;
        }

        provider.QueueTeleportRequest(request);
        TurnOffTeleport();

    }

    private void OnTeleportActivate(InputAction.CallbackContext context)
    {
        // if (gripModeActivate.action.phase != InputActionPhase.Performed)
        // {
            _isActive = true;
            rayInteractor.lineType = XRRayInteractor.LineType.ProjectileCurve;
            rayInteractor.interactionLayers = teleportationLayers;
        // }

    }

    private void OnTeleportCancel(InputAction.CallbackContext context)
    {
        TurnOffTeleport();
    }

    private void TurnOffTeleport()
    {
        _isActive = false;
        rayInteractor.lineType = XRRayInteractor.LineType.StraightLine;
        rayInteractor.interactionLayers = initialInteractionLayers;
    }
}
