using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleolusUI : MonoBehaviour
{
    public GameObject _generateButton;
    public GameObject _stopButton;
    public GameObject _buttonDescription;

    private string _generateDescription = "Press 'Generate' to see it in action!";
    private string _stopDescription = "Press 'Stop' to stop making ribosomes.";

    private GenerateRibosomes _script;

    void Awake()
    {
        // make the stop button invisible
        _stopButton.SetActive(false);
        // make sure the description says the right thing at the start
        _buttonDescription.GetComponent<TMPro.TextMeshProUGUI>().text = _generateDescription;
        // get a reference to the generate ribosome script
        _script = gameObject.GetComponent<GenerateRibosomes>();
    }

    void Update()
    {
        // check if the animation ended before we press the button and change the buttons back
        if (_script.getAnimationPlay() == false && _stopButton.activeSelf == true)
        {
            toggleText();
        }
    }

    public void toggleText()
    {
        // if the generate button is visible, make the stop button visible and vice versa
        if (_generateButton.activeSelf)
        {
            _generateButton.SetActive(false);
            _stopButton.SetActive(true);
            _buttonDescription.GetComponent<TMPro.TextMeshProUGUI>().text = _stopDescription;
            _script.StartGenerate();
        }
        else
        {
            _generateButton.SetActive(true);
            _stopButton.SetActive(false);
            _buttonDescription.GetComponent<TMPro.TextMeshProUGUI>().text = _generateDescription;
            _script.StopGenerate();
        }
    }
}
