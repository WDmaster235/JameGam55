using UnityEngine;
using UnityEngine.EventSystems;

public class TowerMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject skeletonTowerPrefab;
    [SerializeField] private GameObject cannonTowerPrefab;
    [SerializeField] private GameObject bowTowerPrefab;
    [SerializeField] private GameObject cowTowerPrefab;

    private TowerTileScript tile;
    private int openFrame;

    private void Start()
    {
        openFrame = Time.frameCount;
    }

    private void Update()
    {
        if (Time.frameCount == openFrame)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Destroy(gameObject);
        }
    }

    public void SetTile(TowerTileScript newTile)
    {
        tile = newTile;
    }

    public void BuySkeletonTower()
    {
        tile.PlaceTower(skeletonTowerPrefab);
        Destroy(gameObject);
    }

    public void BuyCannonTower()
    {
        tile.PlaceTower(cannonTowerPrefab);
        Destroy(gameObject);
    }

    public void BuyXBowTower()
    {
        tile.PlaceTower(bowTowerPrefab);
        Destroy(gameObject);
    }

    public void BuySpaceCowTower()
    {
        tile.PlaceTower(cowTowerPrefab);
        Destroy(gameObject);
    }
}