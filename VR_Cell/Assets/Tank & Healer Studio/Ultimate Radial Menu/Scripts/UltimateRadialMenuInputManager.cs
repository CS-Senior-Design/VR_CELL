/* UltimateRadialMenuInputManager.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
#endif

public class UltimateRadialMenuInputManager : MonoBehaviour
{
	public static UltimateRadialMenuInputManager Instance
	{
		get;
		private set;
	}
	protected Camera mainCamera;
	public class UltimateRadialMenuInfomation
	{
		public UltimateRadialMenuInputManager inputManager;
		public UltimateRadialMenu radialMenu;
		public bool lastRadialMenuState = false;

		// MOUSE INPUT VARIABLES //
		public Vector2 previousMouseInput = Vector2.zero;
		
		// CENTER SCREEN VARIABLES //
		public float interactHoldTime = 0.0f;
		public int currentButtonIndex = -1;
		public Image inputTracker;

		// CUSTOM RAYCAST VARIABLES //
		public bool raycastInputCalculated;
		public Vector2 raycastInput;
		public float raycastDistance;

		// TOUCH SETTINGS //
		public float currentHoldTime = 0.0f;
		public int interactFingerID = -1;
		public bool touchActivatedRadialMenu = false;
		
		/// <summary>
		/// [INTERNAL] Used only for the Touch Input when using Dynamic Positioning.
		/// </summary>
		public void ResetMenuPosition ()
		{
			if( !touchActivatedRadialMenu )
				return;

			touchActivatedRadialMenu = false;

			if( radialMenu != null )
				radialMenu.ResetPosition();
		}

		/// <summary>
		/// [INTERNAL] Used only for the Center Screen Input if the canvas/radial menu changes size at runtime.
		/// </summary>
		public void UpdatePositioning ()
		{
			// If something is unassigned, just continue.
			if( inputTracker == null || radialMenu == null )
				return;

			// Set the size of the input tracker image.
			inputTracker.rectTransform.sizeDelta = radialMenu.BaseTransform.sizeDelta * inputManager.inputTrackerSize;
		}
	}
	public List<UltimateRadialMenuInfomation> UltimateRadialMenuInformations
	{
		get;
		private set;
	}
	LayerMask worldSpaceMask;
	bool customInputCalculated = false;
	bool customInputDown, customInputUp = false;
	public enum InputDevice
	{
		None,
		Mouse,
		Controller,
		Touch,
		CenterScreen,
		Other
	}
	public InputDevice CurrentInputDevice
	{
		get;
		private set;
	}

	// INTERACT SETTINGS //
	public enum InvokeAction
	{
		OnButtonDown,
		OnButtonClick
	}
	[Header( "Interact Settings" )]
	[Tooltip( "The action required to invoke the radial button." )]
	public InvokeAction invokeAction = InvokeAction.OnButtonDown;
	[Tooltip( "Determines whether or not the Ultimate Radial Menu will receive input when the Ultimate Radial Menu is released and disabled." )]
	public bool onMenuRelease = false;
	[Tooltip( "Determines if the Ultimate Radial Menu should be disabled when the interaction occurs. \n\nNOTE: World space radial menus will not be disabled on interact. They must be disabled manually." )]
	public bool disableOnInteract = false;
	public enum EnableMenuSetting
	{
		Toggle,
		Hold,
		Manual
	}
	[Tooltip( "Determines if this Input Manager should handle enabling and disabling the radial menus in the scene, and how that should be done." )]
	public EnableMenuSetting enableMenuSetting = EnableMenuSetting.Hold;

	// MOUSE SETTINGS //
	[Header( "Mouse and Keyboard Settings" )]
	[Tooltip( "Determines if mouse and keyboard input should be used to send to the Ultimate Radial Menu." )]
	public bool keyboardInput = true;
	public enum MouseInteractButtons
	{
		Left,
		Right,
		Both
	}
	[Tooltip( "The mouse button to be used to interact with the radial menus." )]
	public MouseInteractButtons mouseInteractButton;
	public enum KeyboardEnableKeys
	{
		EnableManually,
		Tab,
		LeftAlt,
		LeftControl,
		CapsLock,
		Tilde,
		Escape,
	}
	[Tooltip( "The keyboard key to be used to enable/disable the radial menus." )]
	public KeyboardEnableKeys keyboardEnableKey = KeyboardEnableKeys.Tab;

	// CONTROLLER SETTINGS //
	[Header( "Controller Settings" )]
	[Tooltip( "Determines if controller input should be used to send to the Ultimate Radial Menu." )]
	public bool controllerInput = false;
	[Tooltip( "Determines if the horizontal input should be inverted or not." )]
	public bool invertHorizontal = false;
	[Tooltip( "Determines if the vertical input should be inverted or not." )]
	public bool invertVertical = false;
#if ENABLE_INPUT_SYSTEM
	public enum Joysticks
	{
		Left,
		Right
	}
	[Tooltip( "The controller joystick to be used for navigating the radial menu." )]
	public Joysticks joystick = Joysticks.Left;
	public enum ControllerButtons
	{
		Nothing = 0,
		North = 1,
		South = 2,
		East = 4,
		West = 8,
		LeftJoystick = 16,
		LeftShoulder = 32,
		LeftTrigger = 64,
		RightJoystick = 128,
		RightShoulder = 256,
		RightTrigger = 512,
		Start = 1024,
		Select = 2048,
		DpadUp = 4096,
		DpadDown = 8192,
		DpadLeft = 16384,
		DpadRight = 32768,
	}
	[Tooltip( "The buttons to be used for interacting with the radial menu buttons." )]
	public ControllerButtons interactButtons;
	[Tooltip( "The buttons to be used for enabling the radial menu." )]
	public ControllerButtons enableButtons;
#else
	[Tooltip( "The input key for the controller horizontal axis." )]
	public string horizontalAxisController = "Horizontal";
	[Tooltip( "The input key for the controller vertical axis." )]
	public string verticalAxisController = "Vertical";
	[Tooltip( "The input key for the controller button interaction." )]
	public string interactButtonController = "Cancel";
	[Tooltip( "The input key used for enabling and disabling the Ultimate Radial Menu." )]
	public string enableButtonController = "Submit";
#endif

	// TOUCH SETTINGS //
	[Header( "Touch Settings" )]
	[Tooltip( "Determines if touch input should be used to send to the Ultimate Radial Menu." )]
	public bool touchInput = false;
	[Tooltip( "Should the radial menu move to the initial touch position?" )]
	public bool dynamicPositioning = false;
	[Range( 0.0f, 2.0f )]
	[Tooltip( "The activation radius for enabling the menu." )]
	public float activationRadius = 0.25f;
	[Tooltip( "Time in seconds that the user needs to hold the touch within the activation radius." )]
	public float activationHoldTime = 0.25f;
	bool touchInformationReset = true;

	// CENTER SCREEN SETTINGS //
	[Header( "Center Screen Settings" )]
	[Tooltip( "Determines if the menu should activated by the center of the screen." )]
	public bool centerScreenInput = false;
	[SerializeField]
	[Tooltip( "Should hovering over the menu for an amount of time interact with the menu?" )]
	private bool interactOnHover = false;
	[SerializeField]
	[Tooltip( "Time is seconds the player must hover over a button to interact with it." )]
	private float interactHoverTime = 1.0f;
	[Tooltip( "Should the calculations use two cameras in order to calculate the center of where the player is looking?" )]
	[SerializeField]
	private bool virtualReality = false;
	[SerializeField]
	private Camera leftEyeCamera, rightEyeCamera;
	[SerializeField]
	[Tooltip( "Should this Input Manager use a image to display the position of the input?" )]
	private bool trackInputPosition = false;
	[SerializeField]
	[Tooltip( "The sprite to be used for tracking the input." )]
	private Sprite trackInputSprite = null;
	[SerializeField]
	[Range( 0.01f, 0.25f )]
	[Tooltip( "The size of the input tracker image." )]
	private float inputTrackerSize = 0.1f;
	[SerializeField]
	[Tooltip( "When enabled, the input manager will only send input to the menus when the cameras are in front of the menu, not behind." )]
	private bool forwardInputOnly = false;
	[SerializeField]
	[Tooltip( "Should the input manager enable the menu when the input is within range?" )]
	private bool enableWhenInRange = false;

	// CUSTOM INPUT SETTINGS //
	[ Header( "Custom Input Settings" )]
	public bool customInput = false;

#if ENABLE_INPUT_SYSTEM
	// CUSTOM CONTROLLER INPUT SETTINGS //
	public bool customControllerInput = false;
	public InputAction customControllerJoystick;
	public InputAction customControllerInteract;
	public InputAction customControllerEnable;
	bool customControllerCalculated = false;
	bool customControllerInputDown, customControllerInputUp;
	bool customControllerEnableMenu, customControllerDisableMenu;
#endif


	protected virtual void Awake ()
	{
		// If this input manager is not located on the event system or an Ultimate Radial Menu object...
		if( !GetComponent<EventSystem>() && !GetComponent<UltimateRadialMenu>() )
		{
			// Log an error to the user explaining the issue and what to do to fix it.
			Debug.LogError( "Ultimate Radial Menu Input Manager\nThis component is not attached to the EventSystem in your scene or an Ultimate Radial Menu component. Please make sure that you have only one Ultimate Radial Menu Input Manager in your scene and that it is located on the EventSystem, unless you want unique controller input then place the Ultimate Radial Menu Input Manager on the Ultimate Radial Menu component that you want to have unique controller input." );

			// Destroy this component and return.
			Destroy( this );
			return;
		}

		// If this gameObject has the EventSystem component...
		if( GetComponent<EventSystem>() )
		{
			// If the current instance is assigned, and the object still exists and is in hierarchy...
			if( Instance != null && Instance.gameObject != null && Instance.gameObject.activeInHierarchy )
			{
				// Then destroy this component so that we will continue to use our current input manager and return.
				Destroy( this );
				return;
			}

			// Assign this component as the current instance.
			Instance = this;
		}

		// Reset the Informations list.
		UltimateRadialMenuInformations = new List<UltimateRadialMenuInfomation>();

		// Store the LayerMask for the UI so that it can be used for world space menus.
		worldSpaceMask = LayerMask.GetMask( "UI" );
	}

	protected virtual void Start ()
	{
		// Set the main camera for calculations.
		UpdateCamera();

#if ENABLE_INPUT_SYSTEM
		// If the user has touch input enabled, then enable the EnhancedTouchSupport from the Input System.
		if( touchInput )
			EnhancedTouchSupport.Enable();

		// If the user wants to have custom controller input...
		if( customControllerInput )
		{
			// If the controller joystick is assigned, then enable it.
			if( customControllerJoystick != null && customControllerJoystick.bindings.Count > 0 )
				customControllerJoystick.Enable();

			// If the controller interact buttons are assigned, then enable the buttons and register the appropriate callbacks.
			if( customControllerInteract != null && customControllerInteract.bindings.Count > 0 )
			{
				customControllerInteract.Enable();
				customControllerInteract.performed += CustomControllerInputDown;
				customControllerInteract.canceled += CustomControllerInputUp;
			}

			// If the user wants to enable the menus from this input manager and the controller enable/disable buttons are assigned, then enable the buttons and register the appropriate callbacks.
			if( enableMenuSetting != EnableMenuSetting.Manual && customControllerEnable != null && customControllerEnable.bindings.Count > 0 )
			{
				customControllerEnable.Enable();
				customControllerEnable.performed += CustomControllerEnable;
				customControllerEnable.canceled += CustomControllerDisable;
			}
		}
#endif
	}

	/// <summary>
	/// [INTERNAL] Called by each Ultimate Radial Menu.
	/// </summary>
	public void AddRadialMenuToList ( UltimateRadialMenu radialMenu, ref UltimateRadialMenuInputManager inputManager )
	{
		// Create a new information variable with this radial menu.
		UltimateRadialMenuInfomation newInfo = new UltimateRadialMenuInfomation() { radialMenu = radialMenu, inputManager = this };

		// If the user has Center Screen Input selected and they want to track the input position...
		if( centerScreenInput && trackInputPosition && trackInputSprite != null )
		{
			// Create a new object for the radial menu that will be used as the tracker.
			GameObject inputTracker = new GameObject( "Input Tracker" );
			RectTransform inputTrackerRect = inputTracker.AddComponent<RectTransform>();
			inputTracker.AddComponent<CanvasRenderer>();
			Image inputTrackerImage = inputTracker.AddComponent<Image>();
			inputTrackerImage.sprite = trackInputSprite;
			newInfo.inputTracker = inputTrackerImage;
			inputTrackerRect.transform.SetParent( radialMenu.BaseTransform );
			inputTrackerRect.SetAsLastSibling();
			inputTrackerRect.sizeDelta = radialMenu.BaseTransform.sizeDelta * inputTrackerSize;
			inputTrackerRect.localPosition = Vector3.zero;
			inputTrackerRect.localRotation = Quaternion.identity;
			inputTrackerRect.localScale = Vector3.one;
			
			radialMenu.OnUpdatePositioning += newInfo.UpdatePositioning;
		}

		// If the user wants to use touch input, then store this radial menu.
		if( touchInput )
			radialMenu.OnRadialMenuDisabled += newInfo.ResetMenuPosition;

		// Add this radial menu to the list for calculations.
		UltimateRadialMenuInformations.Add( newInfo );

		inputManager = this;
	}

	/// <summary>
	/// Updates the current camera for calculations.
	/// </summary>
	protected void UpdateCamera ()
	{
		// Find all the cameras in the scene.
		Camera[] sceneCameras = FindObjectsOfType<Camera>();

		// Loop through each camera.
		for( int i = 0; i < sceneCameras.Length; i++ )
		{
			// If the camera gameObject is active and the camera component is enabled...
			if( sceneCameras[ i ].gameObject.activeInHierarchy && sceneCameras[ i ].enabled )
			{
				// Set this camera to the main camera.
				mainCamera = sceneCameras[ i ];

				// If this camera is tagged as MainCamera, then break the loop. Otherwise, keep looking for a MainCamera.
				if( sceneCameras[ i ].tag == "MainCamera" )
					break;
			}
		}
	}

	/// <summary>
	/// Sets the camera to the provided camera parameter for calculations.
	/// </summary>
	/// <param name="newMainCamera">The new camera to use for calculations.</param>
	public void SetMainCamera ( Camera newMainCamera )
	{
		mainCamera = newMainCamera;
	}

	/// <summary>
	/// Sets the VR cameras for center screen calculations.
	/// </summary>
	/// <param name="newLeftEyeCamera">The new camera assigned to the left eye of the VR device.</param>
	/// <param name="newRightEyeCamera">The new camera assigned to the right eye of the VR device.</param>
	public void SetCamerasVR ( Camera newLeftEyeCamera, Camera newRightEyeCamera )
	{
		leftEyeCamera = newLeftEyeCamera;
		rightEyeCamera = newRightEyeCamera;
	}

	/// <summary>
	/// Performs a physics raycast using the provided input information to see if the input collides with any world space menus.
	/// </summary>
	protected void RaycastWorldSpaceRadialMenu ( ref Vector2 input, ref float distance, Vector2 rayOrigin, int radialMenuIndex )
	{
		// If the current radial menu is not used in world space, then just return.
		if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
			return;

		// If the main camera is null, not active, or the camera component is not enabled, then update the camera reference.
		if( mainCamera == null || !mainCamera.gameObject.activeInHierarchy || !mainCamera.enabled )
			UpdateCamera();

		// Cast a ray from the mouse position.
		Ray ray = mainCamera.ScreenPointToRay( rayOrigin );

		// Temporary hit variable to store hit information.
		RaycastHit hit;

		// Raycast with the calculated information.
		if( Physics.Raycast( ray, out hit, Mathf.Infinity, worldSpaceMask ) )
		{
			// If the collider that was hit is this radial menu...
			if( hit.collider.gameObject == UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.gameObject )
			{
				// Configure the local 3D Position of hit.
				Vector3 localHitPosition = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.InverseTransformPoint( hit.point );

				// Assign the input to being the local input value.
				input = localHitPosition;

				// Configure the distance of the input position from center.
				distance = Vector3.Distance( Vector2.zero, localHitPosition );
			}
		}
	}

	/// <summary>
	/// [INTERNAL] Checks and handles how the radial menu is enabled and disabled.
	/// </summary>
	protected void CheckEnableMenu ( bool buttonPressed, bool buttonReleased, int radialMenuIndex )
	{
		// If the user wants to enable the menu only when holding the button...
		if( enableMenuSetting == EnableMenuSetting.Hold )
		{
			// If the button is pressed and the radial menu isn't active, then enable it.
			if( buttonPressed && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
				UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.EnableRadialMenu();

			// If the button has been released and the radial menu is active, then disable it.
			if( buttonReleased && UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
				UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.DisableRadialMenu();
		}
		// Else the user wants to toggle the enabled state of the radial menu.
		else
		{
			// If the button has been pressed...
			if( buttonPressed )
			{
				// If the radial menu is currently disabled, then enable it.
				if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
					UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.EnableRadialMenu();
				// Else disable the menu.
				else
					UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.DisableRadialMenu();
			}
		}
	}

	private void Update ()
	{
		// Loop through each of the radial menus.
		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
		{
			// If the menu is null, then it must have been deleted...
			if( UltimateRadialMenuInformations[ i ].radialMenu == null )
			{
				// Update the list and break the loop to avoid errors.
				UltimateRadialMenuInformations.RemoveAt( i );

				break;
			}

			// Booleans to check if we want to enable or disable the radial menu this frame.
			bool enableMenu = false;
			bool disableMenu = false;
			bool inputDown = false;
			bool inputUp = false;

			// This is for the current input of the selected Input Type. ( Mouse input for Keyboard controls, and joystick input for controllers )
			Vector2 input = Vector2.zero;

			// This will store the distance from the center of the radial menu to help calculate if the input is within range.
			float distance = 0.0f;

			// If the user wants to use keyboard input then run the MouseAndKeyboardInput function.
			if( keyboardInput )
				MouseAndKeyboardInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user wants to use controller input then run the ControllerInput function.
			if( controllerInput )
				ControllerInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user wants to use touch input, then call the TouchInput function.
			if( touchInput )
				TouchInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user wants to use any VR devices then run the VirtualRealityInput function.
			if( centerScreenInput )
				CenterScreenInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the user has created custom input, then call that here.
			if( customInput )
				CustomInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

#if ENABLE_INPUT_SYSTEM
			// If the user has assigned custom controller input, then call that.
			if( customControllerInput )
				CustomControllerInput( ref enableMenu, ref disableMenu, ref input, ref distance, ref inputDown, ref inputUp, i );

			// If the custom controller input was calculated...
			if( customControllerCalculated )
			{
				// Set the current input device for reference.
				CurrentInputDevice = InputDevice.Controller;

				// If the custom input has been set, then copy it.
				if( customControllerInputDown )
					inputDown = true;

				// If the custom input up was set this frame, copy it.
				if( customControllerInputUp )
					inputUp = true;

				// If the enable button was caught, copy it.
				if( customControllerEnableMenu )
					enableMenu = true;

				// If the disable was caught, copy.
				if( customControllerDisableMenu )
					disableMenu = true;
			}
#endif

			// If the user has sent in some raycast input this frame...
			if( UltimateRadialMenuInformations[ i ].raycastInputCalculated )
			{
				// Set the current input device to other for reference.
				CurrentInputDevice = InputDevice.Other;

				// If the input value is assigned, then copy the custom input values.
				if( UltimateRadialMenuInformations[ i ].raycastInput != Vector2.zero )
				{
					input = UltimateRadialMenuInformations[ i ].raycastInput;
					distance = UltimateRadialMenuInformations[ i ].raycastDistance;
				}

				// Reset the input data.
				UltimateRadialMenuInformations[ i ].raycastInputCalculated = false;
				UltimateRadialMenuInformations[ i ].raycastInput = Vector2.zero;
				UltimateRadialMenuInformations[ i ].raycastDistance = 0.0f;
			}

			// If we want to activate the radial menu when we release the menu when hovering over a button...
			if( onMenuRelease )
			{
				// Check the last known radial menu state to see if it was active. If we are going to disable the menu on this frame and the last known state was true, then set interact to true.
				if( UltimateRadialMenuInformations[ i ].lastRadialMenuState && disableMenu )
					inputDown = inputUp = true;
			}
			
			// If the custom input was calculated this frame...
			if( customInputCalculated )
			{
				// Copy the values if they have been assigned.
				if( customInputDown )
					inputDown = true;
				if( customInputUp )
					inputUp = true;
			}

			// Send all of the calculations to the Ultimate Radial Menu to process.
			UltimateRadialMenuInformations[ i ].radialMenu.ProcessInput( input, distance, inputDown, inputUp );

			// If the user wants to enable the menus through the input manager, check for that.
			if( enableMenuSetting != EnableMenuSetting.Manual )
				CheckEnableMenu( enableMenu, disableMenu, i );

			// Store the last known state for calculations.
			UltimateRadialMenuInformations[ i ].lastRadialMenuState = UltimateRadialMenuInformations[ i ].radialMenu.RadialMenuActive;
		}

#if ENABLE_INPUT_SYSTEM
		// If the custom controller input was caught this frame...
		if( customControllerCalculated )
		{
			// Reset all the custom controller variables.
			customControllerCalculated = false;
			customControllerInputDown = false;
			customControllerInputUp = false;
			customControllerEnableMenu = false;
			customControllerDisableMenu = false;
		}
#endif

		// If the custom input was caught this frame...
		if( customInputCalculated )
		{
			// Reset the custom input.
			customInputCalculated = false;
			customInputDown = customInputUp = false;
		}
	}

	/// <summary>
	/// This function will catch input from the Mouse and Keyboard and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the MouseAndKeyboardInput function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void MouseAndKeyboardInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		Vector2 mousePosition = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
		// Store the mouse from the input system.
		Mouse mouse = InputSystem.GetDevice<Mouse>();

		// If the mouse couldn't be stored from the input system, then return.
		if( mouse == null )
			return;

		// Store the mouse position.
		mousePosition = mouse.position.ReadValue();

		// If the user doesn't want to exclusively use the right mouse button for interacting...
		if( mouseInteractButton != MouseInteractButtons.Right )
		{
			// Check the state of the left mouse button.
			if( mouse.leftButton.wasPressedThisFrame )
				inputDown = true;
			if( mouse.leftButton.wasReleasedThisFrame )
				inputUp = true;
		}

		// If the user doesn't want to exclusively use the left mouse button...
		if( mouseInteractButton != MouseInteractButtons.Left )
		{
			// Then check the state of the right mouse button.
			if( mouse.rightButton.wasPressedThisFrame )
				inputDown = true;
			if( mouse.rightButton.wasReleasedThisFrame )
				inputUp = true;
		}

		// Store the keyboard from the input system.
		Keyboard keyboard = InputSystem.GetDevice<Keyboard>();

		// If the user wants to enable the radial menus through this input manager, and the keyboard was found, and this is not a world space menu...
		if( enableMenuSetting != EnableMenuSetting.Manual && keyboardEnableKey != KeyboardEnableKeys.EnableManually && keyboard != null )
		{
			// Catch the input from the key that the user wants to use.
			switch( keyboardEnableKey )
			{
				default:
				case KeyboardEnableKeys.Tab:
				{
					enableMenu = keyboard.tabKey.wasPressedThisFrame;
					disableMenu = keyboard.tabKey.wasReleasedThisFrame;
				}
				break;
				case KeyboardEnableKeys.LeftAlt:
				{
					enableMenu = keyboard.leftAltKey.wasPressedThisFrame;
					disableMenu = keyboard.leftAltKey.wasReleasedThisFrame;
				}
				break;
				case KeyboardEnableKeys.LeftControl:
				{
					enableMenu = keyboard.leftCtrlKey.wasPressedThisFrame;
					if( !enableMenu )
						enableMenu = keyboard.leftCommandKey.wasPressedThisFrame;

					disableMenu = keyboard.leftCtrlKey.wasReleasedThisFrame;
					if( !disableMenu )
						disableMenu = keyboard.leftCommandKey.wasReleasedThisFrame;
				}
				break;
				case KeyboardEnableKeys.CapsLock:
				{
					enableMenu = keyboard.capsLockKey.wasPressedThisFrame;
					disableMenu = keyboard.capsLockKey.wasReleasedThisFrame;
				}
				break;
				case KeyboardEnableKeys.Tilde:
				{
					enableMenu = keyboard.backquoteKey.wasPressedThisFrame;
					disableMenu = keyboard.backquoteKey.wasReleasedThisFrame;
				}
				break;
				case KeyboardEnableKeys.Escape:
				{
					enableMenu = keyboard.escapeKey.wasPressedThisFrame;
					disableMenu = keyboard.escapeKey.wasReleasedThisFrame;
				}
				break;
			}
		}
#else
		// If the mouse is not present, then just return.
		if( !Input.mousePresent )
			return;

		// Store the mouse position.
		mousePosition = Input.mousePosition;

		// If the user doesn't want to exclusively use the right mouse button for interacting...
		if( mouseInteractButton != MouseInteractButtons.Right )
		{
			// Check the state of the left mouse button.
			if( Input.GetMouseButtonDown( 0 ) )
				inputDown = true;
			if( Input.GetMouseButtonUp( 0 ) )
				inputUp = true;
		}

		// If the user doesn't want to exclusively use the left mouse button...
		if( mouseInteractButton != MouseInteractButtons.Left )
		{
			// Then check the state of the right mouse button.
			if( Input.GetMouseButtonDown( 1 ) )
				inputDown = true;
			if( Input.GetMouseButtonUp( 1 ) )
				inputUp = true;
		}

		// If the keyboard enable button is assigned, and the menu is not in world space...
		if( enableMenuSetting != EnableMenuSetting.Manual && keyboardEnableKey != KeyboardEnableKeys.EnableManually )
		{
			switch( keyboardEnableKey )
			{
				default:
				case KeyboardEnableKeys.Tab:
				{
					enableMenu = Input.GetKeyDown( KeyCode.Tab );
					disableMenu = Input.GetKeyUp( KeyCode.Tab );
				}
				break;
				case KeyboardEnableKeys.LeftAlt:
				{
					enableMenu = Input.GetKeyDown( KeyCode.LeftAlt );
					disableMenu = Input.GetKeyUp( KeyCode.LeftAlt );
				}
				break;
				case KeyboardEnableKeys.LeftControl:
				{
					enableMenu = Input.GetKeyDown( KeyCode.LeftControl );
					disableMenu = Input.GetKeyUp( KeyCode.LeftControl );

					if( !enableMenu )
						enableMenu = Input.GetKeyDown( KeyCode.LeftCommand );

					if( !disableMenu )
						disableMenu = Input.GetKeyUp( KeyCode.LeftCommand );
				}
				break;
				case KeyboardEnableKeys.CapsLock:
				{
					enableMenu = Input.GetKeyDown( KeyCode.CapsLock );
					disableMenu = Input.GetKeyUp( KeyCode.CapsLock );
				}
				break;
				case KeyboardEnableKeys.Tilde:
				{
					enableMenu = Input.GetKeyDown( KeyCode.Tilde );
					disableMenu = Input.GetKeyUp( KeyCode.Tilde );
				}
				break;
				case KeyboardEnableKeys.Escape:
				{
					enableMenu = Input.GetKeyDown( KeyCode.Escape );
					disableMenu = Input.GetKeyUp( KeyCode.Escape );
				}
				break;
			}
		}
#endif
		// If the previous mouse input is the same as the current mouse position, the last known input device was not the mouse, and no input was captured from the mouse and keyboard, then return.
		if( UltimateRadialMenuInformations[ radialMenuIndex ].previousMouseInput == mousePosition && CurrentInputDevice != InputDevice.Mouse && !inputDown && !inputUp && !enableMenu && !disableMenu )
			return;

		// Set the current input device for reference.
		CurrentInputDevice = InputDevice.Mouse;

		// Store the current mouse position for reference on the next frame.
		UltimateRadialMenuInformations[ radialMenuIndex ].previousMouseInput = mousePosition;

		// If this radial menu is world space then send in the information to raycast from.
		if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
			RaycastWorldSpaceRadialMenu( ref input, ref distance, mousePosition, radialMenuIndex );
		// Else the radial menu is on the screen, so process mouse input.
		else
		{
			// Figure out the position of the input on the canvas. ( mouse position / canvas scale factor ) - ( half the canvas size );
			Vector2 inputPositionOnCanvas = ( mousePosition / UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.scaleFactor ) - ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.GetComponent<RectTransform>().sizeDelta / 2 );

			// Apply our new calculated input. ( input position - local position of the menu ) / ( half the menu size );
			input = ( inputPositionOnCanvas - ( Vector2 )UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition ) / ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.sizeDelta.x / 2 );

			// Configure the distance of the mouse position from the Radial Menu's base position.
			distance = Vector2.Distance( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition, inputPositionOnCanvas );
		}
	}

	/// <summary>
	/// This function will catch input from the Controller and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the ControllerInput function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void ControllerInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// Store a boolean to check if the input from the mouse & keyboard has been caught.
		bool inputModifiedThisFrame = false;

		// If any of the bool variables have been modified, then the mouse and keyboard is still the active input, so set the bool to true.
		if( enableMenu || disableMenu || inputDown || inputUp )
			inputModifiedThisFrame = true;

		// Store the horizontal and vertical axis of the targeted joystick axis.
		Vector2 controllerInput = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
		// Store the gamepad from the input system.
		Gamepad gamepad = InputSystem.GetDevice<Gamepad>();

		// If the gamepad is null, then just return.
		if( gamepad == null )
			return;

		// Store the input data of the stick determined by the user.
		controllerInput = joystick == Joysticks.Left ? gamepad.leftStick.ReadValue() : gamepad.rightStick.ReadValue();

		// Check the controller buttons for interacting.
		CheckControllerButtons( gamepad, interactButtons, ref inputDown, ref inputUp );

		// Check the controller buttons for enabling/disabling the menu if the user wants that.
		if( enableMenuSetting != EnableMenuSetting.Manual )
			CheckControllerButtons( gamepad, enableButtons, ref enableMenu, ref disableMenu );
#else
		// Store the horizontal and vertical axis of the targeted joystick axis.
		controllerInput = new Vector2( Input.GetAxis( horizontalAxisController ), Input.GetAxis( verticalAxisController ) );

		// If the activation action is set to being the press of a button on the controller...
		if( Input.GetButtonDown( interactButtonController ) )
			inputDown = true;
		else if( Input.GetButtonUp( interactButtonController ) )
			inputUp = true;

		// If the user has a enable key assigned...
		if( enableMenuSetting != EnableMenuSetting.Manual && enableButtonController != string.Empty )
		{
			// Check for the Enable and Disable button keys and set the enable or disable booleans accordingly.
			if( Input.GetButtonDown( enableButtonController ) )
				enableMenu = true;
			else if( Input.GetButtonUp( enableButtonController ) )
				disableMenu = true;
		}
#endif
		// If the input had not been modified from the Mouse & Keyboard function before this one, then check any of the bool variables. If they have changed, set the current input device to controller for reference.
		if( !inputModifiedThisFrame && ( enableMenu || disableMenu || inputDown || inputUp ) )
			CurrentInputDevice = InputDevice.Controller;

		// If the controller input is not zero...
		if( controllerInput != Vector2.zero )
		{
			// Set the current input device for reference.
			CurrentInputDevice = InputDevice.Controller;

			// If the user wants to invert the horizontal axis, then multiply by -1.
			if( invertHorizontal )
				controllerInput.x *= -1;

			// If the user wants to invert the vertical axis, then do that here.
			if( invertVertical )
				controllerInput.y *= -1;

			// Set the input to what was calculated.
			input = controllerInput;

			// Since this is a controller, we want to make sure that our input distance feels right, so here we will temporarily store the distance before modification.
			float tempDist = Vector2.Distance( Vector2.zero, controllerInput );

			// If the temporary distance is greater than the minimum range, then the distance doesn't matter. All we want to send to the radial menu is that it is perfectly in range, so make the distance exactly in the middle of the min and max.
			if( tempDist >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.minRange )
				distance = Mathf.Lerp( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange, UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange, 0.5f );
		}
	}

#if ENABLE_INPUT_SYSTEM
	/// <summary>
	/// [INTERNAL] This function checks the gamepad for input.
	/// </summary>
	private void CheckControllerButtons ( Gamepad gamepad, ControllerButtons buttonsToCheck, ref bool buttonPress, ref bool buttonRelease )
	{
		// Check for the button that the user has for the input.
		switch( buttonsToCheck )
		{
			// Check for North, South, East and West buttons.
			default:
			case ControllerButtons.North:
			{
				if( !buttonPress )
					buttonPress = gamepad.buttonNorth.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.buttonNorth.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.South:
			{
				if( !buttonPress )
					buttonPress = gamepad.buttonSouth.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.buttonSouth.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.East:
			{
				if( !buttonPress )
					buttonPress = gamepad.buttonEast.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.buttonEast.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.West:
			{
				if( !buttonPress )
					buttonPress = gamepad.buttonWest.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.buttonWest.wasReleasedThisFrame;
			}
			break;
			// Check for left side controller buttons.
			case ControllerButtons.LeftJoystick:
			{
				if( !buttonPress )
					buttonPress = gamepad.leftStickButton.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.leftStickButton.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.LeftShoulder:
			{
				if( !buttonPress )
					buttonPress = gamepad.leftShoulder.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.leftShoulder.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.LeftTrigger:
			{
				if( !buttonPress )
					buttonPress = gamepad.leftTrigger.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.leftTrigger.wasReleasedThisFrame;
			}
			break;
			// Check for right side controller buttons.
			case ControllerButtons.RightJoystick:
			{
				if( !buttonPress )
					buttonPress = gamepad.rightStickButton.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.rightStickButton.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.RightShoulder:
			{
				if( !buttonPress )
					buttonPress = gamepad.rightShoulder.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.rightShoulder.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.RightTrigger:
			{
				if( !buttonPress )
					buttonPress = gamepad.rightTrigger.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.rightTrigger.wasReleasedThisFrame;
			}
			break;
			// Check for Start and Select buttons.
			case ControllerButtons.Start:
			{
				if( !buttonPress )
					buttonPress = gamepad.startButton.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.startButton.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.Select:
			{
				if( !buttonPress )
					buttonPress = gamepad.selectButton.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.selectButton.wasReleasedThisFrame;
			}
			break;
			// Check for Dpad directional buttons.
			case ControllerButtons.DpadUp:
			{
				if( !buttonPress )
					buttonPress = gamepad.dpad.up.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.dpad.up.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.DpadDown:
			{
				if( !buttonPress )
					buttonPress = gamepad.dpad.down.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.dpad.down.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.DpadLeft:
			{
				if( !buttonPress )
					buttonPress = gamepad.dpad.left.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.dpad.left.wasReleasedThisFrame;
			}
			break;
			case ControllerButtons.DpadRight:
			{
				if( !buttonPress )
					buttonPress = gamepad.dpad.right.wasPressedThisFrame;

				if( !buttonRelease )
					buttonRelease = gamepad.dpad.right.wasReleasedThisFrame;
			}
			break;
		}
	}

	/// <summary>
	/// This function will catch the custom joystick input and send it back to the update function.
	/// </summary>
	/// <param name="enableMenu">Not used.</param>
	/// <param name="disableMenu">Not used.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="inputDown">Not used.</param>
	/// <param name="inputUp">Not used.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void CustomControllerInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// If the joystick is actually being used...
		if( customControllerJoystick.ReadValue<Vector2>() != Vector2.zero )
		{
			// Set the current input device for reference.
			CurrentInputDevice = InputDevice.Controller;

			// Store the input data.
			input = customControllerJoystick.ReadValue<Vector2>();

			// Since this is a controller, we want to make sure that our input distance feels right, so here we will temporarily store the distance before modification.
			float tempDist = Vector2.Distance( Vector2.zero, input );

			// If the temporary distance is greater than the minimum range, then the distance doesn't matter. All we want to send to the radial menu is that it is perfectly in range, so make the distance exactly in the middle of the min and max.
			if( tempDist >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.minRange )
				distance = Mathf.Lerp( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange, UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange, 0.5f );
		}
	}

	/// <summary>
	/// [INTERNAL] For Unity's new Input System.
	/// </summary>
	private void CustomControllerInputDown ( InputAction.CallbackContext obj )
	{
		customControllerCalculated = true;
		customControllerInputDown = obj.performed && obj.ReadValue<float>() == 1;
	}

	/// <summary>
	/// [INTERNAL] For Unity's new Input System.
	/// </summary>
	private void CustomControllerInputUp ( InputAction.CallbackContext obj )
	{
		customControllerCalculated = true;
		customControllerInputUp = obj.canceled;
	}

	/// <summary>
	/// [INTERNAL] For Unity's new Input System.
	/// </summary>
	private void CustomControllerEnable ( InputAction.CallbackContext obj )
	{
		customControllerCalculated = true;
		customControllerEnableMenu = obj.performed && obj.ReadValue<float>() == 1;
	}

	/// <summary>
	/// [INTERNAL] For Unity's new Input System.
	/// </summary>
	private void CustomControllerDisable ( InputAction.CallbackContext obj )
	{
		customControllerCalculated = true;
		customControllerDisableMenu = obj.canceled;
	}
#endif

	/// <summary>
	/// This function will catch touch input and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void TouchInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		int touchCount = 0;

#if ENABLE_INPUT_SYSTEM
		touchCount = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers.Count;
#else
		touchCount = Input.touchCount;
#endif
		// If there are touches on the screen...
		if( touchCount > 0 )
		{
			CurrentInputDevice = InputDevice.Touch;

			// If the touch information is reset, then set to false so that it will be reset when there are no touches on the screen.
			if( touchInformationReset )
				touchInformationReset = false;

			for( int i = 0; i < touchCount; i++ )
			{
				Vector2 touchPosition = Vector2.zero;
				bool touchBegan = false;
				bool touchEnded = false;
				int fingerId = -1;

#if ENABLE_INPUT_SYSTEM
				touchPosition = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers[ i ].screenPosition;
				touchBegan = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers[ i ].currentTouch.phase == UnityEngine.InputSystem.TouchPhase.Began;
				touchEnded = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers[ i ].currentTouch.phase == UnityEngine.InputSystem.TouchPhase.Ended;
				fingerId = UnityEngine.InputSystem.EnhancedTouch.Touch.activeFingers[ i ].index;
#else
				touchPosition = Input.GetTouch( i ).position;
				touchBegan = Input.GetTouch( i ).phase == TouchPhase.Began;
				touchEnded = Input.GetTouch( i ).phase == TouchPhase.Ended;
				fingerId = Input.GetTouch( i ).fingerId;
#endif

				// If a finger id has been stored, and this finger id is not the same as the stored finger id, then continue.
				if( UltimateRadialMenuInformations[ radialMenuIndex ].interactFingerID >= 0 && UltimateRadialMenuInformations[ radialMenuIndex ].interactFingerID != fingerId )
					continue;

				// Configure the menu radius for calculations.
				float menuRadius = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.sizeDelta.x / 2;

				// Store the touch position on the canvas. ( touch position / canvas scale factor ) - ( half the canvas size );
				Vector2 touchPositionOnCanvas = ( touchPosition / UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.scaleFactor ) - ( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.ParentCanvas.GetComponent<RectTransform>().sizeDelta / 2 );

				// Temporary input variable for calculations.
				Vector2 modInput = Vector2.zero;

				// Temporary distance variable for calculations.
				float dist = 0.0f;

				// If the radial menu is used in world space, then raycast the input.
				if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
					RaycastWorldSpaceRadialMenu( ref modInput, ref dist, touchPosition, radialMenuIndex );
				// Else calculate the input.
				else
				{
					// By subtracting the mouse position from the radial menu's position we get a relative number. Then we divide by the height of the screen space to give us an easier and more consistent number to work with.
					modInput = ( touchPositionOnCanvas - ( Vector2 )UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition ) / menuRadius;

					// Configure the distance of the mouse position from the Radial Menu's base position.
					dist = Vector2.Distance( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.localPosition, touchPositionOnCanvas );
				}

				// If the input phase began, then store the finger id.
				if( touchBegan )
				{
					// If the input is within distance of the target, then store the finger id.
					if( enableMenuSetting != EnableMenuSetting.Manual && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu && dist < menuRadius * activationRadius )
						UltimateRadialMenuInformations[ radialMenuIndex ].interactFingerID = fingerId;

					// If the radial menu is active...
					if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
					{
						// Configure a deactivation radius by using the radial menu maxRange.
						float deactivationRadius = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.maxRange;

						// If the user wants the input radius to be infinite then set that here.
						if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.infiniteMaxRange )
							deactivationRadius = Mathf.Infinity;

						// Set the input to down since the touch began.
						inputDown = true;

						// If the distance is within the input range of the radial menu, then store the finger id.
						if( dist > UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange && dist < UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange )
							UltimateRadialMenuInformations[ radialMenuIndex ].interactFingerID = fingerId;
						// Else if the distance of the input is out of the deactivation range.
						else if( enableMenuSetting != EnableMenuSetting.Manual && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu && activationRadius > 0 && ( dist > menuRadius * deactivationRadius || dist < UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange ) )
							enableMenu = disableMenu = true;
					}
				}

				// If the finger id is still not stored, then just continue to the next touch.
				if( UltimateRadialMenuInformations[ radialMenuIndex ].interactFingerID == -1 )
					continue;

				// If the radial menu is not active and not in the middle of a transition...
				if( enableMenuSetting != EnableMenuSetting.Manual && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.InTransition )
				{
					// If the calculated distance is within the activation range...
					if( dist < menuRadius * activationRadius )
					{
						// Increases the current hold time for calculations to enable.
						UltimateRadialMenuInformations[ radialMenuIndex ].currentHoldTime += Time.deltaTime;

						// If the current hold time has reached the target time...
						if( UltimateRadialMenuInformations[ radialMenuIndex ].currentHoldTime >= activationHoldTime )
						{
							// Reset the current hold time, and enable the menu.
							UltimateRadialMenuInformations[ radialMenuIndex ].currentHoldTime = 0.0f;
							enableMenu = true;

							// Since the radial menu was enabled using text, set touchActivatedRadialMenu to true.
							UltimateRadialMenuInformations[ radialMenuIndex ].touchActivatedRadialMenu = true;

							// If the user wants to move to the touch position, move to that position now.
							if( dynamicPositioning )
								UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.SetPosition( touchPositionOnCanvas, true );
						}
					}
				}
				else
				{
					// Set the current input device for reference.
					CurrentInputDevice = InputDevice.Touch;

					// Store the input to send to the radial menu.
					input = modInput;

					// Store the distance as well.
					distance = dist;
				}

				// If the touch has ended...
				if( touchEnded )
				{
					// Reset the finger id.
					UltimateRadialMenuInformations[ radialMenuIndex ].interactFingerID = -1;

					// Reset the current hold time.
					UltimateRadialMenuInformations[ radialMenuIndex ].currentHoldTime = 0.0f;

					// Set input down and up to true so that the interact function will be called on the button.
					inputDown = inputUp = true;

					// If the input is not over a button when released, then just disable the menu.
					if( enableMenuSetting == EnableMenuSetting.Hold && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu && activationRadius > 0 )
						disableMenu = true;
				}
			}
		}
		else
		{
			// If the touch information has not been reset...
			if( !touchInformationReset )
			{
				// Set touchInformationReset so that it will not be reset again until there are touches on the screen again.
				touchInformationReset = true;

				// Loop through each information and reset the values.
				for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
				{
					UltimateRadialMenuInformations[ i ].currentHoldTime = 0.0f;
					UltimateRadialMenuInformations[ i ].interactFingerID = -1;
				}
			}
		}
	}

	/// <summary>
	/// This function will catch input from the center of the screen and VR device and modify the information to send back to the Update function.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in this function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void CenterScreenInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// If the radial menu is not being used in world space, then just return.
		if( !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.IsWorldSpaceRadialMenu )
			return;

		// Temporary variables to catch the input of the raycast.
		Vector2 centerScreenInput = Vector2.zero;
		float centerScreenDistance = 0.0f;

		// If the user wants to calculate with VR...
		if( virtualReality )
		{
			// If either of the eyes are unassigned, then inform the user and return.
			if( leftEyeCamera == null || rightEyeCamera == null )
			{
				Debug.LogError( "Ultimate Radial Menu - The left or right eye cameras are unassigned. Please ensure that they are assigned in the inspector. If you need to update them at runtime, please use the SetCamerasVR() function." );
				return;
			}

			// Create a ray from the center position from the two eyes.
			Ray ray = new Ray( Vector3.Lerp( leftEyeCamera.transform.position, rightEyeCamera.transform.position, 0.5f ), leftEyeCamera.transform.forward );

			// Temporary hit variable to store hit information.
			RaycastHit hit;

			// Raycast with the calculated information.
			if( Physics.Raycast( ray, out hit, Mathf.Infinity, worldSpaceMask ) )
			{
				// If the collider that was hit is this radial menu...
				if( hit.collider.gameObject == UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.gameObject )
				{
					// If the user doesn't want to only use forward input, or if the input is hitting the front of the radial menu... 
					if( !forwardInputOnly || UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.InverseTransformPoint( leftEyeCamera.transform.position ).z <= 0 )
					{
						// Configure the local 3D Position of hit.
						Vector3 localHitPosition = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.InverseTransformPoint( hit.point );

						// Assign the input to being the local input value.
						centerScreenInput = localHitPosition;

						// Configure the distance of the input position from center.
						centerScreenDistance = Vector3.Distance( Vector2.zero, localHitPosition );
					}
				}
			}
		}
		// Else just raycast from the center of the screen.
		else
		{
			// If the user doesn't want to only use forward input, or if the input is hitting the front of the radial menu... 
			if( !forwardInputOnly || UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.BaseTransform.InverseTransformPoint( mainCamera.transform.position ).z <= 0 )
				RaycastWorldSpaceRadialMenu( ref centerScreenInput, ref centerScreenDistance, new Vector3( Screen.width / 2, Screen.height / 2, 0 ), radialMenuIndex );
		}
		
		// If the calculated distance is assigned, and the distance is less than the max range of the radial menu, and it is currently disabled...
		if( centerScreenDistance > 0 && centerScreenDistance <= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange )
		{
			// If the user wants to enable the radial menus, and world space menus, set enableMenu to true so that the menu will be enabled.
			if( enableWhenInRange && !UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
				UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.EnableRadialMenu();

			// If the user wants to track the input position and the input tracker has been created, then enable the input tracker.
			if( trackInputPosition && UltimateRadialMenuInformations[ radialMenuIndex ].inputTracker != null )
				UltimateRadialMenuInformations[ radialMenuIndex ].inputTracker.enabled = true;
		}
		// Else if the calculated distance is out of range, or not assigned, and the radial menu is enabled...
		else if( ( centerScreenDistance > UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange || centerScreenDistance <= 0.0f ) && UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.RadialMenuActive )
		{
			// If the user wants to enable the radial menus, and world space menus...
			if( enableWhenInRange )
				UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.DisableRadialMenu();

			// If the user wants to track the position of the input, disable the image since the menu is disabled now.
			if( trackInputPosition && UltimateRadialMenuInformations[ radialMenuIndex ].inputTracker != null )
				UltimateRadialMenuInformations[ radialMenuIndex ].inputTracker.enabled = false;
		}

		// If the center screen input has been calculated...
		if( centerScreenInput != Vector2.zero )
		{
			// If the user wants to track the input position, then assign the position of the input.
			if( trackInputPosition && UltimateRadialMenuInformations[ radialMenuIndex ].inputTracker != null )
				UltimateRadialMenuInformations[ radialMenuIndex ].inputTracker.rectTransform.localPosition = centerScreenInput;

			// If the calculated distance is within range.
			if( centerScreenDistance >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange && centerScreenDistance <= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange )
			{
				// Then set the current input device to Center Screen for reference.
				CurrentInputDevice = InputDevice.CenterScreen;

				// Store the modified data for sending to the radial menu.
				input = centerScreenInput;
				distance = centerScreenDistance;
			}
			
			// If the user wants to interact on hover, and the time is assigned...
			if( interactOnHover && interactHoverTime > 0 )
			{
				// If the calculated distance above is within range of the menu...
				if( distance >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange && distance <= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange )
				{
					// If the radial menu has a button index that is assigned, and it is different from the stored button index...
					if( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex >= 0 && UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex != UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex )
					{
						// Store the current button index and reset the current hold time for reference.
						UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex = UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex;
						UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime = 0.0f;
					}

					// If the current button index is the same as the current button index on the radial menu...
					if( UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex == UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CurrentButtonIndex )
					{
						// If the interact hold time still needs to be increased...
						if( UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime <= interactHoverTime )
						{
							// Increase the hold timer.
							UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime += Time.deltaTime;

							// If the interact timer is above our max hold time, set the input for interact.
							if( UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime >= interactHoverTime )
								inputDown = inputUp = true;
						}
					}
				}
				// Else the input is not within range of the menu...
				else
				{
					// If the current hold time had been calculated at all, reset it.
					if( UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime > 0 )
						UltimateRadialMenuInformations[ radialMenuIndex ].interactHoldTime = 0.0f;

					// If the button index was stored, reset it also.
					if( UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex > 0 )
						UltimateRadialMenuInformations[ radialMenuIndex ].currentButtonIndex = -1;
				}
			}
		}
	}

	/// <summary>
	/// This function is a virtual void to allow for easy custom input logic.
	/// </summary>
	/// <param name="enableMenu">A reference to the enableMenu boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="disableMenu">A reference to the disableMenu boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="input">A reference to the input Vector2 from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="distance">A reference to the distance float from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="inputDown">A reference to the inputDown boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="inputUp">A reference to the inputUp boolean from the Update function. Any changes to this variable in the CustomInput function will be reflected in the Update function.</param>
	/// <param name="radialMenuIndex">The current index of the selected radial button.</param>
	public virtual void CustomInput ( ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown, ref bool inputUp, int radialMenuIndex )
	{
		// WARNING! This is not where you want to put your custom logic. Please check out our video tutorials for more information.
		// Video Tutorials: https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu
	}

	/// <summary>
	/// Send in custom raycast information to send to the Ultimate Radial Menus in the scene.
	/// </summary>
	/// <param name="rayStart">The start point of the ray.</param>
	/// <param name="rayEnd">The end point of the ray.</param>
	public void SendRaycastInput ( Vector3 rayStart, Vector3 rayEnd )
	{
		SendRaycastInput( rayStart, ( rayEnd - rayStart ).normalized, Vector3.Distance( rayStart, rayEnd ) );
	}

	/// <summary>
	/// Send in custom raycast information to send to the Ultimate Radial Menus in the scene.
	/// </summary>
	/// <param name="rayOrigin">The origin of the ray.</param>
	/// <param name="rayDirection">The direction of the ray.</param>
	/// <param name="rayDistance">The distance of the ray.</param>
	public void SendRaycastInput ( Vector3 rayOrigin, Vector3 rayDirection, float rayDistance )
	{
		// Loop through all the menu informations.
		for( int i = 0; i < UltimateRadialMenuInformations.Count; i++ )
		{
			// If the current radial menu is not used in world space, then just return.
			if( !UltimateRadialMenuInformations[ i ].radialMenu.IsWorldSpaceRadialMenu )
				return;

			// If the main camera is null, not active, or the camera component is not enabled, then update the camera reference.
			if( mainCamera == null || !mainCamera.gameObject.activeInHierarchy || !mainCamera.enabled )
				UpdateCamera();

			// Create a ray with the information provided.
			Ray ray = new Ray( rayOrigin, rayDirection );

			// Temporary hit variable to store hit information.
			RaycastHit hit;
			Vector2 input = Vector2.zero;
			float distance = 0.0f;

			// Raycast with the calculated information.
			if( Physics.Raycast( ray, out hit, rayDistance, worldSpaceMask ) )
			{
				// If the collider that was hit is this radial menu...
				if( hit.collider.gameObject == UltimateRadialMenuInformations[ i ].radialMenu.gameObject )
				{
					// Configure the local 3D Position of hit.
					Vector3 localHitPosition = UltimateRadialMenuInformations[ i ].radialMenu.BaseTransform.InverseTransformPoint( hit.point );

					// Assign the input to being the local input value.
					input = localHitPosition;

					// Configure the distance of the input position from center.
					distance = Vector3.Distance( Vector2.zero, localHitPosition );

					// If the distance value is within range of the radial menu...
					if( distance >= UltimateRadialMenuInformations[ i ].radialMenu.CalculatedMinRange && distance <= UltimateRadialMenuInformations[ i ].radialMenu.CalculatedMaxRange )
					{
						// Then assign the custom input data.
						UltimateRadialMenuInformations[ i ].raycastInputCalculated = true;
						UltimateRadialMenuInformations[ i ].raycastInput = input;
						UltimateRadialMenuInformations[ i ].raycastDistance = distance;

						// Set the input device as other.
						CurrentInputDevice = InputDevice.Other;
					}
				}
			}
		}
	}

	/// <summary>
	/// Triggers the input down for sending in the input to the radial menu.
	/// </summary>
	public void TriggerInputDown ()
	{
		customInputCalculated = true;
		customInputDown = true;
	}

	/// <summary>
	/// Triggers the input released for sending to the radial menus.
	/// </summary>
	public void TriggerInputUp ()
	{
		customInputCalculated = true;
		customInputUp = true;
	}
}