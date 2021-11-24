using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public ChoiceMapManager Manager;

    public void ChangeMapDropdown()
    {
        Manager.LoadMapInfo();
    }

    public void ChangeNumberPlace()
    {
        Manager.ChangeNumberPlace();
    }

    public void ChangeColorUnits()
    {
        Manager.ChangeColorUnits();
    }

    //

    public void PlayButton()
    {
        Manager.Play();
        SceneManager.LoadScene("GameScene");
    }

    public void BackButton()
    {
        OptionsMenu.GoToMainMenu();
    }
}
