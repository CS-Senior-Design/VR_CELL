using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionsTrigger : MonoBehaviour {
    public Descriptions description;

    public void TriggerDescription() {
        FindObjectOfType<DescriptionsManager>().StartDescription(description);
    }
}
