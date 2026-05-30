using UnityEngine;
using System.Collections.Generic;

public class SplitCanonShotScript : MonoBehaviour
{
    private int laneIndex;
    private int damage;
    private float speed = 3.9f;
    private float splashRadius = 0.9f;
    private float maxX = 7f;

    public void Initialize(int lane, int newDamage, float newSpeed, float newSplashRadius, float destroyX)
    {
        laneIndex = lane;
        damage = newDamage;
        speed = newSpeed;
        splashRadius = newSplashRadius;
        maxX = destroyX;
    }

    private void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        if (transform.position.x >= maxX)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyScript enemy = other.GetComponent<EnemyScript>();

        if (enemy == null || !enemy.IsAlive || enemy.LaneIndex != laneIndex)
        {
            return;
        }

        Vector3 impactPoint = enemy.Position;
        enemy.TakeDamage(damage);

        if (TowerDefenseGame.Instance != null)
        {
            List<EnemyScript> nearbyEnemies = TowerDefenseGame.Instance.GetEnemiesInRadius(impactPoint, laneIndex, splashRadius);
            int splashDamage = Mathf.CeilToInt(damage * 0.5f);

            for (int i = 0; i < nearbyEnemies.Count; i++)
            {
                EnemyScript nearbyEnemy = nearbyEnemies[i];

                if (nearbyEnemy != null && nearbyEnemy != enemy && nearbyEnemy.IsAlive)
                {
                    nearbyEnemy.TakeDamage(splashDamage);
                }
            }
        }

        Destroy(gameObject);
    }
}
