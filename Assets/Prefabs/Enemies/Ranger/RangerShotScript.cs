using UnityEngine;

public class RangerShotScript : MonoBehaviour
{
    private int damage;

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("tower"))
        {
            return;
        }

        TowerScript tower = other.GetComponent<TowerScript>();

        if (tower != null)
        {
            tower.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}