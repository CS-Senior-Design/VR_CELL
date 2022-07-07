using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSteps : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            transform.position = new Vector3(0,0,0);
        }
    }
}
