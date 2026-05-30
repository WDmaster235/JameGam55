using UnityEngine;

public class TowerDefensePrefabLibrary : MonoBehaviour
{
    public GameObject xBowTowerPrefab;
    public GameObject skeletonSpawnerTowerPrefab;
    public GameObject splitCanonTowerPrefab;
    public GameObject spaceCowTowerPrefab;

    public GameObject houndEnemyPrefab;
    public GameObject zombieEnemyPrefab;
    public GameObject rangerEnemyPrefab;

    public GameObject skeletonPrefab;
    public GameObject xBowShotPrefab;
    public GameObject splitCanonShotPrefab;
    public GameObject rangerShotPrefab;
    public GameObject milkPickupPrefab;

    public GameObject GetTowerPrefab(TowerKind towerKind)
    {
        switch (towerKind)
        {
            case TowerKind.SkeletonSpawner:
                return skeletonSpawnerTowerPrefab;
            case TowerKind.SplitCanon:
                return splitCanonTowerPrefab;
            case TowerKind.SpaceCow:
                return spaceCowTowerPrefab;
            default:
                return xBowTowerPrefab;
        }
    }

    public GameObject GetEnemyPrefab(EnemyKind enemyKind)
    {
        switch (enemyKind)
        {
            case EnemyKind.Hound:
                return houndEnemyPrefab;
            case EnemyKind.Ranger:
                return rangerEnemyPrefab;
            default:
                return zombieEnemyPrefab;
        }
    }
}
