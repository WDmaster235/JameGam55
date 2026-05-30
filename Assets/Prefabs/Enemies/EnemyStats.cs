using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemies/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public GameObject enemyPrefab;

    public int hp;
    public float speed;
    public int attack;
    public float attackCooldown;
    public int cost;
    public int milkDropAmount;

}