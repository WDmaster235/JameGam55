using UnityEngine;

public class SkeletonSpawnerScript : TowerScript
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

        TowerDefenseGame.Instance.SpawnSkeleton(transform.position, LaneIndex, definition, Level);
        return true;
    }
}
