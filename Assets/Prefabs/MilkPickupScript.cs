using UnityEngine;
using UnityEngine.EventSystems;

public sealed class MilkPickupScript : MonoBehaviour, IPointerClickHandler
{
    private int amount;
    private TextMesh label;
    private float lifetime = 20f;
    private float bobTimer;
    private bool collected;

    public void Initialize(int milkAmount, TextMesh pickupLabel)
    {
        amount = Mathf.Max(1, milkAmount);
        label = pickupLabel;

        if (label != null)
        {
            label.text = "+" + amount;
        }
    }

    private void Update()
    {
        bobTimer += Time.deltaTime * 5f;
        transform.localScale = Vector3.one * (0.32f + Mathf.Sin(bobTimer) * 0.035f);

        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Collect();
    }

    private void OnMouseDown()
    {
        Collect();
    }

    private void Collect()
    {
        if (collected)
        {
            return;
        }

        collected = true;

        if (TowerDefenseGame.Instance != null)
        {
            // clicking the pickup adds its milk to the bank
            TowerDefenseGame.Instance.CollectMilkPickup(this, amount);
        }

        Destroy(gameObject);
    }
}
