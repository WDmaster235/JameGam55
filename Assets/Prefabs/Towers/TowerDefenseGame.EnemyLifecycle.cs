using UnityEngine;

public sealed partial class TowerDefenseGame
{
    public void RegisterEnemy(EnemyScript enemy)
    {
        if (enemy != null && !activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void OnEnemyKilled(EnemyScript enemy, int milkDrop)
    {
        // milk stays on the board until the player clicks it
        Vector3 pickupPosition = enemy != null ? enemy.Position : Vector3.zero;
        activeEnemies.Remove(enemy);
        SpawnMilkPickup(pickupPosition, Mathf.Max(1, milkDrop));
    }

    public void CollectMilkPickup(MilkPickupScript pickup, int amount)
    {
        AddMilk(Mathf.Max(1, amount));
        ShowStatus("Picked up " + Mathf.Max(1, amount) + " milk.");
    }

    private void SpawnMilkPickup(Vector3 position, int amount)
    {
        GameObject pickupPrefab = GetMilkPickupPrefab();

        if (pickupPrefab == null)
        {
            ShowMissingPrefab("Milk Pickup");
            return;
        }

        Vector3 pickupPosition = position + new Vector3(0f, 0.15f, -0.35f);
        GameObject pickupObject = Instantiate(pickupPrefab, pickupPosition, Quaternion.identity);
        pickupObject.name = "Milk Pickup +" + amount;

        MilkPickupScript pickup = pickupObject.GetComponent<MilkPickupScript>();

        if (pickup == null)
        {
            Destroy(pickupObject);
            ShowMissingScript("Milk Pickup", "MilkPickupScript");
            return;
        }

        pickup.Initialize(amount, CreateWorldLabel(pickupObject.transform, new Color(0.2f, 0.34f, 0.38f)));
    }

    public void RegisterSkeleton(SkeletonScript skeleton)
    {
        if (skeleton != null && !activeSkeletons.Contains(skeleton))
        {
            activeSkeletons.Add(skeleton);
        }
    }

    public void OnSkeletonDied(SkeletonScript skeleton)
    {
        activeSkeletons.Remove(skeleton);
    }

    public void EnemyEscaped(EnemyScript enemy)
    {
        if (enemy != null)
        {
            activeEnemies.Remove(enemy);
        }

        Lose("An enemy reached the left side.");
    }

    private void SpawnEnemy(EnemyKind enemyKind, int lane)
    {
        EnemyDefinition enemyDefinition = GameDefinitions.GetEnemy(enemyKind);
        EnemyStats enemyStats = GetEnemyStats(enemyKind);
        float y = tiles[lane, 0].transform.position.y;
        // enemies begin past the camera edge
        float enemySpawnX = GetEnemySpawnX();
        GameObject enemyPrefab = GetEnemyPrefab(enemyKind);

        if (enemyPrefab == null)
        {
            ShowMissingPrefab(enemyDefinition.DisplayName);
            return;
        }

        GameObject enemyObject = Instantiate(enemyPrefab, new Vector3(enemySpawnX, y, -0.2f), Quaternion.identity);
        enemyObject.name = enemyDefinition.DisplayName;
        EnemyScript enemy = GetEnemyComponent(enemyObject, enemyKind);

        if (enemy == null)
        {
            Destroy(enemyObject);
            ShowMissingScript(enemyDefinition.DisplayName, GetEnemyScriptName(enemyKind));
            return;
        }

        enemy.Initialize(enemyStats, enemyDefinition, lane);
        enemy.SetLabel(CreateWorldLabel(enemyObject.transform, enemyDefinition.AccentColor));
        RegisterEnemy(enemy);
    }

    private float GetEnemySpawnX()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null || !mainCamera.orthographic)
        {
            return spawnX;
        }

        float rightCameraEdge = mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect;
        return Mathf.Max(spawnX, rightCameraEdge + offscreenSpawnPadding);
    }

    private EnemyScript GetEnemyComponent(GameObject enemyObject, EnemyKind enemyKind)
    {
        switch (enemyKind)
        {
            case EnemyKind.Hound:
                return enemyObject.GetComponent<HoundScript>();
            case EnemyKind.Ranger:
                return enemyObject.GetComponent<RangerScript>();
            default:
                return enemyObject.GetComponent<ZombieScript>();
        }
    }
}
