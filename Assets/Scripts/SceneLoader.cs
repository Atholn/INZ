using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static void GameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public static void Editor()
    {
        SceneManager.LoadScene("Editor");
    }
}
