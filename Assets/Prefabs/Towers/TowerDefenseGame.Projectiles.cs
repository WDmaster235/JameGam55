using UnityEngine;

public sealed partial class TowerDefenseGame
{
    public void SpawnAllyProjectile(Vector3 position, int lane, int damage, float speed, Color color)
    {
        GameObject prefab = GetXBowShotPrefab();
        GameObject shotObject = CreateProjectileObject(prefab, "X-Bow Arrow", position);

        if (shotObject == null)
        {
            return;
        }

        XBowShotScript shot = shotObject.GetComponent<XBowShotScript>();

        if (shot == null)
        {
            Destroy(shotObject);
            ShowMissingScript("X-Bow Arrow", "XBowShotScript");
            return;
        }

        shot.Initialize(lane, damage, speed, SpawnX + 1f);
    }

    public void SpawnSplitCanonProjectile(Vector3 position, int lane, int damage, float speed, float splashRadius, Color color)
    {
        GameObject prefab = GetSplitCanonShotPrefab();
        GameObject shotObject = CreateProjectileObject(prefab, "Split Canon Shot", position);

        if (shotObject == null)
        {
            return;
        }

        SplitCanonShotScript shot = shotObject.GetComponent<SplitCanonShotScript>();

        if (shot == null)
        {
            Destroy(shotObject);
            ShowMissingScript("Split Canon Shot", "SplitCanonShotScript");
            return;
        }

        shot.Initialize(lane, damage, speed, splashRadius, SpawnX + 1f);
    }

    public void SpawnRangerProjectile(Vector3 position, int lane, int damage, float speed, Color color)
    {
        GameObject prefab = GetRangerShotPrefab();
        GameObject shotObject = CreateProjectileObject(prefab, "Ranger Shot", position);

        if (shotObject == null)
        {
            return;
        }

        RangerShotScript shot = shotObject.GetComponent<RangerShotScript>();

        if (shot == null)
        {
            Destroy(shotObject);
            ShowMissingScript("Ranger Shot", "RangerShotScript");
            return;
        }

        shot.Initialize(lane, damage, speed, loseX - 1f);
    }

    private GameObject CreateProjectileObject(GameObject prefab, string objectName, Vector3 position)
    {
        if (prefab == null)
        {
            ShowMissingPrefab(objectName);
            return null;
        }

        GameObject shotObject = Instantiate(prefab, position, Quaternion.identity);
        shotObject.name = objectName;
        return shotObject;
    }

    public void SpawnSkeleton(Vector3 position, int lane, TowerDefinition towerDefinition, int towerLevel)
    {
        float growth = towerLevel <= 0 ? 0f : towerLevel * Mathf.Log(towerLevel + 1f, 2f);
        int health = Mathf.RoundToInt(towerDefinition.SkeletonHealth * (1f + 0.16f * growth));
        int damage = Mathf.RoundToInt(towerDefinition.SkeletonDamage * (1f + 0.18f * growth));
        float speed = towerDefinition.SkeletonSpeed * (1f + 0.05f * growth);

        GameObject skeletonPrefab = GetSkeletonPrefab();
        Vector3 skeletonPosition = position + new Vector3(0.46f, 0f, -0.2f);

        if (skeletonPrefab == null)
        {
            ShowMissingPrefab("Skeleton");
            return;
        }

        GameObject skeletonObject = Instantiate(skeletonPrefab, skeletonPosition, Quaternion.identity);
        skeletonObject.name = "Skeleton";
        SkeletonScript skeleton = skeletonObject.GetComponent<SkeletonScript>();

        if (skeleton == null)
        {
            Destroy(skeletonObject);
            ShowMissingScript("Skeleton", "SkeletonScript");
            return;
        }

        skeleton.Initialize(lane, health, damage, speed, towerDefinition.SkeletonAttackCooldown);
        skeleton.SetLabel(CreateWorldLabel(skeletonObject.transform, new Color(0.18f, 0.18f, 0.18f)));
    }
}
