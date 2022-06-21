/* UltimateRadialMenuInputManagerEditor.cs */
/* Written by Kaz Crowe */
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

[CustomEditor( typeof( UltimateRadialMenuInputManager ) )]
public class UltimateRadialMenuInputManagerEditor : Editor
{
	UltimateRadialMenuInputManager targ;
	bool multipleInputManagerError = false;
	bool uniqueInputManager = false;

	// EDITOR STYLES //
	GUIStyle collapsableSectionStyle = new GUIStyle();

	// SCENE GUI //
	class DisplaySceneGizmo
	{
		public int frames = maxFrames;
		public bool hover = false;

		public bool HighlightGizmo
		{
			get
			{
				return hover || frames < maxFrames;
			}
		}

		public void PropertyUpdated ()
		{
			frames = 0;
		}
	}
	DisplaySceneGizmo DisplayInputTrackerSize = new DisplaySceneGizmo();
	DisplaySceneGizmo DisplayTouchInputActivationRange = new DisplaySceneGizmo();
	const int maxFrames = 200;
	// Gizmo Colors //
	Color colorDefault = Color.black;
	Color colorValueChanged = Color.cyan;

	// DEVELOPMENT MODE //
	bool showDefaultInspector = false;

	private void OnEnable ()
	{
		bool instanceInputManager = false;
		UltimateRadialMenuInputManager[] allInputManagers = FindObjectsOfType<UltimateRadialMenuInputManager>();
		for( int i = 0; i < allInputManagers.Length; i++ )
		{
			// If this input manager is on a radial menu, then continue to the next index.
			if( allInputManagers[ i ].GetComponent<UltimateRadialMenu>() )
				continue;

			// If we have already found an official input manager...
			if( instanceInputManager )
			{
				// Then set the error bool and break the loop.
				multipleInputManagerError = true;
				break;
			}

			// Since this input manager is not on a radial menu component, then this would be our official one.
			instanceInputManager = true;
		}

		targ = ( UltimateRadialMenuInputManager )target;

		uniqueInputManager = targ.GetComponent<UltimateRadialMenu>();

		if( uniqueInputManager )
		{
			serializedObject.FindProperty( "keyboardInput" ).boolValue = false;
			serializedObject.FindProperty( "touchInput" ).boolValue = false;
			serializedObject.FindProperty( "centerScreenInput" ).boolValue = false;
			serializedObject.ApplyModifiedProperties();
		}
	}

	bool DisplayCollapsibleBoxSection ( string sectionTitle, string editorPref, SerializedProperty enabledProp )
	{
		if( EditorPrefs.GetBool( editorPref ) && enabledProp.boolValue )
			collapsableSectionStyle.fontStyle = FontStyle.Bold;

		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();
		enabledProp.boolValue = EditorGUILayout.Toggle( enabledProp.boolValue, GUILayout.Width( 25 ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			if( enabledProp.boolValue )
				EditorPrefs.SetBool( editorPref, true );
			else
				EditorPrefs.SetBool( editorPref, false );
		}

		GUILayout.Space( -25 );

		EditorGUI.BeginDisabledGroup( !enabledProp.boolValue );
		if( GUILayout.Button( sectionTitle, collapsableSectionStyle ) )
			EditorPrefs.SetBool( editorPref, !EditorPrefs.GetBool( editorPref ) );
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.EndHorizontal();

		if( EditorPrefs.GetBool( editorPref ) )
			collapsableSectionStyle.fontStyle = FontStyle.Normal;

		return EditorPrefs.GetBool( editorPref ) && enabledProp.boolValue;
	}

	void CheckPropertyHover ( DisplaySceneGizmo displaySceneGizmo )
	{
		displaySceneGizmo.hover = false;
		var rect = GUILayoutUtility.GetLastRect();
		if( Event.current.type == EventType.Repaint && rect.Contains( Event.current.mousePosition ) )
			displaySceneGizmo.hover = true;
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		
		if( !targ.gameObject.GetComponent<EventSystem>() && !targ.gameObject.GetComponent<UltimateRadialMenu>() )
		{
			EditorGUILayout.BeginVertical( "Box" );
			GUIStyle warningStyle = new GUIStyle( GUI.skin.label );
			warningStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
			warningStyle.alignment = TextAnchor.MiddleCenter;
			EditorGUILayout.LabelField( "WARNING", warningStyle );

			GUIStyle labelStyle = new GUIStyle( GUI.skin.label ) { wordWrap = true };
			EditorGUILayout.LabelField( "The Ultimate Radial Menu Input Manager needs to be located on either the EventSystem in your scene, or on each individual Ultimate Radial Menu object if you want the input to be unique.", labelStyle );
			
			EditorGUILayout.EndVertical();
		}
		else if( multipleInputManagerError )
		{
			EditorGUILayout.BeginVertical( "Box" );
			GUIStyle warningStyle = new GUIStyle( GUI.skin.label );
			warningStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
			warningStyle.alignment = TextAnchor.MiddleCenter;
			EditorGUILayout.LabelField( "WARNING", warningStyle );

			GUIStyle labelStyle = new GUIStyle( GUI.skin.label ) { wordWrap = true };
			EditorGUILayout.LabelField( "There are multiple Ultimate Radial Menu Input Managers in the scene. This is likely because of a earlier version of the Ultimate Radial Menu. Click the button below to fix this.", labelStyle );

			if( GUILayout.Button( "Fix Input Manager" ) )
			{
				UltimateRadialMenuInputManager[] allInputManagers = FindObjectsOfType<UltimateRadialMenuInputManager>();
				for( int i = 0; i < allInputManagers.Length; i++ )
				{
					if( !allInputManagers[ i ].GetComponent<EventSystem>() && !allInputManagers[ i ].GetComponent<UltimateRadialMenu>() )
						DestroyImmediate( allInputManagers[ i ] );
				}

				if( !FindObjectOfType<EventSystem>().gameObject.GetComponent<UltimateRadialMenuInputManager>() )
					FindObjectOfType<EventSystem>().gameObject.AddComponent<UltimateRadialMenuInputManager>();

				multipleInputManagerError = FindObjectsOfType<UltimateRadialMenuInputManager>().Length > 1;
			}

			EditorGUILayout.EndVertical();
		}
		else
		{
			collapsableSectionStyle = new GUIStyle( EditorStyles.label ) { alignment = TextAnchor.MiddleCenter, onActive = new GUIStyleState() { textColor = Color.black } };
			collapsableSectionStyle.active.textColor = collapsableSectionStyle.normal.textColor;

			// INTERACT SETTINGS //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "invokeAction" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "onMenuRelease" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "disableOnInteract" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableMenuSetting" ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// END INTERACT SETTINGS //

			EditorGUILayout.Space();

			if( uniqueInputManager )
			{
				DisplayControllerSettings();

#if ENABLE_INPUT_SYSTEM
				// CUSTOM CONTROLLER SETTINGS //
				DisplayCustomControllerSettings();
				// END CUSTOM CONTROLLER SETTINGS //
#endif
			}
			else
			{
				// MOUSE AND KEYBOARD SETTINGS //
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Mouse & Keyboard Input", "URMIM_KeyboardInput", serializedObject.FindProperty( "keyboardInput" ) ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "mouseInteractButton" ) );
					if( targ.enableMenuSetting != UltimateRadialMenuInputManager.EnableMenuSetting.Manual )
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "keyboardEnableKey" ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
				}
				EditorGUILayout.EndVertical();
				// END MOUSE AND KEYBOARD SETTINGS //

				// CONTROLLER SETTINGS //
				DisplayControllerSettings();
				// END CONTROLLER SETTINGS //

				// TOUCH SETTINGS //
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Touch Input", "URMIM_TouchInput", serializedObject.FindProperty( "touchInput" ) ) )
				{
					if( targ.enableMenuSetting == UltimateRadialMenuInputManager.EnableMenuSetting.Manual )
						EditorPrefs.SetBool( "URMIM_TouchInput", false );
					else
					{
						EditorGUILayout.LabelField( "Enable Menu Settings", EditorStyles.boldLabel );
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "activationRadius" ) );
						if( EditorGUI.EndChangeCheck() )
						{
							DisplayTouchInputActivationRange.frames = 0;
							serializedObject.ApplyModifiedProperties();
						}
						CheckPropertyHover( DisplayTouchInputActivationRange );

						if( targ.activationRadius > 0.0f )
						{
							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField( serializedObject.FindProperty( "activationHoldTime" ) );
							EditorGUILayout.PropertyField( serializedObject.FindProperty( "dynamicPositioning" ) );
							if( EditorGUI.EndChangeCheck() )
								serializedObject.ApplyModifiedProperties();

							EditorGUILayout.HelpBox( "The Touch Input will not enable/disable world space radial menus. These must be enabled/disabled manually.", MessageType.Info );
						}
					}
				}
				EditorGUILayout.EndVertical();
				// END TOUCH SETTINGS //

				// CENTER SCREEN SETTINGS //
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Center Screen Input", "URMIM_CenterScreenInput", serializedObject.FindProperty( "centerScreenInput" ) ) )
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "interactOnHover" ) );
					if( serializedObject.FindProperty( "interactOnHover" ).boolValue )
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "interactHoverTime" ) );
						EditorGUI.indentLevel--;
					}
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "virtualReality" ) );
					if( serializedObject.FindProperty( "virtualReality" ).boolValue )
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "leftEyeCamera" ) );
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "rightEyeCamera" ) );
						EditorGUI.indentLevel--;

						if( serializedObject.FindProperty( "leftEyeCamera" ).objectReferenceValue == null || serializedObject.FindProperty( "rightEyeCamera" ).objectReferenceValue == null )
							EditorGUILayout.HelpBox( "Please make sure to assign both the Left and Right Eye cameras in order for the input manager to calculate the center of the screen.", MessageType.Warning );
					}
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "trackInputPosition" ) );
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
					if( serializedObject.FindProperty( "trackInputPosition" ).boolValue )
					{
						EditorGUI.indentLevel++;
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "trackInputSprite" ) );
						if( EditorGUI.EndChangeCheck() )
							serializedObject.ApplyModifiedProperties();

						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "inputTrackerSize" ) );
						if( EditorGUI.EndChangeCheck() )
						{
							DisplayInputTrackerSize.frames = 0;
							serializedObject.ApplyModifiedProperties();
						}
						CheckPropertyHover( DisplayInputTrackerSize );
						EditorGUI.indentLevel--;
					}

					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "forwardInputOnly" ) );
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableWhenInRange" ) );
					if( serializedObject.FindProperty( "enableWhenInRange" ).boolValue )
					{
						UltimateRadialMenu[] allRadialMenus = FindObjectsOfType<UltimateRadialMenu>();
						for( int i = 0; i < allRadialMenus.Length; i++ )
						{
							if( allRadialMenus[ i ].IsWorldSpaceRadialMenu && allRadialMenus[ i ].radialMenuToggle == UltimateRadialMenu.RadialMenuToggle.Scale )
							{
								EditorGUILayout.HelpBox( "Any menu using the Scale toggle type will not be enabled because the collider essentially disappears from raycast calculations.", MessageType.Error );
								break;
							}
						}
					}
					if( EditorGUI.EndChangeCheck() )
						serializedObject.ApplyModifiedProperties();
					
					//if( targ.enableWorldSpace )
					//	EditorGUILayout.HelpBox( "The Input Manager will not enable/disable the radial menus for Center Screen Input.", MessageType.Info );
				}
				EditorGUILayout.EndVertical();
				// END CENTER SCREEN SETTINGS //

				// CUSTOM SETTINGS //
				EditorGUILayout.BeginVertical( "Box" );
				if( DisplayCollapsibleBoxSection( "Custom Input", "URMIM_CustomInput", serializedObject.FindProperty( "customInput" ) ) )
				{
					GUIStyle warningStyle = new GUIStyle( GUI.skin.label );
					warningStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
					warningStyle.alignment = TextAnchor.MiddleCenter;
					EditorGUILayout.LabelField( "WARNING", warningStyle );

					GUIStyle labelStyle = new GUIStyle( GUI.skin.label ) { wordWrap = true };
					EditorGUILayout.LabelField( "Your custom input logic should be placed inside of a different script that inherits from the UltimateRadialMenuInputManager class.", labelStyle );

					GUIStyle style = new GUIStyle( GUI.skin.label ) { richText = true, wordWrap = true };
					EditorGUILayout.LabelField( "Please check out our <b><color=blue>Video Tutorials</color></b> to learn more!", style );
					var rect = GUILayoutUtility.GetLastRect();
					EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
					if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
						Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu" );
				}
				EditorGUILayout.EndVertical();
				// END CUSTOM SETTINGS //

#if ENABLE_INPUT_SYSTEM
				// CUSTOM CONTROLLER SETTINGS //
				DisplayCustomControllerSettings();
				// END CUSTOM CONTROLLER SETTINGS //
#endif
			}

			if( Application.isPlaying )
			{
				EditorGUILayout.BeginVertical( "Box" );
				collapsableSectionStyle.fontStyle = FontStyle.Bold;
				EditorGUILayout.LabelField( "Debug Information", collapsableSectionStyle );
				EditorGUILayout.LabelField( $"Current Input Device: { targ.CurrentInputDevice }" );
				EditorGUILayout.LabelField( $"Radial Menus Controlled: { targ.UltimateRadialMenuInformations.Count }" );
				EditorGUILayout.EndVertical();
			}

			if( EditorPrefs.GetBool( "UUI_DevelopmentMode" ) )
			{
				EditorGUILayout.Space();
				GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbarButton ) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 11, richText = true };
				GUILayout.BeginHorizontal();
				GUILayout.Space( -10 );
				showDefaultInspector = GUILayout.Toggle( showDefaultInspector, ( showDefaultInspector ? "▼" : "►" ) + "<color=#ff0000ff>Development Inspector</color>", toolbarStyle );
				GUILayout.EndHorizontal();
				if( showDefaultInspector )
				{
					EditorGUILayout.Space();

					base.OnInspectorGUI();

					if( targ.customInput )
					{
						EditorGUILayout.BeginVertical( "Box" );
						GUIStyle warningStyle = new GUIStyle( GUI.skin.label );
						warningStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
						warningStyle.alignment = TextAnchor.MiddleCenter;
						EditorGUILayout.LabelField( "WARNING", warningStyle );

						GUIStyle labelStyle = new GUIStyle( GUI.skin.label ) { wordWrap = true };
						EditorGUILayout.LabelField( "Your custom input logic should be placed inside of a different script that inherits from the UltimateRadialMenuInputManager class.", labelStyle );

						GUIStyle style = new GUIStyle( GUI.skin.label ) { richText = true, wordWrap = true };
						EditorGUILayout.LabelField( "Please check out our <b><color=blue>Video Tutorials</color></b> to learn more!", style );
						var rect = GUILayoutUtility.GetLastRect();
						EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
						if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
							Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TltHWPVuj-GLs9ZBd4tYMmu" );
						EditorGUILayout.EndVertical();
					}
				}
			}

			EditorGUILayout.Space();
		}

		Repaint();
	}

	private void DisplayControllerSettings ()
	{
		EditorGUILayout.BeginVertical( "Box" );
		if( DisplayCollapsibleBoxSection( "Controller Input", "URMIM_ControllerInput", serializedObject.FindProperty( "controllerInput" ) ) )
		{
			EditorGUI.BeginChangeCheck();
#if ENABLE_INPUT_SYSTEM
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "joystick" ) );
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "interactButtons" ) );
					if( targ.enableMenuSetting != UltimateRadialMenuInputManager.EnableMenuSetting.Manual )
						EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableButtons" ), new GUIContent( "Enable Buttons" ) );
#else
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "horizontalAxisController" ), new GUIContent( "Horizontal Axis" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "verticalAxisController" ), new GUIContent( "Vertical Axis" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "interactButtonController" ), new GUIContent( "Interact Button" ) );
			if( targ.enableMenuSetting != UltimateRadialMenuInputManager.EnableMenuSetting.Manual )
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "enableButtonController" ), new GUIContent( "Enable Button" ) );
#endif
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "invertHorizontal" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "invertVertical" ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndVertical();
	}

	private void DisplayCustomControllerSettings ()
	{
#if ENABLE_INPUT_SYSTEM
		// CUSTOM CONTROLLER SETTINGS //
		EditorGUILayout.BeginVertical( "Box" );
		if( DisplayCollapsibleBoxSection( "Custom Controller Input", "URMIM_CustomControllerInput", serializedObject.FindProperty( "customControllerInput" ) ) )
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "customControllerJoystick" ), new GUIContent( "Joystick" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "customControllerInteract" ), new GUIContent( "Interact Buttons" ) );
			if( targ.enableMenuSetting != UltimateRadialMenuInputManager.EnableMenuSetting.Manual )
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "customControllerEnable" ), new GUIContent( "Enable Buttons" ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndVertical();
		// END CUSTOM CONTROLLER SETTINGS //
#endif
	}

	private void OnSceneGUI ()
	{
		Handles.color = colorDefault;

		UltimateRadialMenu[] allRadialMenus = FindObjectsOfType<UltimateRadialMenu>();
		for( int i = 0; i < allRadialMenus.Length; i++ )
		{
			if( targ.centerScreenInput && EditorPrefs.GetBool( "URMIM_CenterScreenInput" ) && allRadialMenus[ i ].IsWorldSpaceRadialMenu )
			{
				RectTransform trans = allRadialMenus[ i ].transform.GetComponent<RectTransform>();
				Vector3 center = allRadialMenus[ i ].BasePosition;
				center.z = trans.position.z;

				if( serializedObject.FindProperty( "trackInputPosition" ).boolValue )
				{
					if( DisplayInputTrackerSize.HighlightGizmo )
						Handles.color = colorValueChanged;

					Handles.DrawWireCube( center, trans.TransformDirection( new Vector3( 1, 1, 0 ) ) * trans.sizeDelta.x * trans.GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale.x * serializedObject.FindProperty( "inputTrackerSize" ).floatValue );

					Handles.color = colorDefault;
				}

				if( serializedObject.FindProperty( "enableWhenInRange" ).boolValue && allRadialMenus[ i ].IsWorldSpaceRadialMenu && allRadialMenus[ i ].radialMenuToggle == UltimateRadialMenu.RadialMenuToggle.Scale )
				{
					Color defaultColor = Handles.color;
					Color errorColor = new Color( 1, 0, 0, 0.25f );
					Handles.color = errorColor;
					Handles.DrawSolidDisc( center, trans.transform.forward, trans.sizeDelta.x / 2 * trans.GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale.x );
					Handles.color = defaultColor;
				}
			}

			if( targ.touchInput && targ.enableMenuSetting != UltimateRadialMenuInputManager.EnableMenuSetting.Manual && EditorPrefs.GetBool( "URMIM_TouchInput" ) && !allRadialMenus[ i ].IsWorldSpaceRadialMenu )
			{
				if( DisplayTouchInputActivationRange.HighlightGizmo )
					Handles.color = colorValueChanged;

				RectTransform trans = allRadialMenus[ i ].transform.GetComponent<RectTransform>();
				Vector3 center = allRadialMenus[ i ].BasePosition;
				center.z = trans.position.z;
				Handles.DrawWireDisc( center, trans.transform.forward, trans.sizeDelta.x / 2 * trans.GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale.x * targ.activationRadius );
			}
		}

		DisplayInputTrackerSize.frames++;
		DisplayTouchInputActivationRange.frames++;

		SceneView.RepaintAll();
	}
}