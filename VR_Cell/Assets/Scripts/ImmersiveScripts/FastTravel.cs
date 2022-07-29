using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravel : MonoBehaviour
{
    private List<GameObject> _anchors = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        // get all the anchors and put them in the list
        foreach (GameObject anchor in GameObject.FindGameObjectsWithTag("organelleAnchor"))
        {
            _anchors.Add(anchor);
        }

        foreach (var anchor in _anchors)
        {
            Debug.Log(anchor.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> getAnchors()
    {
        return _anchors;
    }
}
