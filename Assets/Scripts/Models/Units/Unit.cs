using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public int HpMax;
    public string Name;
    public int Defense;
    public int Attack;

    public Texture2D Profile;
    public int Priority;
    public float CreateTime;

    internal int whichPlayer;
    internal int Hp;

    protected virtual void  Start()
    {
        Hp = HpMax;
    }

    private void Update()
    {
        if (Hp <= 0)
        {
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            gameManager._playersGameObjects[whichPlayer].Remove(gameObject);
            Destroy(gameObject);

            gameManager.CheckWinLose();
        }
    }
}
