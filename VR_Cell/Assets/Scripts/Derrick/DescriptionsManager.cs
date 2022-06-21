using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionsManager : MonoBehaviour
{

    public TMP_Text cellComponentText;
    public TMP_Text sentenceText;
    public TMP_Text counter;
    private Stack<string> sentences;
    private Stack<string> sentencesBin;
    private Stack<string> cellComponents;
    private Stack<string> cellComponentsBin;
    private int descriptionCounter;
    private string sentence;
    private string component;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Stack<string>();
        sentencesBin = new Stack<string>();
        cellComponents = new Stack<string>();
        cellComponentsBin = new Stack<string>();
    }

    public void StartDescription(Descriptions description)
    {
        // Debug.Log("Starting description of component: " + description.cellComponent);

        sentences.Clear();
        sentencesBin.Clear();
        cellComponents.Clear();
        cellComponentsBin.Clear();
        descriptionCounter = 0;

        foreach (string s in description.sentences)
        {
            sentencesBin.Push(s);
        }
        foreach (string s in description.cellComponents)
        {
            cellComponentsBin.Push(s);
        }
        foreach (string s in description.sentences)
        {
            string sent = sentencesBin.Pop();
            sentences.Push(sent);
        }
        foreach (string s in description.cellComponents)
        {
            string sent = cellComponentsBin.Pop();
            cellComponents.Push(sent);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDescription();
            return;
        }

        sentencesBin.Push(sentence);
        cellComponentsBin.Push(component);

        sentence = sentences.Pop();
        component = cellComponents.Pop();

        descriptionCounter++;
        counter.text = descriptionCounter.ToString() + "/30";

        sentenceText.text = sentence;
        cellComponentText.text = component;
    }

    public void DisplayPreviousSentence()
    {
        if (sentencesBin.Count == 0 || descriptionCounter == 1)
        {
            EndDescription();
            return;
        }

        sentences.Push(sentence);
        cellComponents.Push(component);

        sentence = sentencesBin.Pop();
        component = cellComponentsBin.Pop();

        descriptionCounter--;
        counter.text = descriptionCounter.ToString() + "/30";

        sentenceText.text = sentence;
        cellComponentText.text = component;
    }

    void EndDescription()
    {
        Debug.Log("End of description.");
    }
}
