using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //public void Play()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}

    public void Sirmish()
    {
        SceneManager.LoadScene("SirmishChose");
    }

    public void Campaign()
    {
        SceneManager.LoadScene("CampaignChose");
    }

    public void FreeGame()
    {
        SceneManager.LoadScene("FreeGameChose");
    }

    public void Editor()
    {
        SceneManager.LoadScene("Editor");
    }

    public void OptionsMenu()
    {
        SceneManager.LoadScene("OptionsMenu");
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
