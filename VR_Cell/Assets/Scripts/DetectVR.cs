using UnityEngine;
using UnityEngine.XR.Management;

public class DetectVR : MonoBehaviour
{
    public bool startInVR = true;
    public GameObject xrOrigin;
    public GameObject desktopPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        if(startInVR) {

            var xrSettings = XRGeneralSettings.Instance;
            if(xrSettings == null) {
                Debug.Log("XRGeneralSettings is null");
                return;
            }

            var xrManager = xrSettings.Manager;
            if(xrSettings == null) {
                Debug.Log("XRManagerSettings is null");
                return;
            }

            var xrLoader = xrManager.activeLoader;
            if(xrSettings == null) {
                Debug.Log("XRLoader is null");
                xrOrigin.SetActive(false);
                desktopPlayer.SetActive(true);
                return;
            }

            Debug.Log("XRLoader is NOT null");
            xrOrigin.SetActive(true);
            desktopPlayer.SetActive(false);
            return;

        } else {
            xrOrigin.SetActive(false);
            desktopPlayer.SetActive(true);
        }

    }
}
