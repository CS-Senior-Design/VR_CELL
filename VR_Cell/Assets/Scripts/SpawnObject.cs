using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject mitochondria;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mitochondriaSpawn = Instantiate(
                mitochondria,
                new Vector3(0,1,0),
                Quaternion.identity
            );
        }
    }
}
