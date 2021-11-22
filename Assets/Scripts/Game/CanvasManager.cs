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

    //

    public void PlayButton()
    {
        Manager.Play();
        SceneManager.LoadScene("Sirmish");

        //todo create tmp file to load in skirmish
    }

    public void BackButton()
    {
        OptionsMenu.GoToMainMenu();
    }
}
