using UnityEngine;

public class SplitCanonScript : TowerScript
{
    protected override bool TryAct()
    {
        if (TowerDefenseGame.Instance == null || definition == null)
        {
            return false;
        }

        EnemyScript target = TowerDefenseGame.Instance.GetFirstEnemyAhead(LaneIndex, transform.position.x, definition.Range);

        if (target == null || !TryUseActionMilk())
        {
            return false;
        }

        Vector3 spawnPosition = transform.position + new Vector3(0.46f, 0f, 0f);
        TowerDefenseGame.Instance.SpawnSplitCanonProjectile(spawnPosition, LaneIndex, GetScaledDamage(), definition.ProjectileSpeed, definition.SplashRadius, definition.AccentColor);
        return true;
    }
}
