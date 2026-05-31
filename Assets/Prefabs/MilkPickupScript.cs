using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public sealed class MilkPickupScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool autoCollectWithMouse = true;

    // 0 means the mouse must be directly on the pickup collider.
    // Bigger values collect from farther away.
    [SerializeField] private float collectRange = 0.55f;

    private int amount;
    private TextMesh label;
    private Collider2D pickupCollider;
    private float lifetime = 20f;
    private float bobTimer;
    private bool collected;

    private void Awake()
    {
        pickupCollider = GetComponent<Collider2D>();
    }

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
            return;
        }

        if (autoCollectWithMouse)
        {
            TryAutoCollectWithMouse();
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

    private void TryAutoCollectWithMouse()
    {
        if (collected)
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector2 screenPosition;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null)
        {
            return;
        }

        screenPosition = Mouse.current.position.ReadValue();
#else
        screenPosition = Input.mousePosition;
#endif

        Vector3 worldPosition3D = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));
        Vector2 mouseWorldPosition = new Vector2(worldPosition3D.x, worldPosition3D.y);

        if (collectRange <= 0f)
        {
            if (pickupCollider != null)
            {
                if (pickupCollider.OverlapPoint(mouseWorldPosition))
                {
                    Collect();
                }

                return;
            }

            if (((Vector2)transform.position - mouseWorldPosition).sqrMagnitude <= 0.04f)
            {
                Collect();
            }

            return;
        }

        float collectRangeSquared = collectRange * collectRange;
        float distanceSquared = ((Vector2)transform.position - mouseWorldPosition).sqrMagnitude;

        if (distanceSquared <= collectRangeSquared)
        {
            Collect();
        }
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
            TowerDefenseGame.Instance.CollectMilkPickup(this, amount);
        }

        Destroy(gameObject);
    }
}