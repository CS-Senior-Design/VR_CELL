using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationDirection;
    public float durationTime;
    private float smooth;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update () {
        smooth = Time.deltaTime * durationTime;
        transform.Rotate(rotationDirection * smooth);
    }
}
