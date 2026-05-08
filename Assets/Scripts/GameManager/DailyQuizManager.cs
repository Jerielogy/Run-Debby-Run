using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class DailyQuizManager : MonoBehaviour
{
    [Header("UI Connections")]
    public GameObject dailyPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI optionAText;
    public TextMeshProUGUI optionBText;

    [Header("Result Section")]
    public GameObject resultSection;
    public TextMeshProUGUI resultTitle;

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

        List<QuestionData> unlockedQuestions = new List<QuestionData>();

        // We check 'LuzonProgress' which is saved in your GameManager
        int currentLuzonProgress = PlayerPrefs.GetInt("LuzonProgress", 0);

        // Question 1: Requires Region 1 Finished (Index 1)
        if (currentLuzonProgress >= 1 && questionBank.Length > 0)
            unlockedQuestions.Add(questionBank[0]);

        // Question 2: Requires Region CAR Finished (Index 2)
        if (currentLuzonProgress >= 2 && questionBank.Length > 1)
            unlockedQuestions.Add(questionBank[1]);

        // Question 3: Requires Region 2 Finished (Index 3)
        if (currentLuzonProgress >= 3 && questionBank.Length > 2)
            unlockedQuestions.Add(questionBank[2]);

        if (unlockedQuestions.Count == 0)
        {
            Debug.Log("No questions unlocked! Finish Luzon levels first.");
            return;
        }

        dailyPanel.SetActive(true);
        resultSection.SetActive(false);

        int randomIndex = UnityEngine.Random.Range(0, unlockedQuestions.Count);
        currentQuestion = unlockedQuestions[randomIndex];

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
            }
            PlayerPrefs.SetString(LAST_PLAYED_KEY, DateTime.Now.ToString("yyyy-MM-dd"));
        }
        else
        {
            resultTitle.text = "<color=black>WRONG!</color>";
        }
    }

    public void ClosePanel()
    {
        dailyPanel.SetActive(false);
    }
}