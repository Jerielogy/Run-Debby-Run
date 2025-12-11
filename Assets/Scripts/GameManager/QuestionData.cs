using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Daily/Question")]
public class QuestionData : ScriptableObject
{
    [Header("The Challenge")]
    [TextArea] public string questionText; // The question text
    public string optionA;                 // First choice 
    public string optionB;                 // Second choice

    [Header("The Solution")]
    [Tooltip("Set to 0 for Option A. Set to 1 for Option B.")]
    public int correctIndex;               // 0 or 1

    [Header("The Reward")]
    [TextArea] public string triviaFact;   // Fun fact shown after answering
    public PhotoData exclusivePhoto;       // The specific photo they win
}