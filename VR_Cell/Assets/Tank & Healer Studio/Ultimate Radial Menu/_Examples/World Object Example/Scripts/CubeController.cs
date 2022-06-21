using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CubeController : MonoBehaviour
{
	[System.Serializable]
	public class CubeColor
	{
		public Color color = Color.white;
		public UltimateRadialButtonInfo buttonInfo;
	}
	public CubeColor[] cubeColors;

	Transform onMouseDownTransform;
	Renderer selectedRenderer;


	private void Start ()
	{
		for( int i = 0; i < cubeColors.Length; i++ )
		{
			cubeColors[ i ].buttonInfo.id = i;
			UltimateRadialMenu.RegisterToRadialMenu( "ObjectExample", UpdateCubeColor, cubeColors[ i ].buttonInfo );
		}
	}

	private void Update ()
	{
		Vector2 mousePosition = Vector2.zero;
		bool mouseButtonDown = false;
		bool mouseButtonUp = false;
#if ENABLE_INPUT_SYSTEM
		// Store the mouse from the input system.
		Mouse mouse = InputSystem.GetDevice<Mouse>();

		// If the mouse couldn't be stored from the input system, then return.
		if( mouse == null )
			return;

		// Store the mouse data.
		mousePosition = mouse.position.ReadValue();
		mouseButtonDown = mouse.leftButton.wasPressedThisFrame;
		mouseButtonUp = mouse.leftButton.wasReleasedThisFrame;
#else
		// Store the mouse data.
		mousePosition = Input.mousePosition;
		mouseButtonDown = Input.GetMouseButtonDown( 0 );
		mouseButtonUp = Input.GetMouseButtonUp( 0 );
#endif

		// If the left mouse button is down on the frame...
		if( mouseButtonDown )
		{
			// Cast a ray so that we can check if the mouse position is over an object.
			Ray ray = Camera.main.ScreenPointToRay( mousePosition );

			RaycastHit hit;

			// If the raycast hit something, then store the hit transform.
			if( Physics.Raycast( ray, out hit ) )
				onMouseDownTransform = hit.transform;
			// Else, if the radial menu is active and the mouse is not over a button of the radial menu, then disable the menu.
			else if( UltimateRadialMenu.GetUltimateRadialMenu( "ObjectExample" ).RadialMenuActive && UltimateRadialMenu.GetUltimateRadialMenu( "ObjectExample" ).CurrentButtonIndex < 0 )
				UltimateRadialMenu.GetUltimateRadialMenu( "ObjectExample" ).DisableRadialMenu();
		}

		// If the left mouse button came up on this frame...
		if( mouseButtonUp )
		{
			// Cast a ray so that we can check if the mouse position is over an object.
			Ray ray = Camera.main.ScreenPointToRay( mousePosition );

			RaycastHit hit;

			// If the raycast hit something...
			if( Physics.Raycast( ray, out hit ) )
			{
				// If the hit transform is the same as the transform when the mouse button was pressed...
				if( hit.transform == onMouseDownTransform )
				{
					// Store the selected renderer as the hit transform.
					selectedRenderer = hit.transform.GetComponent<Renderer>();

					// Configure the screen position of the hit transform.
					Vector3 screenPosition = Camera.main.WorldToScreenPoint( hit.transform.position );

					// Call SetPosition() on the radial menu to move it to the transform's position.
					UltimateRadialMenu.GetUltimateRadialMenu( "ObjectExample" ).SetPosition( screenPosition );

					// If the radial menu is currently disabled, then enable the menu.
					if( !UltimateRadialMenu.GetUltimateRadialMenu( "ObjectExample" ).RadialMenuActive )
						UltimateRadialMenu.GetUltimateRadialMenu( "ObjectExample" ).EnableRadialMenu();
				}
			}
		}
	}

	public void UpdateCubeColor ( int id )
	{
		// If the selected renderer is null, then just return.
		if( selectedRenderer == null )
			return;

		selectedRenderer.material.color = cubeColors[ id ].color;
	}
}