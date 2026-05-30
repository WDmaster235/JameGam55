using UnityEngine;

public sealed class SpaceCowScript : TowerScript
{
    protected override bool TryAct()
    {
        if (definition == null || TowerDefenseGame.Instance == null)
        {
            return false;
        }

        int milkAmount = Mathf.RoundToInt(definition.MilkProduced * GetPowerMultiplier(0.16f));
        TowerDefenseGame.Instance.AddMilk(milkAmount);
        TowerDefenseGame.Instance.ShowStatus(DisplayName + " made " + milkAmount + " milk.");
        return true;
    }
}
