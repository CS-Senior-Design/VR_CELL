using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleHover : MonoBehaviour
{
    private bool isMoving;
    private Vector3 startingPosition;
    private Vector3 upperBound;
    private Vector3 lowerBound;
    public float animationSpeed = 1.0f;

    void Start()
    {  
        isMoving = false;
        startingPosition = transform.position;
        upperBound = new Vector3(startingPosition.x, startingPosition.y + 0.26f, startingPosition.z);
        lowerBound = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(hoverup());
        }
    }

    IEnumerator hoverup()
    {
        while (upperBound.y - gameObject.transform.position.y > 0.06f) 
        {

            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, upperBound, Time.deltaTime * animationSpeed);
            yield return null;
        }

        StartCoroutine(hoverdown());
    }

    IEnumerator hoverdown()
    {
        while (gameObject.transform.position.y - lowerBound.y > 0.06f) 
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, lowerBound, Time.deltaTime * animationSpeed);
            yield return null;
        }
        isMoving = false;
    }
}
