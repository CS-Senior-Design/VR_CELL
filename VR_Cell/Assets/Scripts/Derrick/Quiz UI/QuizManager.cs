using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public TMP_Text questionNumber;

    public TMP_Text questionPrompt;

    public TMP_Text

            promptA,
            promptB,
            promptC,
            promptD;

    public GameObject

            buttonA,
            buttonB,
            buttonC,
            buttonD;

    private Stack<Question>

            questionStack,
            questionStackBin;

    private int questionCounter;

    private int numOfQuestions;

    private Question currentQuestion;

    void Start()
    {
        questionStack = new Stack<Question>();
        questionStackBin = new Stack<Question>();
    }

    public void checkAnswer()
    {
    }

    public void turnButtonRed(GameObject _button)
    {
    }

    public void turnButtonGreen(GameObject _button)
    {
    }

    public void turnButtonWhite(GameObject _button)
    {
    }

    public void StartQuiz(Question[] questions)
    {
        questionStack.Clear();
        questionStackBin.Clear();
        questionCounter = 0;
        numOfQuestions = 12;

        foreach (Question q in questions)
        {
            questionStackBin.Push (q);
        }

        foreach (Question q in questions)
        {
            Question tempQuestion = questionStackBin.Pop();
            questionStack.Push (tempQuestion);
            Debug.Log(tempQuestion.questionPrompt);
        }

        DisplayNextQuestion();
    }

    public void DisplayNextQuestion()
    {
        // On each new question, reset all button colors back to gray
        // turnButtonWhite(buttonA);
        // turnButtonWhite(buttonB);
        // turnButtonWhite(buttonC);
        // turnButtonWhite(buttonD);
        if (questionStack.Count == 0)
        {
            EndQuiz();
            return;
        }

        questionStackBin.Push (currentQuestion);
        currentQuestion = questionStack.Pop();

        // If question is true/false, de-activate two answer choices
        if (currentQuestion.questionOptions.Length == 2)
        {
            buttonA.SetActive(false);
            buttonC.SetActive(false);

            questionCounter++;
            questionNumber.text =
                "Question " + questionCounter.ToString() + "/" + numOfQuestions;

            questionPrompt.text = currentQuestion.questionPrompt;
            promptB.text = currentQuestion.questionOptions[0];
            promptD.text = currentQuestion.questionOptions[1];
        }
        else
        // Otherwise question is standard 4-multiple-choice; activate the answer choices
        {
            buttonA.SetActive(true);
            buttonC.SetActive(true);
            questionCounter++;
            questionNumber.text =
                "Question " + questionCounter.ToString() + "/" + numOfQuestions;

            questionPrompt.text = currentQuestion.questionPrompt;
            promptA.text = currentQuestion.questionOptions[0];
            promptB.text = currentQuestion.questionOptions[1];
            promptC.text = currentQuestion.questionOptions[2];
            promptD.text = currentQuestion.questionOptions[3];
        }
    }

    public void DisplayPreviousQuestion()
    {
        if (questionStackBin.Count == 0 || questionCounter == 1)
        {
            EndQuiz();
            return;
        }

        questionStack.Push (currentQuestion);
        currentQuestion = questionStackBin.Pop();

        // If question is true/false, de-activate two answer choices
        if (currentQuestion.questionOptions.Length == 2)
        {
            buttonA.SetActive(false);
            buttonC.SetActive(false);
            questionCounter--;
            questionNumber.text =
                "Question " + questionCounter.ToString() + "/" + numOfQuestions;

            questionPrompt.text = currentQuestion.questionPrompt;
            promptB.text = currentQuestion.questionOptions[0];
            promptD.text = currentQuestion.questionOptions[1];
        }
        else
        // Otherwise question is standard 4-multiple-choice; activate the answer choices
        {
            buttonA.SetActive(true);
            buttonC.SetActive(true);
            questionCounter--;
            questionNumber.text =
                "Question " + questionCounter.ToString() + "/" + numOfQuestions;

            questionPrompt.text = currentQuestion.questionPrompt;
            promptA.text = currentQuestion.questionOptions[0];
            promptB.text = currentQuestion.questionOptions[1];
            promptC.text = currentQuestion.questionOptions[2];
            promptD.text = currentQuestion.questionOptions[3];
        }
    }

    void EndQuiz()
    {
        Debug.Log("End of Quiz.");
    }
}
