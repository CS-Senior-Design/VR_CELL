using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizTrigger : MonoBehaviour
{
    public Question[] questions;

    public void TriggerQuiz()
    {
        FindObjectOfType<QuizManager>().StartQuiz(questions);
    }
}
