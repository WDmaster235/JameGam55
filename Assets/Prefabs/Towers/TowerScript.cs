using UnityEngine;

public abstract class TowerScript : MonoBehaviour
{
    protected TowerStats towerStats;

    protected int currentHealth;
    protected float actionRate;
    protected int cost;

    protected float actionTimer;

    public virtual void SetTowerStats(TowerStats stats)
    {
        towerStats = stats;

        currentHealth = towerStats.health;
        actionRate = towerStats.actionRate;
        cost = towerStats.cost;

        actionTimer = actionRate;
    }

    protected virtual void Update()
    {
        actionTimer -= Time.deltaTime;

        if (actionTimer <= 0f)
        {
            Action();
            actionTimer = actionRate;
        }
    }

    protected virtual void Action()
    {
        // add later
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public int GetCost()
    {
        return cost;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetActionRate()
    {
        return actionRate;
    }
}