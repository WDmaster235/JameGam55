using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerStats", menuName = "Towers/Tower Stats")]
public class TowerStats : ScriptableObject
{
    public GameObject towerPrefab;

    public int health;
    public float actionRate;
    public int cost;
}