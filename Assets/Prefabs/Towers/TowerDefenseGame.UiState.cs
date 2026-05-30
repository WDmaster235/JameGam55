using UnityEngine;
using UnityEngine.UI;

public sealed partial class TowerDefenseGame
{
    private TextMesh CreateWorldLabel(Transform parent, Color color)
    {
        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(parent, false);
        labelObject.transform.localPosition = new Vector3(0f, 0.72f, -0.05f);
        labelObject.transform.localScale = new Vector3(0.08f, 0.08f, 1f);

        TextMesh textMesh = labelObject.AddComponent<TextMesh>();
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 32;
        textMesh.color = color;
        textMesh.text = "";
        return textMesh;
    }

    public void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private void Lose(string reason)
    {
        if (gameOver)
        {
            return;
        }

        gameOver = true;
        ShowStatus("Game Over: " + reason);
        UpdateUi();
    }

    private void UpdateUi()
    {
        if (milkText == null)
        {
            return;
        }

        // the ui is rebuilt from the current game state
        milkText.text = "Milk: " + milk;
        waveText.text = gameOver ? "Wave " + waveNumber + " - Game Over" : "Wave " + waveNumber;
        milkFill.fillAmount = Mathf.Clamp01(milk / Mathf.Max(250f, milk));

        foreach (TowerDefinition towerDefinition in GameDefinitions.AllTowers)
        {
            Button button;

            if (towerButtons.TryGetValue(towerDefinition.Kind, out button))
            {
                button.interactable = !gameOver && milk >= towerDefinition.Cost;
            }
        }

        if (selectedBuildKind.HasValue)
        {
            TowerDefinition selectedDefinition = GameDefinitions.GetTower(selectedBuildKind.Value);
            selectedText.text = "Selected: " + selectedDefinition.DisplayName;
        }
        else if (selectedTower != null)
        {
            selectedText.text = "Selected: " + selectedTower.DisplayName;
        }
        else
        {
            selectedText.text = "Selected: none";
        }

        if (selectedTower != null && selectedTower.IsAlive)
        {
            int upgradeCost = selectedTower.GetUpgradeCost();
            int sellRefund = selectedTower.GetSellRefund();
            detailText.text = selectedTower.DisplayName + " Lv " + selectedTower.Level
                + "\nHP " + selectedTower.CurrentHealth + "/" + selectedTower.MaxHealth
                + (selectedTower.UsesMilkStorage ? "\nStored milk " + selectedTower.CurrentMilk + "/" + selectedTower.MilkStorage : "\nMakes milk every 7 sec")
                + "\nUpgrade: " + upgradeCost + " milk"
                + "\nSell: " + sellRefund + " milk";
            upgradeButton.interactable = !gameOver && milk - upgradeCost > 0;
            rechargeButton.interactable = !gameOver && selectedTower.UsesMilkStorage && milk > 0 && selectedTower.CurrentMilk < selectedTower.MilkStorage;
            sellButton.interactable = !gameOver;
        }
        else if (selectedBuildKind.HasValue)
        {
            TowerDefinition selectedDefinition = GameDefinitions.GetTower(selectedBuildKind.Value);
            detailText.text = selectedDefinition.DisplayName
                + "\nCost: " + selectedDefinition.Cost + " milk"
                + "\nClick an empty tile to place it.";
            upgradeButton.interactable = false;
            rechargeButton.interactable = false;
            sellButton.interactable = false;
        }
        else
        {
            detailText.text = selectedTile != null ? "Empty tile selected." : "Click a placed tower.";
            upgradeButton.interactable = false;
            rechargeButton.interactable = false;
            sellButton.interactable = false;
        }
    }
}
