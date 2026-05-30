using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TowerTileScript : MonoBehaviour
{
    [SerializeField] private GameObject towerMenuPrefab;

    private GameObject currentTower;
    private static GameObject openedMenu;

    private void OnMouseDown()
    {
        if (currentTower != null)
        {
            return;
        }

        if (openedMenu != null)
        {
            Destroy(openedMenu);
        }

        Vector3 menuPosition = transform.position + new Vector3(0f, 1f, 0f);

        openedMenu = Instantiate(towerMenuPrefab, menuPosition, Quaternion.identity);

        TowerMenuScript menuScript = openedMenu.GetComponent<TowerMenuScript>();
        menuScript.SetTile(this);
    }

    public void PlaceTower(GameObject towerPrefab)
    {
        if (currentTower != null)
        {
            return;
        }

        currentTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
    }
}