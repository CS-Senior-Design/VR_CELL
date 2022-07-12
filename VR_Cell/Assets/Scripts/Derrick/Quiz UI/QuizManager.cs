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
    private Question currentQuestion;
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
            Debug.Log(tempQuestion.questionPrompt);
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
        questionStackBin.Push(currentQuestion);
        currentQuestion = questionStack.Pop();

        questionCounter++;
        questionNumber.text = "Question " + questionCounter.ToString() + "/" + numOfQuestions;

        questionPrompt.text = currentQuestion.questionPrompt;
        promptA.text = currentQuestion.questionOptions[0];
        promptB.text = currentQuestion.questionOptions[1];
        promptC.text = currentQuestion.questionOptions[2];
        promptD.text = currentQuestion.questionOptions[3];
    }

    public void DisplayPreviousQuestion()
    {
        if (questionStackBin.Count == 0 || questionCounter == 1)
        {
            EndQuiz();
            return;
        }

        // Insert logic here.
        questionStack.Push(currentQuestion);
        currentQuestion = questionStackBin.Pop();

        questionCounter--;
        questionNumber.text = "Question " + questionCounter.ToString() + "/" + numOfQuestions;

        questionPrompt.text = currentQuestion.questionPrompt;
        promptA.text = currentQuestion.questionOptions[0];
        promptB.text = currentQuestion.questionOptions[1];
        promptC.text = currentQuestion.questionOptions[2];
        promptD.text = currentQuestion.questionOptions[3];
    }

    void EndQuiz()
    {
        Debug.Log("End of Quiz.");
    }
}
