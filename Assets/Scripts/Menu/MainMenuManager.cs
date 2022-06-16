using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject PlayPanel;
    public GameObject OptionsPanel;

    private void Awake()
    {
        MainMenuPanel.SetActive(true);
        PlayPanel.SetActive(false);
        OptionsPanel.SetActive(false);
    }

    #region MainMenu
    public void Menu(GameObject panel)
    {
        MainMenuPanel.SetActive(!MainMenuPanel.activeSelf);
        panel.SetActive(!panel.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    #region PlayMenu
    public void Campaign()
    {
        SceneManager.LoadScene("Campaign");
    }

    public void Sirmish()
    {
        SceneManager.LoadScene("Sirmish");
    }

    public void Editor()
    {
        SceneManager.LoadScene("Editor");
    }
    #endregion
}
