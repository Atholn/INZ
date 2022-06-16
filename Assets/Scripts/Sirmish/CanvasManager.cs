using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public ChoiceMapManager Manager;

    public void ChangeMapDropdown()
    {
        Manager.LoadMapInfo();
    }

    public void ChangeTypeOfPlayer()
    {
        Manager.ChangeTypeOfPlayer();
    }

    public void ChangeColorUnits()
    {
        Manager.ChangeColorUnits();
    }

    public void ChangeNumberPlace()
    {
        Manager.ChangeNumberPlace();
    }

    public void PlayButton()
    {
        Manager.Play();
        SceneLoader.GameScene();
    }

    public void BackButton()
    {
        SceneLoader.MainMenu();
    }
}
