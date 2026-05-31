using System.Collections.Generic;
using UnityEngine;

public sealed partial class TowerDefenseGame
{
    public EnemyScript GetFirstEnemyAhead(int lane, float fromX, float range)
    {
        // Towers shoot to the right, so only enemies to the right are valid targets.
        EnemyScript closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            EnemyScript enemy = activeEnemies[i];

            if (enemy == null || !enemy.IsAlive || enemy.LaneIndex != lane)
            {
                continue;
            }

            float distance = enemy.Position.x - fromX;

            if (distance < 0f || distance > range)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public List<EnemyScript> GetEnemiesInRadius(Vector3 center, int lane, float radius)
    {
        List<EnemyScript> enemies = new List<EnemyScript>();
        float radiusSquared = radius * radius;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            EnemyScript enemy = activeEnemies[i];

            if (enemy == null || !enemy.IsAlive || enemy.LaneIndex != lane)
            {
                continue;
            }

            if ((enemy.Position - center).sqrMagnitude <= radiusSquared)
            {
                enemies.Add(enemy);
            }
        }

        return enemies;
    }

    public ILaneDamageable GetClosestDefenderInMeleeRange(int lane, float enemyX, float range)
    {
        // Enemies move left, so they should only attack defenders in front of them.
        // Small negative tolerance keeps melee stable when colliders slightly overlap.
        ILaneDamageable closestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < activeSkeletons.Count; i++)
        {
            SkeletonScript skeleton = activeSkeletons[i];

            if (skeleton == null || !skeleton.IsAlive || skeleton.LaneIndex != lane)
            {
                continue;
            }

            float distance = enemyX - skeleton.Position.x;

            if (distance < -0.15f || distance > range)
            {
                continue;
            }

            float absoluteDistance = Mathf.Abs(distance);

            if (absoluteDistance < closestDistance)
            {
                closestTarget = skeleton;
                closestDistance = absoluteDistance;
            }
        }

        for (int i = 0; i < activeTowers.Count; i++)
        {
            TowerScript tower = activeTowers[i];

            if (tower == null || !tower.IsAlive || tower.LaneIndex != lane)
            {
                continue;
            }

            float distance = enemyX - tower.Position.x;

            if (distance < -0.15f || distance > range)
            {
                continue;
            }

            float absoluteDistance = Mathf.Abs(distance);

            if (absoluteDistance < closestDistance)
            {
                closestTarget = tower;
                closestDistance = absoluteDistance;
            }
        }

        return closestTarget;
    }

    public ILaneDamageable GetFirstDefenderAheadForRanger(int lane, float rangerX, float detectionDistance)
    {
        // Rangers shoot left, so both towers and skeletons in front of the ranger are valid targets.
        ILaneDamageable closestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < activeSkeletons.Count; i++)
        {
            SkeletonScript skeleton = activeSkeletons[i];

            if (skeleton == null || !skeleton.IsAlive || skeleton.LaneIndex != lane)
            {
                continue;
            }

            float distance = rangerX - skeleton.Position.x;

            if (distance < 0f || distance > detectionDistance)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestTarget = skeleton;
                closestDistance = distance;
            }
        }

        for (int i = 0; i < activeTowers.Count; i++)
        {
            TowerScript tower = activeTowers[i];

            if (tower == null || !tower.IsAlive || tower.LaneIndex != lane)
            {
                continue;
            }

            float distance = rangerX - tower.Position.x;

            if (distance < 0f || distance > detectionDistance)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestTarget = tower;
                closestDistance = distance;
            }
        }

        return closestTarget;
    }

    public EnemyScript GetClosestEnemyForSkeleton(int lane, float skeletonX, float range)
    {
        EnemyScript closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            EnemyScript enemy = activeEnemies[i];

            if (enemy == null || !enemy.IsAlive || enemy.LaneIndex != lane)
            {
                continue;
            }

            float distance = enemy.Position.x - skeletonX;

            if (distance < -0.15f || distance > range)
            {
                continue;
            }

            float absoluteDistance = Mathf.Abs(distance);

            if (absoluteDistance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = absoluteDistance;
            }
        }

        return closestEnemy;
    }
}