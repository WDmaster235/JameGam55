using UnityEngine;

public class XBowShotScript : MonoBehaviour
{
    private int laneIndex;
    private int damage;
    private float speed = 5.8f;
    private float maxX = 7f;

    public void Initialize(int lane, int newDamage, float newSpeed, float destroyX)
    {
        laneIndex = lane;
        damage = newDamage;
        speed = newSpeed;
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

        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
