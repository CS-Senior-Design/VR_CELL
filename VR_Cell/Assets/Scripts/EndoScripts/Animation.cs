using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    
    // the object that goes into the golgi
    public GameObject _enterModel;
    // the object that comes out of the golgi
    public GameObject _exitModel;
    // variable to store the entermodel once we spawn it
    private GameObject _enter = null;
    // variable to store the exitmodel once we spawn it
    private GameObject _exit = null;
    // animation in progress
    private bool _enterInProgress = false;
    private bool _exitInProgress = false;
    // animation speed
    private float _animationSpeed = 1.0f;
    // amount of time the vesicle floats at the end
    private float _floatTime = 2.0f;
    // start positions
    private Vector3 _startPositionEnter;// = new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z);
    private Vector3 _startPositionExit;// = new Vector3(transform.position.x - 0.1f,transform.position.y, transform.position.z);
    // target positions
    private Vector3 _targetPositionEnter; // = new Vector3(transform.position.x - 0.1f,transform.position.y, transform.position.z);
    private Vector3 _targetPositionExit; // = new Vector3(transform.position.x - 0.25f,transform.position.y, transform.position.z);
    // used for the float coroutine
    private Vector3 _targetPositionFloat; // = new Vector3(transform.position.x - 0.25f,transform.position.y + 0.5f, transform.position.z);

    void Awake()
    {
        // spawn the enter and exit objects 
        _enter = Instantiate(
            _enterModel,
            _startPositionEnter,
            Quaternion.identity
        );
        _exit = Instantiate(
            _exitModel,
            _startPositionExit,
            Quaternion.identity
        );
        // make them inactive while not being used
        _enter.SetActive(false);
        _exit.SetActive(false);
        
        // start positions
        _startPositionEnter = new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z);
        _startPositionExit = new Vector3(transform.position.x - 0.1f,transform.position.y, transform.position.z);
        // target positions
        _targetPositionEnter = new Vector3(transform.position.x - 0.1f,transform.position.y, transform.position.z);
        _targetPositionExit = new Vector3(transform.position.x - 0.25f,transform.position.y, transform.position.z);
        // used for the float coroutine
        _targetPositionFloat = new Vector3(transform.position.x - 0.25f,transform.position.y + 0.5f, transform.position.z);
    }

    public GameObject getEnter()
    {
        return _enter;
    }

    public GameObject getExit()
    {
        return _exit;
    }

    public void StartAnimation()
    {
        // only trigger the animation if neither of the animations are in progress
        if (_enterInProgress == false && _exitInProgress == false)
        {
            Debug.Log("entered");
            // make the _enter object active
            _enter.SetActive(true);
            // make the _exit object inactive while the enter object is being animated
            _exit.SetActive(false);
            // place _enter object at its start position
            _enter.transform.position = _startPositionEnter;
            // place _exit object at its start position
            _exit.transform.position = _startPositionExit;
            // slowly move the _enter object towards its target position
            StartCoroutine(Animate1());
        }
    }

    // coroutine animation of the _enter object moving into the golgi
    IEnumerator Animate1()
    {
        // while the _enter object has not reached withit 0.008 units of the destination
        while ((_enter.transform.position - _targetPositionEnter).sqrMagnitude > 0.008f)
        {
            // toggle to show animation in progress
            _enterInProgress = true;
            // slowly move it towards the target position
            _enter.transform.position = Vector3.Lerp(_enter.transform.position, _targetPositionEnter, Time.deltaTime * _animationSpeed);
            // wait for the next frame
            yield return null;
        }
        // once the _enter object has reached its destination, make it inactive
        _enter.SetActive(false);
        // toggle the animation progress to false, since it's complete
        _enterInProgress = false;
        // toggle the exit animation to true since it's about to start
        _exitInProgress = true;
        // make the _exit object visible
        _exit.SetActive(true);
        // slowly move the _exit object towards its target position
        StartCoroutine(Animate2());
    }

    // coroutine animation of the _exit object moving out of the golgi
    IEnumerator Animate2()
    {
        // while the _exit object has not reached within 0.001 units of its destination
        while ((_exit.transform.position - _targetPositionExit).sqrMagnitude > 0.001f)
        {
            // show that the animation is in progress
            _exitInProgress = true;
            // slowly move it towards the target position
            _exit.transform.position = Vector3.Lerp(_exit.transform.position, _targetPositionExit, Time.deltaTime * _animationSpeed);
            // wait for the next frame
            yield return null;
        }
        
        // option 1
        // start a coroutine to make it slowly move upwards as if floating
        //StartCoroutine(Float());
        
        // option 2
        // let it float away with actual physics
        // get the rigidbody of the _exit object (it has to have one)
        var rigidBody = _exit.GetComponent<Rigidbody>();
        // turn on the rigidbody
        rigidBody.isKinematic = false;
        // add a force to the rigidbody to make it float away
        rigidBody.AddForce(new Vector3(-0.03f,0.2f,0), ForceMode.Impulse);
        // wait _floatTime seconds before ending the float animation
        Invoke("ResetVesicle", _floatTime);
    }

    // function to reset the vesicle to its start position
    void ResetVesicle()
    {
        // get the rigid body
        var rigidBody = _exit.GetComponent<Rigidbody>();
        // deactivate the rigid body
        rigidBody.isKinematic = true;
        // hide the _exit object
        _exit.SetActive(false);
        // show that the animation is complete
        _exitInProgress = false;
    }

    // coroutine to make the vesicle slwoly float upwards towards _targetPositionFloat
    IEnumerator Float()
    {
        // let the object float up to 3.0f in the y axis
        while (_exit.transform.position.y != _targetPositionFloat.y)
        {
            _exitInProgress = true;
            _exit.transform.position = Vector3.Lerp(_exit.transform.position, _targetPositionFloat, Time.deltaTime * _animationSpeed);
            yield return null;
        }
        _exitInProgress = false;
    }
}
