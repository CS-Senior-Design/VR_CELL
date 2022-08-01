using System.Collections;
using System.Diagnostics;
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

    private Stopwatch stopWatch;

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
        stopWatch = System.Diagnostics.Stopwatch.StartNew();

        foreach (Question q in questions)
        {
            questionStackBin.Push (q);
        }

        foreach (Question q in questions)
        {
            Question tempQuestion = questionStackBin.Pop();
            questionStack.Push (tempQuestion);
            UnityEngine.Debug.Log(tempQuestion.questionPrompt);
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
    public void EndQuiz()
    {
        UnityEngine.Debug.Log("Ending quiz");
        stopWatch.Stop();
        var elapsedSeconds = stopWatch.Elapsed.TotalSeconds;

        // Calculate final score percentage
        float scorePercentage = (float)score / (float)numOfQuestions * 100;
        UnityEngine.Debug.Log("Score: " + scorePercentage + "%");

        // Show results panel, hide quiz panel
        uiPanel.SetActive(false);
        scorePanel.SetActive(true);

        // Get references to panel info
        GameObject uiCanvas = scorePanel.transform.GetChild(0).gameObject;
        TMP_Text postQuizResponse = uiCanvas.transform.Find("Post-Quiz Response").gameObject.GetComponent<TMP_Text>();
        TMP_Text numQuestionsCorrect = uiCanvas.transform.Find("Num Questions Correct").gameObject.GetComponent<TMP_Text>();
        TMP_Text overallScore = uiCanvas.transform.Find("Overall Score").gameObject.GetComponent<TMP_Text>();
        TMP_Text timeSpent = uiCanvas.transform.Find("Time Spent").gameObject.GetComponent<TMP_Text>();

        // Set Post Quiz Response based on score percentage
        if (scorePercentage < 70)
        {
            postQuizResponse.text = "You're not quite there yet! Would you like to try again?";
        }
        else if (scorePercentage < 80)
        {
            postQuizResponse.text = "You're pretty good!";
        }
        else if (scorePercentage < 90)
        {
            postQuizResponse.text = "You're pretty great!";
        }
        else
        {
            postQuizResponse.text = "You're a master! Great job.";
        }

        // Set Num Questions Correct
        numQuestionsCorrect.text = "You got " + score.ToString() + " out of " + numOfQuestions.ToString() + " questions correct!";
        // Set Overall Score
        overallScore.text = "Your overall score is " + scorePercentage.ToString("F2") + "%";
        // Set Time Spent
        timeSpent.text = "You spent " + elapsedSeconds.ToString("F2") + " seconds on the quiz!";
    }
}
