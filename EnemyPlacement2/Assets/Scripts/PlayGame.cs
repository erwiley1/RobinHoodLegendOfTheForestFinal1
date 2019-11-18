//PlayGame script written by Evan Wiley
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayGame : MonoBehaviour
{
    // function that loads scene "Level 1"
    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    // function that loads scene "RHLOTF_Story"
    public void StartStory()
    {
        SceneManager.LoadScene("RHLOTF_Story");
    }
    
    // function that loads scene "RHLOTF_Instructions"
    public void StartInstructions()
    {
        SceneManager.LoadScene("RHLOTF_Instructions");
    }

    // function that loads scene "RHLOTF_Credits"
    public void StartCredits()
    {   
        SceneManager.LoadScene("RHLOTF_Credits");
    }

    // function that loads scene "RHLOTF_Menu"
    public void StartMenu()
    {
        SceneManager.LoadScene("RHLOTF_Menu");
    }

    // function that loads scene "SampleScene"
    public void StartLevel1()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // function that loads scene "RHLOTF_WinScreen"
    public void WinGame()
    {
        SceneManager.LoadScene("RHLOTF_WinScreen");
    }
}
