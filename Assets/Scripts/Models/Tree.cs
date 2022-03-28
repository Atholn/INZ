using UnityEngine;

public class Tree : MonoBehaviour
{
    public int WoodResource;

    private void Update()
    {

    }

    internal void ChoppingProcess(int choppingStrenght = 10)
    {
        if (WoodResource > 0)
            WoodResource -= choppingStrenght;

        if (WoodResource <= 0)
        {
            GameObject.FindObjectOfType<GameManager>().Nature.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
