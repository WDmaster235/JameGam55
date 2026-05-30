using UnityEngine;

public sealed partial class TowerDefenseGame
{
    private int GetEnemyCost(EnemyKind enemyKind)
    {
        EnemyStats enemyStats = GetEnemyStats(enemyKind);

        if (enemyStats != null && enemyStats.cost > 0)
        {
            return enemyStats.cost;
        }

        return GameDefinitions.GetEnemy(enemyKind).Cost;
    }

    private EnemyStats GetEnemyStats(EnemyKind enemyKind)
    {
        EnemyStats stats;

        if (enemyStatsByKind.TryGetValue(enemyKind, out stats))
        {
            return stats;
        }

        stats = LoadEnemyStatsAsset(enemyKind);

        if (stats == null)
        {
            stats = CreateRuntimeEnemyStats(enemyKind);
        }

        enemyStatsByKind.Add(enemyKind, stats);
        return stats;
    }

    private EnemyStats CreateRuntimeEnemyStats(EnemyKind enemyKind)
    {
        EnemyDefinition enemyDefinition = GameDefinitions.GetEnemy(enemyKind);
        EnemyStats stats = ScriptableObject.CreateInstance<EnemyStats>();
        stats.hp = enemyDefinition.Health;
        stats.speed = enemyDefinition.Speed;
        stats.attack = enemyDefinition.Attack;
        stats.attackCooldown = enemyDefinition.AttackCooldown;
        stats.cost = enemyDefinition.Cost;
        stats.milkDropAmount = Mathf.Max(1, enemyDefinition.MilkDropAmount);
        return stats;
    }

    private EnemyStats LoadEnemyStatsAsset(EnemyKind enemyKind)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<EnemyStats>(GetEnemyStatsAssetPath(enemyKind));
#else
        return null;
#endif
    }

    private string GetEnemyStatsAssetPath(EnemyKind enemyKind)
    {
        switch (enemyKind)
        {
            case EnemyKind.Hound:
                return "Assets/Prefabs/Enemies/Hound/HoundStats.asset";
            case EnemyKind.Zombie:
                return "Assets/Prefabs/Enemies/Zombie/ZombieStats.asset";
            default:
                return "Assets/Prefabs/Enemies/Ranger/RangerStats.asset";
        }
    }

    private GameObject GetTowerPrefab(TowerKind towerKind)
    {
        ResolvePrefabLibrary();
        return prefabLibrary != null ? prefabLibrary.GetTowerPrefab(towerKind) : null;
    }

    private GameObject GetEnemyPrefab(EnemyKind enemyKind)
    {
        ResolvePrefabLibrary();
        return prefabLibrary != null ? prefabLibrary.GetEnemyPrefab(enemyKind) : null;
    }

    private GameObject GetSkeletonPrefab()
    {
        ResolvePrefabLibrary();
        return prefabLibrary != null ? prefabLibrary.skeletonPrefab : null;
    }

    private GameObject GetXBowShotPrefab()
    {
        ResolvePrefabLibrary();
        return prefabLibrary != null ? prefabLibrary.xBowShotPrefab : null;
    }

    private GameObject GetSplitCanonShotPrefab()
    {
        ResolvePrefabLibrary();
        return prefabLibrary != null ? prefabLibrary.splitCanonShotPrefab : null;
    }

    private GameObject GetRangerShotPrefab()
    {
        ResolvePrefabLibrary();
        return prefabLibrary != null ? prefabLibrary.rangerShotPrefab : null;
    }

    private GameObject GetMilkPickupPrefab()
    {
        ResolvePrefabLibrary();
        return prefabLibrary != null ? prefabLibrary.milkPickupPrefab : null;
    }

    private void ResolvePrefabLibrary()
    {
        if (prefabLibrary != null)
        {
            return;
        }

        prefabLibrary = FindAnyObjectByType<TowerDefensePrefabLibrary>();
    }

    private string GetTowerScriptName(TowerKind towerKind)
    {
        switch (towerKind)
        {
            case TowerKind.SkeletonSpawner:
                return "SkeletonSpawnerScript";
            case TowerKind.SplitCanon:
                return "SplitCanonScript";
            case TowerKind.SpaceCow:
                return "SpaceCowScript";
            default:
                return "XBowScript";
        }
    }

    private string GetEnemyScriptName(EnemyKind enemyKind)
    {
        switch (enemyKind)
        {
            case EnemyKind.Hound:
                return "HoundScript";
            case EnemyKind.Ranger:
                return "RangerScript";
            default:
                return "ZombieScript";
        }
    }

    private void ShowMissingPrefab(string objectName)
    {
        if (prefabLibrary == null)
        {
            ShowStatus("Missing TowerDefensePrefabLibrary in the scene.");
            Debug.LogError("Missing TowerDefensePrefabLibrary in the scene. add it to an active scene GameObject and assign the prefabs there.");
            return;
        }

        ShowStatus("Missing prefab for " + objectName + ".");
        Debug.LogError("Missing prefab for " + objectName + ". Assign it on TowerDefensePrefabLibrary.");
    }

    private void ShowMissingScript(string objectName, string scriptName)
    {
        ShowStatus(objectName + " prefab needs " + scriptName + ".");
        Debug.LogError(objectName + " prefab needs " + scriptName + " on the root object.");
    }
}
