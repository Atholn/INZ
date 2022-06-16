using UnityEngine;

public class GameFollowScript : MonoBehaviour
{
    private float height;
    private int limit;
    private GameManager _gameManager;

    private int[] sizes; //0 - x, 1 - y

    private void Start()
    {

        _gameManager = FindObjectOfType<GameManager>();
        height = _gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.ItemHeightPosY;

        BuildingUnit buildingUnit = _gameManager.GameItemControllers[_gameManager.CurrentButtonPressed].item.ItemPrefab.GetComponent<BuildingUnit>();

        limit = buildingUnit != null ? buildingUnit.Size / 2 : 0;
        sizes = _gameManager.GetSizeMap();
    }

    private void Update()
    {
        int vx = (int)( _gameManager.v.x - ( _gameManager.v.x % 1));
        int vz = (int)( _gameManager.v.z - ( _gameManager.v.z % 1));

        if (vx >= limit && vx <= sizes[0] - limit - 1 &&
             vz >= limit && vz <= sizes[1] - limit - 1)
        {
            transform.position = new Vector3(vx, height + 0.2f, vz);
        }
    }
}
