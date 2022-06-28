// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;

// public class SocketEvents : MonoBehaviour
// {
//     // public GameObject organelle;
//     // private Color defaultOrganelleColor;

//     void Awake()
//     {
//         // defaultOrganelleColor = organelle.Color;
//         XRSocketInteractor socket = gameObject.GetComponent<XRSocketInteractor>();
//         socket.onSelectEntered.AddListener(SelectedObject);
//         // socket.onSelectExit.AddListener(ColorChange);
//     }

//     public void SelectedObject(XRBaseInteractable obj)
//     {
//         Debug.Log("object in socket 1");
//     }

    // public void ColorChange(XRBaseInteractable obj)
    // {
    //     organelle.gameObject.SetActive(true);
    //     ColorChange colorChange = obj.GameObject.GetComponent<ColorChange>();

    //     if (colorChange != null)
    //     {
    //         organelle.color = colorChange.color;
    //     }
    //     else
    //     {
    //         organelle.color = defaultOrganelleColor;
    //     }
    // }

    // public void SetDefaultColor(XRBaseInteractable obj)
    // {
    //     organelle.color = defaultOrganelleColor;
    // }

// }
