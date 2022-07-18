using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public TMP_Text questionNumber;

    public TMP_Text questionPrompt;

    public GameObject scorePanel;

    public GameObject uiPanel;

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

    private int score;

    private Question currentQuestion;

    void Start()
    {
        questionStack = new Stack<Question>();
        questionStackBin = new Stack<Question>();
        score = 0;
    }

    public void checkAnswer(GameObject selectedButton)
    {
        // If selected button is correct, add to score and mark the button green.
        if (
            selectedButton
                .gameObject
                .transform
                .GetChild(0)
                .GetComponent<TMPro.TextMeshProUGUI>()
                .text ==
            currentQuestion.questionAnswer
        )
        {
            score++;
            turnButtonGreen (selectedButton);
        }
        else
        // Otherwise, find the correct button, mark that green, then mark selected button red.
        {
            if (
                buttonA
                    .gameObject
                    .transform
                    .GetChild(0)
                    .GetComponent<TMPro.TextMeshProUGUI>()
                    .text ==
                currentQuestion.questionAnswer
            )
            {
                turnButtonGreen (buttonA);
            }
            else if (
                buttonB
                    .gameObject
                    .transform
                    .GetChild(0)
                    .GetComponent<TMPro.TextMeshProUGUI>()
                    .text ==
                currentQuestion.questionAnswer
            )
            {
                turnButtonGreen (buttonB);
            }
            else if (
                buttonC
                    .gameObject
                    .transform
                    .GetChild(0)
                    .GetComponent<TMPro.TextMeshProUGUI>()
                    .text ==
                currentQuestion.questionAnswer
            )
            {
                turnButtonGreen (buttonC);
            }
            else if (
                buttonD
                    .gameObject
                    .transform
                    .GetChild(0)
                    .GetComponent<TMPro.TextMeshProUGUI>()
                    .text ==
                currentQuestion.questionAnswer
            )
            {
                turnButtonGreen (buttonD);
            }

            turnButtonRed (selectedButton);
        }
    }

    public void turnButtonRed(GameObject _button)
    {
        _button.GetComponent<Image>().color = Color.red;
    }

    public void turnButtonGreen(GameObject _button)
    {
        _button.GetComponent<Image>().color = Color.green;
    }

    public void turnButtonWhite(GameObject _button)
    {
        _button.GetComponent<Image>().color = Color.white;
    }

    public void StartQuiz(Question[] questions)
    {
        questionStack.Clear();
        questionStackBin.Clear();
        questionCounter = 0;
        score = 0;
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
        turnButtonWhite (buttonA);
        turnButtonWhite (buttonB);
        turnButtonWhite (buttonC);
        turnButtonWhite (buttonD);

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
        // On each new question, reset all button colors back to gray
        turnButtonWhite (buttonA);
        turnButtonWhite (buttonB);
        turnButtonWhite (buttonC);
        turnButtonWhite (buttonD);

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

    // Calculate final score, show results panel.
    void EndQuiz()
    {
        Debug.Log("End of Quiz.");

        private float scorePercentage = (float)((score / numOfQuestions) * 100);
        Debug.Log("Quiz score: " + scorePercentage + "%");

        // Deactivate the uiPanel, activate the scorePanel, and push score info to it.
        uiPanel.SetActive(false);
        scorePanel.SetActive(true);

        GameObject uiCanvas = scorePanel.transform.GetChild(0).gameObject;
        TMP_Text postQuizResponse =
            uiCanvas
                .transform
                .Find("Post-Quiz Response")
                .gameObject
                .GetComponent<TMP_Text>();
        TMP_Text numQuestionsCorrect =
            uiCanvas
                .transform
                .Find("Num Questions Correct")
                .gameObject
                .GetComponent<TMP_Text>();
        TMP_Text overallScore =
            uiCanvas
                .transform
                .Find("Overall Score")
                .gameObject
                .GetComponent<TMP_Text>();
        TMP_Text timeSpent =
            uiCanvas
                .transform
                .Find("Time Spent")
                .gameObject
                .GetComponent<TMP_Text>();

        if (scorePercentage < 70)
        {
            postQuizResponse.text = "Would you like to try again?";
        }
        else if (scorePercentage < 90)
        {
            postQuizResponse.text = "Nicely done!";
        }
        else if (scorePercentage > 90)
        {
            postQuizResponse.text = "Great job, you're a master!";
        }

        numQuestionsCorrect.text = "Number of Correct Answers: " + score;
        overallScore.text = "Overall Score: " + scorePercentage + "%";
        timeSpent.text = "Time Spent: 00:00.0"; // TODO: Change to be dynamic
    }
}
