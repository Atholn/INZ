using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum TypeOfPlayer
    {
        human,
        computer
    }

    internal TypeOfPlayer typeOfPlayer;

    internal int actualUnitsPoint = 0;
    internal int actualMaxUnitsPoint = 0;
    internal int actualWood = 0;
    internal int actualGold = 0;
}
