using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteboardDescription : MonoBehaviour
{
    public GameObject _textArea;
    public GameObject _textTitle;

    private Dictionary<string, string> _description = new Dictionary<string, string>();


    // Start is called before the first frame update
    void Start()
    {
        // create the dictionary
        _description.Add("Nucleus", " The most prominent organelle of the cell, the nucleus contains all the genetic information for the organism in the DNA packed inside its nuclear envelope. It comprises of two main parts, the inner and outer membrane.");    
        _description.Add("Rough ER", "The rough ER is a major component of the endomembrane system. It's involved in folding and quality control of newly formed proteins, which are then sent out to be packaged by the Golgi Apparatus.");
        _description.Add("Smooth ER", "Smooth ER is a component of the endomembrane system. It's involved in the synthesis of lipids and steroids, which are then sent out to be packaged by the Golgi Apparatus.");
        _description.Add("Mitochondria", "Mitochondria generate most of the chemical energy needed to power the cell's biochemical reactions.");
        _description.Add("Golgi Apparatus", "The Golgi Apparatus acts as a factory in which proteins received from the rough ER are further processed and sorted for transport to their eventual destinations: lysosomes, the plasma membrane, or secretion.");
        _description.Add("Centrosomes", "A centrosome is a cellular structure involved in the process of cell division.");
    }

    public void changeText(string name)
    {
        _textArea.GetComponent<TMPro.TextMeshProUGUI>().text = _description[name];
        _textTitle.GetComponent<TMPro.TextMeshProUGUI>().text = name;
    }
}
