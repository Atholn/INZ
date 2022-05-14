using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFollowScript : MonoBehaviour
{
    private float height;
    private int limit;
    private GameManager _gameManager;

    private void Start()
    {

        _gameManager = FindObjectOfType<GameManager>();
        height = _gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.ItemHeightPosY;

        ItemStartPoint startPointUnit = _gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.ItemPrefab.GetComponent<ItemStartPoint>();

        limit = startPointUnit != null ? startPointUnit.BuildSize / 2 : 0;
    }

    private void Update()
    {
        int vx = (int)( _gameManager.v.x - ( _gameManager.v.x % 1));
        int vz = (int)( _gameManager.v.z - ( _gameManager.v.z % 1));


            transform.position = new Vector3(vx, height + 0.2f, vz);
            return;
        


    }
}
