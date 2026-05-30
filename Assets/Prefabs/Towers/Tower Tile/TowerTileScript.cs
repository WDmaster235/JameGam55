using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class TowerTileScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject towerMenuPrefab;

    private TowerScript currentTower;
    private TowerDefenseGame game;
    private static GameObject openedMenu;

    public int LaneIndex { get; private set; }
    public int ColumnIndex { get; private set; }
    public TowerScript CurrentTower { get { return currentTower; } }

    public void Initialize(int laneIndex, int columnIndex, TowerDefenseGame owningGame)
    {
        LaneIndex = laneIndex;
        ColumnIndex = columnIndex;
        game = owningGame;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HandleClick();
    }

    private void OnMouseDown()
    {
        HandleClick();
    }

    private void HandleClick()
    {
        if (game == null)
        {
            game = TowerDefenseGame.Instance;
        }

        if (game != null)
        {
            game.HandleTileClicked(this);
            return;
        }

        OpenLegacyMenu();
    }

    private void OpenLegacyMenu()
    {
        if (currentTower != null || towerMenuPrefab == null)
        {
            return;
        }

        if (openedMenu != null)
        {
            Destroy(openedMenu);
        }

        Vector3 menuPosition = transform.position + new Vector3(0f, -1.15f, 0f);
        openedMenu = Instantiate(towerMenuPrefab, menuPosition, Quaternion.identity);

        TowerMenuScript menuScript = openedMenu.GetComponent<TowerMenuScript>();

        if (menuScript != null)
        {
            menuScript.SetTile(this);
        }
    }

    public void PlaceTower(TowerKind towerKind)
    {
        if (TowerDefenseGame.Instance != null)
        {
            TowerDefenseGame.Instance.TryPlaceTower(this, towerKind);
        }
    }

    public void PlaceTower(GameObject towerPrefab)
    {
        if (currentTower != null || towerPrefab == null)
        {
            return;
        }

        GameObject towerObject = Instantiate(towerPrefab, transform.position, Quaternion.identity);
        currentTower = towerObject.GetComponent<TowerScript>();
    }

    public void SetTower(TowerScript tower)
    {
        currentTower = tower;
    }

    public void ClearTower(TowerScript tower)
    {
        if (currentTower == tower)
        {
            currentTower = null;
        }
    }
}
