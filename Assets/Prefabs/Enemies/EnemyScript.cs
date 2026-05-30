using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyScript : MonoBehaviour
{
    protected EnemyStats enemyStats;
    protected EnemyDefinition definition;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected TextMesh label;

    protected int currentHp;
    protected int maxHp;
    protected float speed;
    protected int attack;
    protected float attackCooldown;
    protected int milkDropAmount;
    protected int cost;
    protected bool isAlive = true;

    protected float attackTimer;
    protected float meleeRange = 0.62f;

    protected ILaneDamageable targetUnit;

    public int LaneIndex { get; private set; }
    public bool IsAlive { get { return isAlive; } }
    public Vector3 Position { get { return transform.position; } }
    public int Cost { get { return cost; } }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        if (!isAlive || TowerDefenseGame.Instance == null || TowerDefenseGame.Instance.GameOver)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            return;
        }

        attackTimer -= Time.fixedDeltaTime;

        if (transform.position.x <= TowerDefenseGame.Instance.LoseX)
        {
            Escape();
            return;
        }

        Move();
        UpdateLabel();
    }

    public virtual void Initialize(EnemyDefinition enemyDefinition, int laneIndex)
    {
        rb = rb != null ? rb : GetComponent<Rigidbody2D>();
        animator = animator != null ? animator : GetComponent<Animator>();
        definition = enemyDefinition;
        LaneIndex = laneIndex;
        maxHp = enemyDefinition.Health;
        currentHp = maxHp;
        speed = enemyDefinition.Speed;
        attack = enemyDefinition.Attack;
        attackCooldown = enemyDefinition.AttackCooldown;
        milkDropAmount = enemyDefinition.MilkDropAmount;
        cost = enemyDefinition.Cost;
        attackTimer = Random.Range(0f, attackCooldown);
        UpdateLabel();
    }

    public virtual void Initialize(EnemyStats stats, EnemyDefinition enemyDefinition, int laneIndex)
    {
        // use scriptable object values when they are set
        rb = rb != null ? rb : GetComponent<Rigidbody2D>();
        animator = animator != null ? animator : GetComponent<Animator>();
        definition = enemyDefinition;
        enemyStats = stats;
        LaneIndex = laneIndex;

        maxHp = GetStatOrFallback(stats != null ? stats.hp : 0, enemyDefinition.Health);
        currentHp = maxHp;
        speed = GetStatOrFallback(stats != null ? stats.speed : 0f, enemyDefinition.Speed);
        attack = GetStatOrFallback(stats != null ? stats.attack : 0, enemyDefinition.Attack);
        attackCooldown = GetStatOrFallback(stats != null ? stats.attackCooldown : 0f, enemyDefinition.AttackCooldown);
        cost = GetStatOrFallback(stats != null ? stats.cost : 0, enemyDefinition.Cost);
        milkDropAmount = Mathf.Max(1, GetStatOrFallback(stats != null ? stats.milkDropAmount : 0, enemyDefinition.MilkDropAmount));
        attackTimer = Random.Range(0f, attackCooldown);
        UpdateLabel();
    }

    public void SetLabel(TextMesh newLabel)
    {
        label = newLabel;
        UpdateLabel();
    }

    public virtual void SetEnemyStats(EnemyStats stats)
    {
        enemyStats = stats;

        currentHp = enemyStats.hp;
        maxHp = enemyStats.hp;
        speed = enemyStats.speed;
        attack = enemyStats.attack;
        attackCooldown = enemyStats.attackCooldown;
        milkDropAmount = enemyStats.milkDropAmount;
        cost = enemyStats.cost;
    }

    private int GetStatOrFallback(int statValue, int fallbackValue)
    {
        return statValue > 0 ? statValue : fallbackValue;
    }

    private float GetStatOrFallback(float statValue, float fallbackValue)
    {
        return statValue > 0f ? statValue : fallbackValue;
    }

    protected virtual void Move()
    {
        targetUnit = TowerDefenseGame.Instance.GetClosestDefenderInMeleeRange(LaneIndex, transform.position.x, meleeRange);

        if (targetUnit != null && targetUnit.IsAlive)
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
        if (targetUnit == null || !targetUnit.IsAlive)
        {
            return;
        }

        targetUnit.TakeDamage(attack);
    }

    public virtual void TakeDamage(int damage)
    {
        if (!isAlive)
        {
            return;
        }

        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            UpdateLabel();
        }
    }

    protected virtual void Die()
    {
        if (!isAlive)
        {
            return;
        }

        isAlive = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (TowerDefenseGame.Instance != null)
        {
            // death creates a clickable milk drop
            TowerDefenseGame.Instance.OnEnemyKilled(this, milkDropAmount);
        }

        Destroy(gameObject);
    }

    protected virtual void Escape()
    {
        if (!isAlive)
        {
            return;
        }

        isAlive = false;

        if (TowerDefenseGame.Instance != null)
        {
            TowerDefenseGame.Instance.EnemyEscaped(this);
        }

        Destroy(gameObject);
    }

    protected virtual void UpdateLabel()
    {
        if (label == null)
        {
            return;
        }

        string displayName = definition != null ? definition.DisplayName : name;
        label.text = displayName + "\nHP " + currentHp + "/" + maxHp;
    }
}
