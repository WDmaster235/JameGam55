using UnityEngine;

public class RangerScript : EnemyScript
{
    [SerializeField] private GameObject rangerShotPrefab;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private float stopDistance = 3f;
    [SerializeField] private float detectionDistance = 30f;

    public override void Initialize(EnemyDefinition enemyDefinition, int laneIndex)
    {
        base.Initialize(enemyDefinition, laneIndex);
        stopDistance = enemyDefinition.RangerStopDistance;
        detectionDistance = enemyDefinition.RangerDetectionDistance;
    }

    public override void Initialize(EnemyStats stats, EnemyDefinition enemyDefinition, int laneIndex)
    {
        base.Initialize(stats, enemyDefinition, laneIndex);
        stopDistance = enemyDefinition.RangerStopDistance;
        detectionDistance = enemyDefinition.RangerDetectionDistance;
    }

    protected override void Move()
    {
        targetUnit = TowerDefenseGame.Instance.GetFirstDefenderAheadForRanger(
            LaneIndex,
            transform.position.x,
            detectionDistance
        );

        if (targetUnit == null)
        {
            rb.linearVelocity = Vector2.left * speed;
            return;
        }

        float xDistance = transform.position.x - targetUnit.Position.x;

        if (xDistance > stopDistance)
        {
            rb.linearVelocity = Vector2.left * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            Attack();
        }
    }

    protected override void Attack()
    {
        if (attackTimer > 0)
        {
            return;
        }

        attackTimer = attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            FireRangerShot();
        }
    }

    // Animation events can use this shot too.
    public void FireRangerShot()
    {
        if (TowerDefenseGame.Instance == null || targetUnit == null || !targetUnit.IsAlive)
        {
            return;
        }

        Vector3 spawnPosition = transform.position;

        if (shootPoint != null)
        {
            spawnPosition = shootPoint.position;
        }
        else
        {
            spawnPosition += Vector3.left * 0.42f;
        }

        float projectileSpeed = definition != null ? definition.ProjectileSpeed : 4.4f;
        Color shotColor = definition != null ? definition.AccentColor : Color.magenta;

        TowerDefenseGame.Instance.SpawnRangerProjectile(
            spawnPosition,
            LaneIndex,
            attack,
            projectileSpeed,
            shotColor
        );
    }
}