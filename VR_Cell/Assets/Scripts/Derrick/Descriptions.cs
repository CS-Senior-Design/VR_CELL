using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Descriptions {
    public string[] cellComponents;

    [TextArea(3, 10)]
    public string[] sentences;
}
