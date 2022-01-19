using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public int WoodResource;

    private void Update()
    {
        if (WoodResource <= 0)
        {
            GameObject.FindObjectOfType<GameManager>().Nature.Remove(gameObject);
            Destroy(gameObject);
        }

    }

    internal void ChoppingProcess(int choppingStrenght = 10)
    {
        if (WoodResource > 0)
            WoodResource -= choppingStrenght;
    }

}
