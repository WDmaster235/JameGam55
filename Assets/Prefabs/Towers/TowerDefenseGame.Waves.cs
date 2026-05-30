using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class TowerDefenseGame
{
    private IEnumerator RunWaves()
    {
        while (!gameOver)
        {
            // every wave starts after a visible break
            yield return StartCoroutine(ShowWaveCountdown(waveNumber + 1));

            if (gameOver)
            {
                yield break;
            }

            waveNumber++;
            int waveMoney = Mathf.RoundToInt(initialWaveMoney * Mathf.Pow(waveMoneyGrowth, waveNumber - 1));
            List<EnemyKind>[] wavePurchases = BuildWavePurchases(waveMoney);
            ShowStatus("Wave " + waveNumber + " has " + waveMoney + " enemy money.");

            while (!gameOver && HasEnemiesToSpawn(wavePurchases))
            {
                int lane = ChooseLaneWithQueuedEnemy(wavePurchases);
                EnemyKind enemyKind = wavePurchases[lane][0];
                wavePurchases[lane].RemoveAt(0);
                SpawnEnemy(enemyKind, lane);
                yield return new WaitForSeconds(Random.Range(0.45f, 1.05f));
            }

            while (!gameOver && activeEnemies.Count > 0)
            {
                yield return null;
            }

            if (!gameOver)
            {
                ShowStatus("Wave " + waveNumber + " cleared.");
            }
        }
    }

    private IEnumerator ShowWaveCountdown(int nextWaveNumber)
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
        }

        for (int second = waveBreakSeconds; second > 0; second--)
        {
            if (gameOver)
            {
                yield break;
            }

            ShowStatus("Next wave starts in " + second + ".");

            if (countdownText != null)
            {
                countdownText.text = "Wave " + nextWaveNumber + "\n" + second;
            }

            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null)
        {
            countdownText.text = "Wave " + nextWaveNumber + "\nGo!";
        }

        yield return new WaitForSeconds(0.35f);

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    private List<EnemyKind>[] BuildWavePurchases(int waveMoney)
    {
        // wave money is split across the four lanes
        int[] laneBudgets = SplitMoneyAcrossLanes(waveMoney);
        List<EnemyKind>[] purchases = new List<EnemyKind>[LaneCount];

        for (int lane = 0; lane < LaneCount; lane++)
        {
            purchases[lane] = BuyEnemiesForLane(laneBudgets[lane]);
        }

        return purchases;
    }

    private int[] SplitMoneyAcrossLanes(int waveMoney)
    {
        int[] laneBudgets = new int[LaneCount];
        int minimumLaneMoney = Mathf.FloorToInt(waveMoney * 0.1f);
        int remainingMoney = waveMoney;

        for (int lane = 0; lane < LaneCount; lane++)
        {
            laneBudgets[lane] = minimumLaneMoney;
            remainingMoney -= minimumLaneMoney;
        }

        while (remainingMoney > 0)
        {
            int lane = Random.Range(0, LaneCount);
            int amount = Random.Range(1, remainingMoney + 1);
            laneBudgets[lane] += amount;
            remainingMoney -= amount;
        }

        return laneBudgets;
    }

    private List<EnemyKind> BuyEnemiesForLane(int laneBudget)
    {
        List<EnemyKind> enemies = new List<EnemyKind>();
        int cheapestEnemyCost = GetCheapestEnemyCost();

        while (laneBudget >= cheapestEnemyCost)
        {
            List<EnemyDefinition> affordableEnemies = GetAffordableEnemies(laneBudget);

            if (affordableEnemies.Count == 0)
            {
                break;
            }

            EnemyDefinition chosenEnemy = affordableEnemies[Random.Range(0, affordableEnemies.Count)];
            enemies.Add(chosenEnemy.Kind);
            laneBudget -= GetEnemyCost(chosenEnemy.Kind);
        }

        return enemies;
    }

    private int GetCheapestEnemyCost()
    {
        int cheapest = int.MaxValue;

        foreach (EnemyDefinition enemyDefinition in GameDefinitions.AllEnemies)
        {
            cheapest = Mathf.Min(cheapest, GetEnemyCost(enemyDefinition.Kind));
        }

        return cheapest;
    }

    private List<EnemyDefinition> GetAffordableEnemies(int budget)
    {
        List<EnemyDefinition> affordableEnemies = new List<EnemyDefinition>();

        foreach (EnemyDefinition enemyDefinition in GameDefinitions.AllEnemies)
        {
            if (GetEnemyCost(enemyDefinition.Kind) <= budget)
            {
                affordableEnemies.Add(enemyDefinition);
            }
        }

        return affordableEnemies;
    }

    private bool HasEnemiesToSpawn(List<EnemyKind>[] wavePurchases)
    {
        for (int lane = 0; lane < wavePurchases.Length; lane++)
        {
            if (wavePurchases[lane].Count > 0)
            {
                return true;
            }
        }

        return false;
    }

    private int ChooseLaneWithQueuedEnemy(List<EnemyKind>[] wavePurchases)
    {
        List<int> lanes = new List<int>();

        for (int lane = 0; lane < wavePurchases.Length; lane++)
        {
            if (wavePurchases[lane].Count > 0)
            {
                lanes.Add(lane);
            }
        }

        return lanes[Random.Range(0, lanes.Count)];
    }
}
