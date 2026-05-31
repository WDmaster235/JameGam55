using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed partial class TowerDefenseGame
{
    public void HandleTileClicked(TowerTileScript clickedTile)
    {
        if (gameOver || clickedTile == null)
        {
            return;
        }

        selectedTile = clickedTile;

        if (clickedTile.CurrentTower != null)
        {
            SelectTower(clickedTile.CurrentTower);
            selectedBuildKind = null;
            ShowStatus("Selected " + selectedTower.DisplayName + ".");
            UpdateUi();
            return;
        }

        SelectTower(null);

        if (selectedBuildKind.HasValue)
        {
            TryPlaceTower(clickedTile, selectedBuildKind.Value);
        }
        else
        {
            ShowStatus("Choose a tower first.");
        }
    }

    public void SelectBuild(TowerKind towerKind)
    {
        if (gameOver)
        {
            return;
        }

        selectedBuildKind = towerKind;
        SelectTower(null);
        ShowStatus("Selected " + GameDefinitions.GetTower(towerKind).DisplayName + ".");
        UpdateUi();
    }

    public bool TryPlaceTower(TowerTileScript targetTile, TowerKind towerKind)
    {
        if (targetTile == null || targetTile.CurrentTower != null)
        {
            return false;
        }

        // tower cost is paid before the object is created
        TowerDefinition towerDefinition = GameDefinitions.GetTower(towerKind);

        if (!CanSpendMilk(towerDefinition.Cost, false))
        {
            ShowStatus("Not enough milk for " + towerDefinition.DisplayName + ".");
            return false;
        }

        GameObject towerObject = CreateTowerObject(towerKind, towerDefinition, targetTile.transform.position + new Vector3(0f, 0f, -0.25f));

        if (towerObject == null)
        {
            return false;
        }

        TowerScript tower = GetTowerComponent(towerObject, towerKind);

        if (tower == null)
        {
            Destroy(towerObject);
            ShowMissingScript(towerDefinition.DisplayName, GetTowerScriptName(towerKind));
            return false;
        }

        TrySpendMilk(towerDefinition.Cost, false);
        tower.Initialize(targetTile, towerDefinition);
        tower.SetLabel(CreateWorldLabel(towerObject.transform, towerDefinition.AccentColor));

        targetTile.SetTower(tower);
        activeTowers.Add(tower);
        SelectTower(tower);
        selectedBuildKind = null;
        ShowStatus(towerDefinition.DisplayName + " placed.");
        return true;
    }

    private GameObject CreateTowerObject(TowerKind towerKind, TowerDefinition towerDefinition, Vector3 position)
    {
        GameObject towerPrefab = GetTowerPrefab(towerKind);

        if (towerPrefab == null)
        {
            ShowMissingPrefab(towerDefinition.DisplayName);
            return null;
        }

        GameObject towerObject = Instantiate(towerPrefab, position, Quaternion.identity);
        towerObject.name = towerDefinition.DisplayName;
        return towerObject;
    }

    private TowerScript GetTowerComponent(GameObject towerObject, TowerKind towerKind)
    {
        switch (towerKind)
        {
            case TowerKind.SkeletonSpawner:
                return towerObject.GetComponent<SkeletonSpawnerScript>();
            case TowerKind.SplitCanon:
                return towerObject.GetComponent<SplitCanonScript>();
            case TowerKind.SpaceCow:
                return towerObject.GetComponent<SpaceCowScript>();
            default:
                return towerObject.GetComponent<XBowScript>();
        }
    }

    public void OnTowerDestroyed(TowerScript tower, int penalty)
    {
        // destroyed towers punish the player with a milk loss
        activeTowers.Remove(tower);

        if (tower != null && tower == selectedTower)
        {
            SelectTower(null);
        }

        if (tower != null && tower.Position != Vector3.zero)
        {
            TowerTileScript towerTile = GetTile(tower.LaneIndex, tower.ColumnIndex);

            if (towerTile != null)
            {
                towerTile.ClearTower(tower);
            }
        }

        if (!TrySpendMilk(penalty, false))
        {
            milk = 0;
            Lose("A destroyed tower cost more milk than you had.");
        }
        else
        {
            ShowStatus("A tower broke and cost " + penalty + " milk.");
        }
    }

    public TowerTileScript GetTile(int lane, int column)
    {
        if (lane < 0 || lane >= LaneCount || column < 0 || column >= ColumnCount)
        {
            return null;
        }

        return tiles[lane, column];
    }

    private void UpgradeSelectedTower()
    {
        if (selectedTower == null)
        {
            return;
        }

        int upgradeCost = selectedTower.GetUpgradeCost();

        if (selectedTower.TryUpgrade())
        {
            ShowStatus(selectedTower.DisplayName + " upgraded for " + upgradeCost + " milk.");
        }
        else
        {
            ShowStatus("Not enough spare milk to upgrade.");
        }
    }

    private void RechargeSelectedTower()
    {
        if (selectedTower == null || !selectedTower.UsesMilkStorage)
        {
            return;
        }

        int pulledMilk = selectedTower.ManualRecharge(999);
        ShowStatus(pulledMilk > 0 ? "Recharged with " + pulledMilk + " milk." : "No recharge needed.");
    }

    private void SellSelectedTower()
    {
        if (selectedTower == null || !selectedTower.IsAlive)
        {
            return;
        }

        // selling refunds half of all milk spent on the tower
        TowerScript towerToSell = selectedTower;
        int refund = towerToSell.GetSellRefund();
        TowerTileScript towerTile = GetTile(towerToSell.LaneIndex, towerToSell.ColumnIndex);

        activeTowers.Remove(towerToSell);

        if (towerTile != null)
        {
            towerTile.ClearTower(towerToSell);
            selectedTile = towerTile;
        }

        SelectTower(null);
        AddMilk(refund);
        Destroy(towerToSell.gameObject);
        ShowStatus("Sold tower for " + refund + " milk.");
        UpdateUi();
    }

    private void SelectTower(TowerScript newTower)
    {
        if (selectedTower != null)
        {
            selectedTower.SetLabelVisible(false);
        }

        selectedTower = newTower;

        if (selectedTower != null)
        {
            selectedTower.SetLabelVisible(true);
        }
    }

    private void HandleBoardClickInput()
    {
        if (gameOver || Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();

        if (IsPointerOverUi(screenPosition))
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] != null && hits[i].GetComponent<MilkPickupScript>() != null)
            {
                return;
            }
        }

        TowerTileScript clickedTile = GetTileUnderWorldPoint(worldPosition);

        if (clickedTile != null)
        {
            HandleTileClicked(clickedTile);
        }
    }

    private bool IsPointerOverUi(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].module is GraphicRaycaster)
            {
                return true;
            }
        }

        return false;
    }

    private TowerTileScript GetTileUnderWorldPoint(Vector3 worldPosition)
    {
        TowerTileScript closestTile = null;
        float closestDistanceSquared = Mathf.Infinity;
        const float xTolerance = 0.62f;
        const float yTolerance = 0.62f;

        for (int lane = 0; lane < LaneCount; lane++)
        {
            for (int column = 0; column < ColumnCount; column++)
            {
                TowerTileScript tile = tiles[lane, column];

                if (tile == null)
                {
                    continue;
                }

                Vector3 tilePosition = tile.transform.position;
                float dx = Mathf.Abs(worldPosition.x - tilePosition.x);
                float dy = Mathf.Abs(worldPosition.y - tilePosition.y);

                if (dx > xTolerance || dy > yTolerance)
                {
                    continue;
                }

                float distanceSquared = (worldPosition - tilePosition).sqrMagnitude;

                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestTile = tile;
                }
            }
        }

        return closestTile;
    }
}
