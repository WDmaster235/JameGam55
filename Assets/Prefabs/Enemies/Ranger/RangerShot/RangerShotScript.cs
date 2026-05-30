using UnityEngine;

public class RangerShotScript : MonoBehaviour
{
    private int damage;
    private int laneIndex;
    private float speed = 4.4f;
    private float minX = -7f;

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void Initialize(int lane, int newDamage, float newSpeed, float destroyX)
    {
        laneIndex = lane;
        damage = newDamage;
        speed = newSpeed;
        minX = destroyX;
    }

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x <= minX)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TowerScript tower = other.GetComponent<TowerScript>();

        if (tower != null && tower.IsAlive && tower.LaneIndex == laneIndex)
        {
            tower.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        SkeletonScript skeleton = other.GetComponent<SkeletonScript>();

        if (skeleton != null && skeleton.IsAlive && skeleton.LaneIndex == laneIndex)
        {
            skeleton.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
