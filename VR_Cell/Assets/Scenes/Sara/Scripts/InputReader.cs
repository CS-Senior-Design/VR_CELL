// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// // will allow us to get input device
// using UnityEngine.XR;

// public class InputReader : MonoBehaviour
// {
//     // creates a list of input devices to store our inputs in
//     List<InputDevice> inputDevices = new List<InputDevice>();

//     // Start is called before the first frame update
//     void Start()
//     {
//         // initializes the input reader here, but all components may not be loaded
//         InitializeInputReader();
        
//     }

//     void InitializeInputReader()
//     {
//         // InputDevices.GetDevices(inputDevices);
//         InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, inputDevices);

//         foreach (var inputDevice in inputDevices)
//         {
//             inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
//             Debug.Log(inputDevice.name + " " + triggerValue);
            
//             // Debug.Log(inputDevice.name + " " + inputDevice.characteristics);
//         }
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // should have a total of 3 input devices. if it's less, then we try to initialize them again.
//         if (inputDevices.Count < 2)
//         {
//             InitializeInputReader();
//         }
        
//     }
// }
