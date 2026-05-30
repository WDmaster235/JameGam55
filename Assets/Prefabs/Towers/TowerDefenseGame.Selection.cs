using UnityEngine;

public sealed partial class TowerDefenseGame
{
    public void HandleTileClicked(TowerTileScript clickedTile)
    {
        if (gameOver || clickedTile == null)
        {
            return;
        }

        // clicking a tile either selects a tower or places one
        selectedTile = clickedTile;

        if (clickedTile.CurrentTower != null)
        {
            selectedTower = clickedTile.CurrentTower;
            selectedBuildKind = null;
            ShowStatus("Selected " + selectedTower.DisplayName + ".");
            UpdateUi();
            return;
        }

        selectedTower = null;

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
        selectedTower = null;
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
        selectedTower = tower;
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
            selectedTower = null;
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

        selectedTower = null;
        AddMilk(refund);
        Destroy(towerToSell.gameObject);
        ShowStatus("Sold tower for " + refund + " milk.");
        UpdateUi();
    }
}
