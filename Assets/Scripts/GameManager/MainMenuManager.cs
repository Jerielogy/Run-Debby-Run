using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // You MUST add this to change scenes

public class MainMenuManager : MonoBehaviour
{
    // This function will be for your "Play" button
    public void StartGame()
    {
        // Make sure your game scene is named "Game"
        // or "Region1-Level1"
        SceneTransitionManager.Instance.LoadScene("Region1 Level1");
    }

    // This function is for your "Options" button
    public void GoToOptions()
    {
        // Make sure your options scene is named "OptionsScene"
        SceneManager.LoadScene("OptionsScene");
    }

    // This function is for your "Exit" button
    public void ExitGame()
    {
        Debug.Log("Quitting game..."); // This shows a message in the editor
        Application.Quit(); // This only works in a real, built game
    }
}