using UnityEngine;

public class GoldMine : MonoBehaviour
{
    public int GoldResource;

    internal void DiggingGoldmine()
    {
        GoldResource -= 10;
        if (GoldResource <= 0)
        {
            GameObject.FindObjectOfType<GameManager>().Nature.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
