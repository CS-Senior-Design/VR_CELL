// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.InputSystem;
// using UnityEngine.XR.Interaction.Toolkit;

// public class Slot : MonoBehaviour
// {
//     public GameObject ItemInSlot;
//     public Image slotImage;
//     Color originalColor;

//     //Creates an enum that will determine if we're using the right or left controller
//     public enum ControllerType
//     {
//         RightHand,
//         LeftHand
//     }

//     //Stores the target controller from the editor
//     public ControllerType targetController;

//     //References our Input Actions that we are using
//     public InputActionAsset inputAction;

//     private InputAction _triggerPressed;

//     // Start is called before the first frame update
//     void Start()
//     {
//         slotImage = GetComponentInChildren<Image>();
//         originalColor = slotImage.color;
//         _triggerPressed = inputAction.FindActionMap("XRI " + targetController.ToString()).FindAction("Activate");
//         _triggerPressed.Enable();
//         _triggerPressed.performed += OnTriggerStay;
//     }

//     // Update is called once per frame
//     private void OnTriggerStay(InputAction.CallbackContext context)
//     {
//         if (ItemInSlot != null)
//             return;

//         GameObject obj = context.gameObject;

//         if (!isItem(obj))
//             return;
        
//         if (_triggerPressed)
//         {
//             InsertItem(obj);
//         }
//     }

//     bool isItem(GameObject obj)
//     {
//         return obj.GetComponent<Item>();
//     }

//     void InsertItem(GameObject obj)
//     {
//         obj.GetComponent<Rigidbody>().isKinematic = true;
//         obj.transform.SetParent(gameObject.transform, true);
//         obj.transform.localPosition = Vector3.zero;
//         obj.transform.localEulerAngles = obj.GetComponent<Item>.slotRotation;
//         obj.GetComponent<Item>().inSlot = true;
//         obj.GetComponent<Item>().currentSlot = this;
//         ItemInSlot = obj;
//         slotImage.color = Color.gray;
//     }

//     public void ResetColor()
//     {
//         slotImage.color = originalColor;
//     }
// }
