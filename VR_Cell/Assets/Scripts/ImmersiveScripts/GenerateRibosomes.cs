using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRibosomes : MonoBehaviour
{
    // where should the ribosome spawn
    private Vector3 _startPosition = new Vector3(0.0f, 0.0f, 0.0f);
    // take the ribosome prefabs 
    public GameObject _ribosome1;
    public GameObject _ribosome2;
    // animation speed 
    private float _animationSpeed = 0.07f;
    // gameobject list for the proteins
    public List<GameObject> _proteins = new List<GameObject>();
    // variable to track if the animation is complete
    private bool _animationComplete = false;
    // variable to toggle if the animation should continue
    private bool _animationPlay = false;
    // variable to track how many ribosomes have been created
    private int _ribosomeCount = 0;
    // variable to change the max number of ribosomes
    private int _maxRibosomes = 100;
    // variable to track which ribosome to spawn
    private bool _ribosomeType = true;

    // Start is called before the first frame update
    void Start()
    {
        // get the position of each of the protein transporters
        foreach (GameObject protein in GameObject.FindGameObjectsWithTag("transportProtein"))
        {
            _proteins.Add(protein);
        }
    }
    public bool getAnimationPlay()
    {
        return _animationPlay;
    }

    public void StartGenerate()
    {
        // start the coroutine
        _animationComplete = false;
        _animationPlay = true;
        StartCoroutine(Generate());
    }

    public void StopGenerate()
    {
        // stop the coroutine
        _animationPlay = false;
    }

    IEnumerator Generate()
    {
        // index to track the position on the _proteins array so we don't go out of bounds
        int index = _proteins.Count - 1;
        // play the animation indefinitely until the user tells us to stop or until we reach 100 ribosomes
        while (_animationPlay == true)
        {
            if (index == -1)
            {
                index = _proteins.Count - 1;
            }
            GameObject ribosome = null;
            // spawn a ribosome
            if (_ribosomeType == true)    
            {
                _ribosomeType = false;
                ribosome = Instantiate(_ribosome1, _startPosition, Quaternion.identity);
            }
            else
            {
                _ribosomeType = true;
                ribosome = Instantiate(_ribosome2, _startPosition, Quaternion.identity);
            }     
            // make it face towards its target protein
            ribosome.transform.LookAt(_proteins[index].transform.position, Vector3.right);
            _ribosomeCount++;
            if (_ribosomeCount == _maxRibosomes)
            {
                _animationPlay = false;
            }
            // animate it towards the next protein
            StartCoroutine(RibosomeCreate(ribosome, _proteins[index].transform.position));
            yield return new WaitForSeconds(0.5f);
            index--;
        }
        _animationComplete = true;
    }

    // coroutine to create ribosomes and make them fly to the protein
    IEnumerator RibosomeCreate(GameObject ribosome, Vector3 destination)
    {
        while ((ribosome.transform.position - destination).sqrMagnitude > 0.01f && ribosome.GetComponent<RibosomeState>().isGrabbed() == false)
        {
            // slowly move it towards the target position
            ribosome.transform.position = Vector3.Lerp(ribosome.transform.position, destination, Time.deltaTime * _animationSpeed);
            // wait for the next frame
            yield return null;
        }
        // once the ribosome reaches its destination, destroy it and remove it from the list
        if (ribosome.GetComponent<RibosomeState>().isGrabbed() == false)
        {
            Destroy(ribosome);
        }
    }
}
