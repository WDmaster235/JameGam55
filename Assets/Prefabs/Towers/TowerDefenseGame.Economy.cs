public sealed partial class TowerDefenseGame
{
    public void AddMilk(int amount)
    {
        if (amount <= 0 || gameOver)
        {
            return;
        }

        milk += amount;
        UpdateUi();
    }

    public bool TrySpendMilk(int amount, bool requirePositiveAfter)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (!CanSpendMilk(amount, requirePositiveAfter))
        {
            return false;
        }

        milk -= amount;
        UpdateUi();
        return true;
    }

    private bool CanSpendMilk(int amount, bool requirePositiveAfter)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (requirePositiveAfter)
        {
            if (milk - amount <= 0)
            {
                return false;
            }
        }
        else if (milk < amount)
        {
            return false;
        }

        return true;
    }

    public int PullMilk(int amount)
    {
        if (amount <= 0 || milk <= 0 || gameOver)
        {
            return 0;
        }

        int pulledMilk = UnityEngine.Mathf.Min(amount, milk);
        milk -= pulledMilk;
        UpdateUi();
        return pulledMilk;
    }
}
