using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionsManager : MonoBehaviour {

    public TMP_Text cellComponentText;
    public TMP_Text sentenceText;
    private Queue<string> sentences;
    private Queue<string> sentencesBin;
    private Queue<string> cellComponents;
    private Queue<string> cellComponentsBin;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        sentencesBin = new Queue<string>();
        cellComponents = new Queue<string>();
        cellComponentsBin = new Queue<string>();
    }

    public void StartDescription(Descriptions description) {
        // Debug.Log("Starting description of component: " + description.cellComponent);

        sentences.Clear();
        sentencesBin.Clear();
        cellComponents.Clear();
        cellComponentsBin.Clear();

        foreach(string s in description.sentences) {
            sentences.Enqueue(s);
        }
        foreach(string s in description.cellComponents) {
            cellComponents.Enqueue(s);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (sentences.Count == 0) {
            EndDescription();
            return;
        }

        string sentence = sentences.Dequeue();
        string component = cellComponents.Dequeue();
        // Debug.Log(sentence);
        sentenceText.text = sentence;
        sentencesBin.Enqueue(sentence);
        cellComponentText.text = component;
        cellComponentsBin.Enqueue(component);
    }

    public void DisplayPreviousSentence() {
        if (sentencesBin.Count == 0) {
            EndDescription();
            return;
        }

        string sentence = sentencesBin.Dequeue();
        string component = cellComponentsBin.Dequeue();
        // Debug.Log(sentence);
        sentenceText.text = sentence;
        sentences.Enqueue(sentence);
        cellComponentText.text = component;
        cellComponents.Enqueue(component);
    }

    void EndDescription() {
        Debug.Log("End of description.");
    }
}
