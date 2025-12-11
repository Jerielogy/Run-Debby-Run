using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DailyQuizManager : MonoBehaviour
{
    [Header("UI Connections")]
    public GameObject dailyPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI optionAText;
    public TextMeshProUGUI optionBText;

    [Header("Result Section")]
    public GameObject resultSection;
    public TextMeshProUGUI resultTitle; // Just "CORRECT!" or "WRONG!"

    [Header("Data")]
    public QuestionData[] questionBank;

    private QuestionData currentQuestion;
    private const string LAST_PLAYED_KEY = "LastDailyDate";

    public bool debugMode = true;

    public void OpenDailyQuiz()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        string lastPlayed = PlayerPrefs.GetString(LAST_PLAYED_KEY, "");

        if (!debugMode && lastPlayed == today)
        {
            Debug.Log("Already played today!");
            return;
        }

        dailyPanel.SetActive(true);
        resultSection.SetActive(false);

        int randomIndex = UnityEngine.Random.Range(0, questionBank.Length);
        currentQuestion = questionBank[randomIndex];

        questionText.text = currentQuestion.questionText;
        optionAText.text = currentQuestion.optionA;
        optionBText.text = currentQuestion.optionB;
    }

    public void SubmitAnswer(int index)
    {
        bool isCorrect = (index == currentQuestion.correctIndex);

        resultSection.SetActive(true);

        if (isCorrect)
        {
            resultTitle.text = "<color=white>CORRECT!</color>";

            if (currentQuestion.exclusivePhoto != null)
            {
                currentQuestion.exclusivePhoto.Unlock();
                Debug.Log("Unlocked Exclusive Photo");
            }

            PlayerPrefs.SetString(LAST_PLAYED_KEY, DateTime.Now.ToString("yyyy-MM-dd"));
        }
        else
        {
            // Just show WRONG, no explanation
            resultTitle.text = "<color=black>WRONG!</color>";
        }
    }

    public void ClosePanel()
    {
        dailyPanel.SetActive(false);
    }
}