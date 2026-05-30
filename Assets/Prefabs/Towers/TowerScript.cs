using UnityEngine;

public abstract class TowerScript : MonoBehaviour, ILaneDamageable
{
    protected TowerStats towerStats;
    protected TowerDefinition definition;
    protected TowerTileScript tile;
    protected TextMesh label;

    protected int currentHealth;
    protected int maxHealth;
    protected float actionRate;
    protected int cost;
    protected int currentMilk;
    protected int milkStorage;
    protected int actionMilkCost;
    protected int rechargeMilkAmount;
    protected float rechargeInterval;
    protected int deathMilkPenalty;
    protected int totalMilkSpent;

    protected float actionTimer;
    protected float rechargeTimer;
    protected bool isAlive = true;

    public int LaneIndex { get; private set; }
    public int ColumnIndex { get; private set; }
    public int Level { get; private set; }
    public Vector3 Position { get { return transform.position; } }
    public bool IsAlive { get { return isAlive; } }
    public TowerKind Kind { get { return definition != null ? definition.Kind : TowerKind.XBow; } }
    public string DisplayName { get { return definition != null ? definition.DisplayName : name; } }
    public int CurrentMilk { get { return currentMilk; } }
    public int MilkStorage { get { return milkStorage; } }
    public int CurrentHealth { get { return currentHealth; } }
    public int MaxHealth { get { return maxHealth; } }
    public int TotalMilkSpent { get { return totalMilkSpent; } }
    public bool UsesMilkStorage { get { return milkStorage > 0; } }

    public virtual void Initialize(TowerTileScript owningTile, TowerDefinition towerDefinition)
    {
        tile = owningTile;
        definition = towerDefinition;
        LaneIndex = owningTile != null ? owningTile.LaneIndex : 0;
        ColumnIndex = owningTile != null ? owningTile.ColumnIndex : 0;
        Level = 0;
        cost = definition.Cost;
        totalMilkSpent = definition.Cost;
        currentMilk = definition.MilkStorage;
        rechargeTimer = definition.RechargeInterval;

        ApplyLevelStats(true);
        actionTimer = Random.Range(0.15f, Mathf.Max(0.25f, actionRate));
        UpdateLabel();
    }

    public virtual void SetTowerStats(TowerStats stats)
    {
        towerStats = stats;

        currentHealth = towerStats.health;
        maxHealth = towerStats.health;
        actionRate = towerStats.actionRate;
        cost = towerStats.cost;
        totalMilkSpent = towerStats.cost;
        milkStorage = towerStats.milkStorage;
        currentMilk = towerStats.milkStorage;
        actionMilkCost = towerStats.actionMilkCost;
        rechargeInterval = towerStats.rechargeInterval;
        rechargeMilkAmount = towerStats.rechargeMilkAmount;
        deathMilkPenalty = towerStats.deathMilkPenalty;
        actionTimer = actionRate;
    }

    public void SetLabel(TextMesh newLabel)
    {
        label = newLabel;
        UpdateLabel();
    }

    protected virtual void Update()
    {
        if (!isAlive || TowerDefenseGame.Instance == null || TowerDefenseGame.Instance.GameOver)
        {
            return;
        }

        RechargeFromMilkBank();

        actionTimer -= Time.deltaTime;

        if (actionTimer <= 0f)
        {
            if (TryAct())
            {
                actionTimer = actionRate;
            }
            else
            {
                actionTimer = 0.2f;
            }
        }

        UpdateLabel();
    }

    protected virtual bool TryAct()
    {
        return false;
    }

    protected bool TryUseActionMilk()
    {
        if (actionMilkCost <= 0 || milkStorage <= 0)
        {
            return true;
        }

        if (currentMilk < actionMilkCost)
        {
            return false;
        }

        currentMilk -= actionMilkCost;
        return true;
    }

    private void RechargeFromMilkBank()
    {
        if (milkStorage <= 0 || rechargeInterval <= 0f || rechargeMilkAmount <= 0)
        {
            return;
        }

        // towers slowly pull milk from the player bank
        rechargeTimer -= Time.deltaTime;

        if (rechargeTimer > 0f)
        {
            return;
        }

        rechargeTimer = rechargeInterval;
        ManualRecharge(rechargeMilkAmount);
    }

    public int ManualRecharge(int requestedAmount)
    {
        if (milkStorage <= 0 || requestedAmount <= 0 || TowerDefenseGame.Instance == null)
        {
            return 0;
        }

        int missingMilk = milkStorage - currentMilk;

        if (missingMilk <= 0)
        {
            return 0;
        }

        int pulledMilk = TowerDefenseGame.Instance.PullMilk(Mathf.Min(requestedAmount, missingMilk));
        currentMilk += pulledMilk;
        UpdateLabel();
        return pulledMilk;
    }

    public int GetUpgradeCost()
    {
        int baseCost = definition != null ? definition.UpgradeBaseCost : Mathf.Max(20, cost);
        return Mathf.CeilToInt(baseCost * Mathf.Pow(1.6f, Level));
    }

    public bool TryUpgrade()
    {
        if (TowerDefenseGame.Instance == null)
        {
            return false;
        }

        int upgradeCost = GetUpgradeCost();

        if (!TowerDefenseGame.Instance.TrySpendMilk(upgradeCost, true))
        {
            return false;
        }

        totalMilkSpent += upgradeCost;
        Level++;
        // upgrades grow slower than their milk price
        ApplyLevelStats(false);
        UpdateLabel();
        return true;
    }

    public int GetSellRefund()
    {
        return Mathf.FloorToInt(totalMilkSpent * 0.5f);
    }

    protected virtual void ApplyLevelStats(bool fullHeal)
    {
        if (definition == null)
        {
            return;
        }

        int previousMaxHealth = maxHealth;
        int previousMilkStorage = milkStorage;
        float growth = GetNLogNGrowth(Level);

        maxHealth = Mathf.RoundToInt(definition.BaseHealth * (1f + 0.16f * growth));
        actionRate = Mathf.Max(0.15f, definition.ActionRate / (1f + 0.07f * growth));
        cost = definition.Cost;
        milkStorage = Mathf.RoundToInt(definition.MilkStorage * (1f + 0.12f * growth));
        actionMilkCost = definition.ActionMilkCost;
        rechargeInterval = definition.RechargeInterval;
        rechargeMilkAmount = Mathf.RoundToInt(definition.RechargeMilkAmount * (1f + 0.05f * growth));
        deathMilkPenalty = Mathf.RoundToInt(definition.DeathMilkPenalty * (1f + 0.08f * growth));

        if (fullHeal || currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Max(0, maxHealth - previousMaxHealth));
        }

        if (milkStorage <= 0)
        {
            currentMilk = 0;
        }
        else if (fullHeal)
        {
            currentMilk = milkStorage;
        }
        else
        {
            currentMilk = Mathf.Min(milkStorage, currentMilk + Mathf.Max(0, milkStorage - previousMilkStorage));
        }
    }

    protected float GetPowerMultiplier(float scale)
    {
        return 1f + scale * GetNLogNGrowth(Level);
    }

    protected float GetNLogNGrowth(int level)
    {
        if (level <= 0)
        {
            return 0f;
        }

        return level * Mathf.Log(level + 1f, 2f);
    }

    public virtual void TakeDamage(int damage)
    {
        if (!isAlive)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
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

        if (TowerDefenseGame.Instance != null)
        {
            TowerDefenseGame.Instance.OnTowerDestroyed(this, deathMilkPenalty);
        }

        Destroy(gameObject);
    }

    protected int GetScaledDamage()
    {
        if (definition == null)
        {
            return 0;
        }

        return Mathf.RoundToInt(definition.Damage * GetPowerMultiplier(0.18f));
    }

    protected virtual void UpdateLabel()
    {
        if (label == null || definition == null)
        {
            return;
        }

        if (milkStorage > 0)
        {
            label.text = DisplayName + "\nLv " + Level + "  HP " + currentHealth + "/" + maxHealth + "\nMilk " + currentMilk + "/" + milkStorage;
        }
        else
        {
            label.text = DisplayName + "\nLv " + Level + "  HP " + currentHealth + "/" + maxHealth;
        }
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
