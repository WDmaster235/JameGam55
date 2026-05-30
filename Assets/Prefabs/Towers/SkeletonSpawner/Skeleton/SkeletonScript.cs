using UnityEngine;

public class SkeletonScript : MonoBehaviour, ILaneDamageable
{
    private Rigidbody2D rb;
    private TextMesh label;
    private int currentHealth;
    private int maxHealth;
    private int damage;
    private float speed;
    private float attackCooldown;
    private float attackTimer;
    private float attackRange = 0.62f;
    private bool isAlive = true;

    public int LaneIndex { get; private set; }
    public Vector3 Position { get { return transform.position; } }
    public bool IsAlive { get { return isAlive; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(int lane, int health, int attackDamage, float moveSpeed, float cooldown)
    {
        rb = rb != null ? rb : GetComponent<Rigidbody2D>();
        LaneIndex = lane;
        maxHealth = health;
        currentHealth = health;
        damage = attackDamage;
        speed = moveSpeed;
        attackCooldown = cooldown;
        attackTimer = Random.Range(0f, attackCooldown);
        UpdateLabel();
    }

    public void SetLabel(TextMesh newLabel)
    {
        label = newLabel;
        UpdateLabel();
    }

    private void OnEnable()
    {
        if (TowerDefenseGame.Instance != null)
        {
            TowerDefenseGame.Instance.RegisterSkeleton(this);
        }
    }

    private void FixedUpdate()
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
        EnemyScript target = TowerDefenseGame.Instance.GetClosestEnemyForSkeleton(LaneIndex, transform.position.x, attackRange);

        if (target != null && target.IsAlive)
        {
            rb.linearVelocity = Vector2.zero;
            Attack(target);
        }
        else
        {
            rb.linearVelocity = Vector2.right * speed;
        }

        if (transform.position.x > TowerDefenseGame.Instance.SpawnX + 1f)
        {
            Die();
        }

        UpdateLabel();
    }

    private void Attack(EnemyScript target)
    {
        if (attackTimer > 0f)
        {
            return;
        }

        attackTimer = attackCooldown;
        target.TakeDamage(damage);
    }

    public void TakeDamage(int incomingDamage)
    {
        if (!isAlive)
        {
            return;
        }

        currentHealth -= incomingDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            UpdateLabel();
        }
    }

    private void Die()
    {
        if (!isAlive)
        {
            return;
        }

        isAlive = false;

        if (TowerDefenseGame.Instance != null)
        {
            TowerDefenseGame.Instance.OnSkeletonDied(this);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (TowerDefenseGame.Instance != null)
        {
            TowerDefenseGame.Instance.OnSkeletonDied(this);
        }
    }

    private void UpdateLabel()
    {
        if (label == null)
        {
            return;
        }

        label.text = "Skeleton\nHP " + currentHealth + "/" + maxHealth;
    }
}
