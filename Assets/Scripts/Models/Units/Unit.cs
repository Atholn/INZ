using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public int HpMax;
    public string Name;
    public int Defense;
    public int AttackPower;
    public int AttackDistance;

    public Texture2D Profile;
    public int Priority;
    public float CreateTime;
    public float HeightPosY;
    public int GoldCost;
    public int WoodCost;

    internal int WhichPlayer;
    internal int Hp;

    protected virtual void Awake()
    {   

    }

    protected virtual void  Start()
    {
        Hp = HpMax;
    }

    protected virtual void Update()
    {
        if (Hp <= 0)
        {
            GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

            gameManager._playersGameObjects[WhichPlayer].Remove(gameObject);
            gameManager.UpdateUnitPoints(WhichPlayer);
            gameManager.CheckWinLose();
        }
    }
}
