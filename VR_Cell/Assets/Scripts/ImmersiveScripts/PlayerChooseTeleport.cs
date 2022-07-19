using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerChooseTeleport : MonoBehaviour
{
    // get all the teleportation anchors and info panels
    // nucleolus
    public TeleportationAnchor nucleolusAnchor;
    public GameObject nucleolusInfoPanel;

    public TeleportationAnchor roughERAnchor;
    public GameObject roughERInfoPanel;

    // teleport to the nucleolus
    public void teleportToNucleolus()
    {
        // show the infopanel
        nucleolusInfoPanel.SetActive(true);
        // teleport to the nucleolus
        transform.position = nucleolusAnchor.teleportAnchorTransform.position;
        // orient the player to the nucleolus
        PlayerOrientation playerOrientation = GetComponent<PlayerOrientation>();
        playerOrientation.setOrientation(nucleolusInfoPanel);
    }

    // teleport to the rough ER
    public void teleportToRoughER()
    {
        // show the infopanel
        roughERInfoPanel.SetActive(true);
        // teleport to the rough ER
        transform.position = roughERAnchor.teleportAnchorTransform.position;
        // orient the player to the rough ER
        transform.LookAt(roughERInfoPanel.transform.position);
    }

}
