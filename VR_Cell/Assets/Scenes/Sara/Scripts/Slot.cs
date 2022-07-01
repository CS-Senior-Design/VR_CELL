// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.InputSystem;
// using UnityEngine.XR;
// using UnityEngine.XR.Interaction.Toolkit;

// public class Slot : MonoBehaviour
// {
//     public GameObject ItemInSlot;
//     public Image slotImage;
//     Color originalColor;

//     // Start is called before the first frame update
//     void Start()
//     {
//         slotImage = GetComponentInChildren<Image>();
//         originalColor = slotImage.color;
//     }

//     // Update is called once per frame
//     private void OnTriggerStay(Collider other)
//     {
//         bool triggerValue;

//         if (ItemInSlot != null)
//             return;

//         GameObject obj = other.gameObject;

//         if (!IsItem(obj))
//             return;
        
//         if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
//         {
//             InsertItem(obj);
//         }
//     }

//     bool IsItem(GameObject obj)
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
