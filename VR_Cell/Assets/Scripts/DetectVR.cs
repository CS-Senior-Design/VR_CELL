using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class DetectVR : MonoBehaviour
{
    public GameObject xrOrigin;
    public GameObject desktopPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        var xrSettings = XRGeneralSettings.Instance;
        if(xrSettings == null) {
            Debug.Log("XRGeneralSettings is null");
        }
    }
}
