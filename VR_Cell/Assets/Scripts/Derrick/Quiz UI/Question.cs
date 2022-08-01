using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    [TextArea(3, 10)]
    public string questionPrompt;

    public string[] questionOptions;

    public string questionAnswer;
}
