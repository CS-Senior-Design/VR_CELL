using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganelleHover : MonoBehaviour
{
    private bool _isMovingUp;
    private bool _isMovingDown;
    private float _yOffset = 0.08f;
    private float animationSpeed = 0.2f;
    private Vector3 _startingPosition;
    // Start is called before the first frame update
    void Start()
    {
        // call coroutine to hover the organelles until a button is pressed
        _isMovingDown = false;
        _isMovingUp = false;

        _startingPosition = gameObject.transform.position;
    }

    void Update()
    {
        if (_isMovingDown == false && _isMovingUp == false)
        {
            float randomNumber = Random.Range(0, 10);
            if (randomNumber > 5)
                StartCoroutine(HoverUp());
            else
                StartCoroutine(HoverDown());
        }
    }

    // hover coroutine
    IEnumerator HoverUp()
    {
        _isMovingUp = true;
        // get the position of the organelle
        Vector3 startPosition = _startingPosition;

        // get the upper bound
        Vector3 upperBound = new Vector3(startPosition.x, startPosition.y + _yOffset, startPosition.z);

        // set the animation spee
        while (upperBound.y - gameObject.transform.position.y > 0.05f)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, upperBound, Time.deltaTime * animationSpeed);
            yield return null;
        }
    
        _isMovingUp = false;

        StartCoroutine(HoverDown());

    }

    // hover coroutine
    IEnumerator HoverDown()
    {
        _isMovingDown = true;
        // get the position of the organelle
        Vector3 startPosition = _startingPosition;

        // get the lower bound
        Vector3 lowerBound = new Vector3(startPosition.x, startPosition.y - _yOffset, startPosition.z);

        while ( gameObject.transform.position.y - lowerBound.y > 0.05f)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, lowerBound, Time.deltaTime * animationSpeed);
            yield return null;
        }
        _isMovingDown = false;
    }

}
