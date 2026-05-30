using UnityEngine;

public class RangerScript : EnemyScript
{
    [SerializeField] private GameObject rangerShotPrefab;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private float stopDistance = 3f;
    [SerializeField] private float detectionDistance = 10f;
    [SerializeField] private float detectionHeight = 1f;

    private Transform target;

    protected override void FixedUpdate()
    {
        attackTimer -= Time.fixedDeltaTime;

        FindTargetInFront();

        Move();
    }

    protected override void Move()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector2.left * speed;
            return;
        }

        float xDistance = Mathf.Abs(transform.position.x - target.position.x);

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

    // call this from the Ranger attack animation event
    public void FireRangerShot()
    {
        Vector3 spawnPosition = transform.position;

        if (shootPoint != null)
        {
            spawnPosition = shootPoint.position;
        }

        GameObject shot = Instantiate(rangerShotPrefab, spawnPosition, Quaternion.identity);

        RangerShotScript shotScript = shot.GetComponent<RangerShotScript>();

        if (shotScript != null)
        {
            shotScript.SetDamage(attack);
        }
    }

    private void FindTargetInFront()
    {
        Vector2 boxCenter = (Vector2)transform.position + Vector2.left * (detectionDistance / 2f);
        Vector2 boxSize = new Vector2(detectionDistance, detectionHeight);

        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);

        Transform closestTower = null;
        float closestXDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("tower"))
            {
                continue;
            }

            if (hit.transform.position.x > transform.position.x)
            {
                continue;
            }

            float xDistance = Mathf.Abs(transform.position.x - hit.transform.position.x);

            if (xDistance < closestXDistance)
            {
                closestXDistance = xDistance;
                closestTower = hit.transform;
            }
        }

        target = closestTower;
    }
}