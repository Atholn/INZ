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

    internal int Hp;

    protected virtual void  Start()
    {
        Hp = HpMax;
    }
}
