using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRibosomes : MonoBehaviour
{
    // where should the ribosome spawn
    private Vector3 _startPosition = new Vector3(0.0f, 0.0f, 0.0f);
    // take the ribosome prefab 
    public GameObject _ribosome;
    // animation speed 
    public float _animationSpeed = 0.05f;
    // gameobject list for the proteins
    public List<GameObject> _proteins = new List<GameObject>();
    // variable to track if the animation is complete
    public bool _animationComplete = false;
    // variable to toggle if the animation should continue
    public bool _animationPlay = false;

    // Start is called before the first frame update
    void Start()
    {
        // get the position of each of the protein transporters
        foreach (GameObject protein in GameObject.FindGameObjectsWithTag("transportProtein"))
        {
            _proteins.Add(protein);
        }
    }

    public void StartGenerate()
    {
        // start the coroutine
        _animationComplete = false;
        _animationPlay = true;
        // set the _generatingRibosomes in the UI controller script to true
        GameObject.FindGameObjectsWithTag("immersiveUIController")[0].GetComponent<DisplayUI>().setGeneratingRibosomes(true);
        StartCoroutine(Generate());
    }

    public void StopGenerate()
    {
        // stop the coroutine
        _animationPlay = false;
        GameObject.FindGameObjectsWithTag("immersiveUIController")[0].GetComponent<DisplayUI>().setGeneratingRibosomes(false);
        Debug.Log("animation should stop");
    }

    IEnumerator Generate()
    {
        // index to track the position on the _proteins array so we don't go out of bounds
        int index = 0;
        // play the animation indefinitely until the user tells us to stop
        while (_animationPlay == true)
        {
            if (index == _proteins.Count)
            {
                index = 0;
            }
            // spawn a ribosome
            GameObject ribosome = Instantiate(_ribosome, _startPosition, Quaternion.identity);
            // animate it towards the next protein
            StartCoroutine(RibosomeCreate(ribosome, _proteins[index].transform.position));
            yield return new WaitForSeconds(0.5f);
            index++;
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
