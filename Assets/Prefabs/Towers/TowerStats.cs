using UnityEngine;

public enum TowerKind
{
    XBow,
    SkeletonSpawner,
    SplitCanon,
    SpaceCow
}

public enum EnemyKind
{
    Hound,
    Zombie,
    Ranger
}

public interface ILaneDamageable
{
    int LaneIndex { get; }
    Vector3 Position { get; }
    bool IsAlive { get; }
    void TakeDamage(int damage);
}

[CreateAssetMenu(fileName = "NewTowerStats", menuName = "Towers/Tower Stats")]
public class TowerStats : ScriptableObject
{
    public GameObject towerPrefab;

    public int health;
    public float actionRate;
    public int cost;
    public int milkStorage;
    public int actionMilkCost;
    public float rechargeInterval;
    public int rechargeMilkAmount;
    public int deathMilkPenalty;
    public int upgradeBaseCost;
}

public sealed class TowerDefinition
{
    public readonly TowerKind Kind;
    public readonly string DisplayName;
    public readonly int Cost;
    public readonly int BaseHealth;
    public readonly float ActionRate;
    public readonly int MilkStorage;
    public readonly int ActionMilkCost;
    public readonly float RechargeInterval;
    public readonly int RechargeMilkAmount;
    public readonly int DeathMilkPenalty;
    public readonly int UpgradeBaseCost;
    public readonly int Damage;
    public readonly float ProjectileSpeed;
    public readonly float Range;
    public readonly float SplashRadius;
    public readonly int MilkProduced;
    public readonly int SkeletonHealth;
    public readonly int SkeletonDamage;
    public readonly float SkeletonSpeed;
    public readonly float SkeletonAttackCooldown;
    public readonly Color MainColor;
    public readonly Color AccentColor;

    public TowerDefinition(
        TowerKind kind,
        string displayName,
        int cost,
        int baseHealth,
        float actionRate,
        int milkStorage,
        int actionMilkCost,
        float rechargeInterval,
        int rechargeMilkAmount,
        int deathMilkPenalty,
        int upgradeBaseCost,
        int damage,
        float projectileSpeed,
        float range,
        float splashRadius,
        int milkProduced,
        int skeletonHealth,
        int skeletonDamage,
        float skeletonSpeed,
        float skeletonAttackCooldown,
        Color mainColor,
        Color accentColor)
    {
        Kind = kind;
        DisplayName = displayName;
        Cost = cost;
        BaseHealth = baseHealth;
        ActionRate = actionRate;
        MilkStorage = milkStorage;
        ActionMilkCost = actionMilkCost;
        RechargeInterval = rechargeInterval;
        RechargeMilkAmount = rechargeMilkAmount;
        DeathMilkPenalty = deathMilkPenalty;
        UpgradeBaseCost = upgradeBaseCost;
        Damage = damage;
        ProjectileSpeed = projectileSpeed;
        Range = range;
        SplashRadius = splashRadius;
        MilkProduced = milkProduced;
        SkeletonHealth = skeletonHealth;
        SkeletonDamage = skeletonDamage;
        SkeletonSpeed = skeletonSpeed;
        SkeletonAttackCooldown = skeletonAttackCooldown;
        MainColor = mainColor;
        AccentColor = accentColor;
    }
}

public sealed class EnemyDefinition
{
    public readonly EnemyKind Kind;
    public readonly string DisplayName;
    public readonly int Health;
    public readonly float Speed;
    public readonly int Attack;
    public readonly float AttackCooldown;
    public readonly int Cost;
    public readonly int MilkDropAmount;
    public readonly float RangerStopDistance;
    public readonly float RangerDetectionDistance;
    public readonly float ProjectileSpeed;
    public readonly Color MainColor;
    public readonly Color AccentColor;

    public EnemyDefinition(
        EnemyKind kind,
        string displayName,
        int health,
        float speed,
        int attack,
        float attackCooldown,
        int cost,
        int milkDropAmount,
        float rangerStopDistance,
        float rangerDetectionDistance,
        float projectileSpeed,
        Color mainColor,
        Color accentColor)
    {
        Kind = kind;
        DisplayName = displayName;
        Health = health;
        Speed = speed;
        Attack = attack;
        AttackCooldown = attackCooldown;
        Cost = cost;
        MilkDropAmount = milkDropAmount;
        RangerStopDistance = rangerStopDistance;
        RangerDetectionDistance = rangerDetectionDistance;
        ProjectileSpeed = projectileSpeed;
        MainColor = mainColor;
        AccentColor = accentColor;
    }
}

public static class GameDefinitions
{
    public static readonly TowerDefinition XBowTower = new TowerDefinition(
        TowerKind.XBow,
        "X-Bow",
        75,
        180,
        0.55f,
        60,
        2,
        8f,
        20,
        28,
        65,
        9,
        5.8f,
        8f,
        0f,
        0,
        0,
        0,
        0f,
        0f,
        new Color(0.42f, 0.73f, 0.92f),
        new Color(0.95f, 0.92f, 0.42f));

    public static readonly TowerDefinition SkeletonSpawnerTower = new TowerDefinition(
        TowerKind.SkeletonSpawner,
        "Skeleton Spawner",
        95,
        120,
        7f,
        100,
        28,
        9f,
        25,
        35,
        85,
        0,
        0f,
        8f,
        0f,
        0,
        45,
        16,
        1.15f,
        0.9f,
        new Color(0.72f, 0.78f, 0.84f),
        new Color(0.45f, 0.38f, 0.67f));

    public static readonly TowerDefinition SplitCanonTower = new TowerDefinition(
        TowerKind.SplitCanon,
        "Split Canon",
        120,
        190,
        3.2f,
        80,
        18,
        10f,
        24,
        45,
        105,
        45,
        3.9f,
        8f,
        0.9f,
        0,
        0,
        0,
        0f,
        0f,
        new Color(0.87f, 0.44f, 0.32f),
        new Color(0.2f, 0.2f, 0.22f));

    public static readonly TowerDefinition SpaceCowTower = new TowerDefinition(
        TowerKind.SpaceCow,
        "Space Cow",
        50,
        65,
        7f,
        0,
        0,
        0f,
        0,
        18,
        55,
        0,
        0f,
        0f,
        0f,
        30,
        0,
        0,
        0f,
        0f,
        new Color(0.94f, 0.94f, 0.98f),
        new Color(0.49f, 0.91f, 0.64f));

    public static readonly TowerDefinition[] AllTowers =
    {
        XBowTower,
        SkeletonSpawnerTower,
        SplitCanonTower,
        SpaceCowTower
    };

    public static readonly EnemyDefinition HoundEnemy = new EnemyDefinition(
        EnemyKind.Hound,
        "Hound",
        35,
        1.65f,
        4,
        0.9f,
        8,
        8,
        0f,
        0f,
        0f,
        new Color(0.93f, 0.63f, 0.25f),
        new Color(0.38f, 0.2f, 0.1f));

    public static readonly EnemyDefinition ZombieEnemy = new EnemyDefinition(
        EnemyKind.Zombie,
        "Zombie",
        75,
        0.85f,
        9,
        1.25f,
        14,
        14,
        0f,
        0f,
        0f,
        new Color(0.49f, 0.77f, 0.47f),
        new Color(0.18f, 0.34f, 0.2f));

    public static readonly EnemyDefinition RangerEnemy = new EnemyDefinition(
        EnemyKind.Ranger,
        "Ranger",
        55,
        0.72f,
        7,
        1.45f,
        24,
        24,
        3f,
        6.5f,
        4.4f,
        new Color(0.7f, 0.48f, 0.9f),
        new Color(0.22f, 0.14f, 0.36f));

    public static readonly EnemyDefinition[] AllEnemies =
    {
        HoundEnemy,
        ZombieEnemy,
        RangerEnemy
    };

    public static TowerDefinition GetTower(TowerKind kind)
    {
        switch (kind)
        {
            case TowerKind.XBow:
                return XBowTower;
            case TowerKind.SkeletonSpawner:
                return SkeletonSpawnerTower;
            case TowerKind.SplitCanon:
                return SplitCanonTower;
            default:
                return SpaceCowTower;
        }
    }

    public static EnemyDefinition GetEnemy(EnemyKind kind)
    {
        switch (kind)
        {
            case EnemyKind.Hound:
                return HoundEnemy;
            case EnemyKind.Zombie:
                return ZombieEnemy;
            default:
                return RangerEnemy;
        }
    }
}
