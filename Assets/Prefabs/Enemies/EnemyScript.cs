using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyScript : MonoBehaviour
{
    protected EnemyStats enemyStats;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected int currentHp;
    protected float speed;
    protected int attack;
    protected float attackCooldown;
    protected int milkDropAmount;

    protected float attackTimer;

    protected TowerScript targetTower;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        attackTimer -= Time.fixedDeltaTime;

        Move();
    }

    public virtual void SetEnemyStats(EnemyStats stats)
    {
        enemyStats = stats;

        currentHp = enemyStats.hp;
        speed = enemyStats.speed;
        attack = enemyStats.attack;
        attackCooldown = enemyStats.attackCooldown;
        milkDropAmount = enemyStats.milkDropAmount;
    }

    protected virtual void Move()
    {
        if (targetTower != null)
        {
            rb.linearVelocity = Vector2.zero;
            Attack();
            return;
        }

        rb.linearVelocity = Vector2.left * speed;
    }

    protected virtual void Attack()
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
            DealAttackDamage();
        }
    }

    public virtual void DealAttackDamage()
    {
        if (targetTower == null)
        {
            return;
        }

        targetTower.TakeDamage(attack);
    }

    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // milk drop needs to be added here
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("tower"))
        {
            return;
        }

        targetTower = other.GetComponent<TowerScript>();
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("tower"))
        {
            return;
        }

        TowerScript towerHealth = other.GetComponent<TowerScript>();

        if (towerHealth == targetTower)
        {
            targetTower = null;
        }
    }
}