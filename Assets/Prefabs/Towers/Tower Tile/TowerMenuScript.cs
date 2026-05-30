using UnityEngine;

public class TowerMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject skeletonTowerPrefab;
    [SerializeField] private GameObject cannonTowerPrefab;
    [SerializeField] private GameObject bowTowerPrefab;
    [SerializeField] private GameObject cowTowerPrefab;

    private TowerTileScript tile;

    public void SetTile(TowerTileScript newTile)
    {
        tile = newTile;
    }

    public void BuySkeletonTower()
    {
        if (TowerDefenseGame.Instance != null)
        {
            tile.PlaceTower(TowerKind.SkeletonSpawner);
        }
        else
        {
            tile.PlaceTower(skeletonTowerPrefab);
        }

        Destroy(gameObject);
    }

    public void BuyCannonTower()
    {
        if (TowerDefenseGame.Instance != null)
        {
            tile.PlaceTower(TowerKind.SplitCanon);
        }
        else
        {
            tile.PlaceTower(cannonTowerPrefab);
        }

        Destroy(gameObject);
    }

    public void BuyXBowTower()
    {
        if (TowerDefenseGame.Instance != null)
        {
            tile.PlaceTower(TowerKind.XBow);
        }
        else
        {
            tile.PlaceTower(bowTowerPrefab);
        }

        Destroy(gameObject);
    }

    public void BuySpaceCowTower()
    {
        if (TowerDefenseGame.Instance != null)
        {
            tile.PlaceTower(TowerKind.SpaceCow);
        }
        else
        {
            tile.PlaceTower(cowTowerPrefab);
        }

        Destroy(gameObject);
    }
}
