using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public TMP_Text questionNumber;
    public TMP_Text questionPrompt;
    public TMP_Text promptA, promptB, promptC, promptD;

    private Stack<Question> questionStack, questionStackBin;

    private int questionCounter;
    private int numOfQuestions;
    private string prompt;
    private string A, B, C, D;

    void Start()
    {
        questionStack = new Stack<Question>();
        questionStackBin = new Stack<Question>();
    }

    public void StartQuiz(Question[] questions)
    {
        questionStack.Clear();
        questionStackBin.Clear();
        questionCounter = 0;
        numOfQuestions = 12;

        foreach (Question q in questions)
        {
            questionStackBin.Push(q);
        }

        foreach (Question q in questions)
        {
            Question tempQuestion = questionStackBin.Pop();
            questionStack.Push(tempQuestion);
        }

        DisplayNextQuestion();
    }

    public void DisplayNextQuestion()
    {
        if (questionStack.Count == 0)
        {
            EndQuiz();
            return;
        }

        // Insert logic here.

        questionCounter++;
        questionNumber.text = "Question " + questionCounter.ToString() + "/" + numOfQuestions;


    }

    public void DisplayPreviousQuestion()
    {
        if (questionStackBin.Count == 0 || questionCounter == 1)
        {
            EndQuiz();
            return;
        }

        // Insert logic here.

        questionCounter--;
        questionNumber.text = "Question " + questionCounter.ToString() + "/" + numOfQuestions;

        
    }

    void EndQuiz()
    {
        Debug.Log("End of Quiz.");
    }
}
