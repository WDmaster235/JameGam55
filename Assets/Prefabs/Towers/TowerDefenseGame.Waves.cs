using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class TowerDefenseGame
{
    private IEnumerator RunWaves()
    {
        while (!gameOver)
        {
            yield return StartCoroutine(ShowWaveCountdown(waveNumber + 1));

            if (gameOver)
            {
                yield break;
            }

            waveNumber++;

            int waveMoney = Mathf.RoundToInt(initialWaveMoney * Mathf.Pow(waveMoneyGrowth, waveNumber - 1));
            List<EnemyKind>[] wavePurchases = BuildWavePurchases(waveMoney);

            float[] nextLaneSpawnTime = new float[LaneCount];
            int lastSpawnLane = -1;

            ShowStatus("Wave " + waveNumber + " has " + waveMoney + " enemy money.");

            while (!gameOver && HasEnemiesToSpawn(wavePurchases))
            {
                int lane = ChooseLaneWithQueuedEnemy(wavePurchases, nextLaneSpawnTime, lastSpawnLane);

                if (lane < 0)
                {
                    yield return new WaitForSeconds(GetShortestLaneSpawnWait(wavePurchases, nextLaneSpawnTime));
                    continue;
                }

                EnemyKind enemyKind = wavePurchases[lane][0];
                wavePurchases[lane].RemoveAt(0);

                SpawnEnemy(enemyKind, lane);

                nextLaneSpawnTime[lane] = Time.time + laneSpawnCooldown;
                lastSpawnLane = lane;

                yield return new WaitForSeconds(Random.Range(0.45f, 0.95f));
            }

            float clearTimeout = 45f;

            while (!gameOver && activeEnemies.Count > 0 && clearTimeout > 0f)
            {
                clearTimeout -= Time.deltaTime;
                yield return null;
            }

            if (clearTimeout <= 0f)
            {
                Debug.LogWarning("Wave forced to clear.");
                activeEnemies.Clear();
            }

            if (!gameOver)
            {
                DespawnAllSkeletons();
                ShowStatus("Wave " + waveNumber + " cleared. Skeletons despawned.");
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

            string message = "Wave " + nextWaveNumber + " starts in " + second + " seconds.";

            ShowStatus(message);

            if (countdownText != null)
            {
                countdownText.text = message;
            }

            yield return new WaitForSeconds(1f);
        }

        ShowStatus("Wave " + nextWaveNumber + " started.");

        if (countdownText != null)
        {
            countdownText.text = "Wave " + nextWaveNumber + " started!";
        }

        yield return new WaitForSeconds(0.35f);

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    private List<EnemyKind>[] BuildWavePurchases(int waveMoney)
    {
        int[] laneBudgets = SplitMoneyAcrossLanes(waveMoney);
        List<EnemyKind>[] purchases = new List<EnemyKind>[LaneCount];

        for (int lane = 0; lane < LaneCount; lane++)
        {
            purchases[lane] = BuyEnemiesForLane(laneBudgets[lane], waveNumber);
        }

        return purchases;
    }

    private int[] SplitMoneyAcrossLanes(int waveMoney)
    {
        int[] laneBudgets = new int[LaneCount];
        int cheapestEnemyCost = GetCheapestEnemyCost();
        int remainingMoney = waveMoney;

        if (remainingMoney >= cheapestEnemyCost * LaneCount)
        {
            for (int lane = 0; lane < LaneCount; lane++)
            {
                laneBudgets[lane] = cheapestEnemyCost;
                remainingMoney -= cheapestEnemyCost;
            }
        }
        else
        {
            int lane = 0;

            while (remainingMoney >= cheapestEnemyCost)
            {
                laneBudgets[lane % LaneCount] += cheapestEnemyCost;
                remainingMoney -= cheapestEnemyCost;
                lane++;
            }
        }

        while (remainingMoney > 0)
        {
            int lane = GetLaneWithSmallestBudget(laneBudgets);
            int maxChunk = Mathf.Min(remainingMoney, cheapestEnemyCost * 2);
            int amount = Random.Range(1, maxChunk + 1);

            laneBudgets[lane] += amount;
            remainingMoney -= amount;
        }

        return laneBudgets;
    }

    private int GetLaneWithSmallestBudget(int[] laneBudgets)
    {
        int smallestBudget = int.MaxValue;
        List<int> smallestLanes = new List<int>();

        for (int lane = 0; lane < laneBudgets.Length; lane++)
        {
            if (laneBudgets[lane] < smallestBudget)
            {
                smallestBudget = laneBudgets[lane];
                smallestLanes.Clear();
                smallestLanes.Add(lane);
            }
            else if (laneBudgets[lane] == smallestBudget)
            {
                smallestLanes.Add(lane);
            }
        }

        return smallestLanes[Random.Range(0, smallestLanes.Count)];
    }

    private List<EnemyKind> BuyEnemiesForLane(int laneBudget, int currentWave)
    {
        List<EnemyKind> enemies = new List<EnemyKind>();
        int cheapestEnemyCost = GetCheapestEnemyCost();

        while (laneBudget >= cheapestEnemyCost)
        {
            List<EnemyDefinition> affordableEnemies = GetAffordableEnemies(laneBudget, currentWave);

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

    private List<EnemyDefinition> GetAffordableEnemies(int budget, int currentWave)
    {
        List<EnemyDefinition> affordableEnemies = new List<EnemyDefinition>();

        foreach (EnemyDefinition enemyDefinition in GameDefinitions.AllEnemies)
        {
            if (!IsEnemyUnlocked(enemyDefinition.Kind, currentWave))
            {
                continue;
            }

            if (GetEnemyCost(enemyDefinition.Kind) <= budget)
            {
                affordableEnemies.Add(enemyDefinition);
            }
        }

        return affordableEnemies;
    }

    private bool IsEnemyUnlocked(EnemyKind enemyKind, int currentWave)
    {
        switch (enemyKind)
        {
            case EnemyKind.Hound:
                return true;

            case EnemyKind.Zombie:
                return currentWave >= 2;

            case EnemyKind.Ranger:
                return currentWave >= 3;

            default:
                return true;
        }
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

    private int ChooseLaneWithQueuedEnemy(List<EnemyKind>[] wavePurchases, float[] nextLaneSpawnTime, int lastSpawnLane)
    {
        List<int> lanes = new List<int>();

        for (int lane = 0; lane < wavePurchases.Length; lane++)
        {
            if (wavePurchases[lane].Count > 0 && Time.time >= nextLaneSpawnTime[lane])
            {
                lanes.Add(lane);
            }
        }

        if (lanes.Count == 0)
        {
            return -1;
        }

        if (lanes.Count > 1 && lastSpawnLane >= 0)
        {
            lanes.Remove(lastSpawnLane);
        }

        return lanes[Random.Range(0, lanes.Count)];
    }

    private float GetShortestLaneSpawnWait(List<EnemyKind>[] wavePurchases, float[] nextLaneSpawnTime)
    {
        float shortestWait = laneSpawnCooldown;

        for (int lane = 0; lane < wavePurchases.Length; lane++)
        {
            if (wavePurchases[lane].Count <= 0)
            {
                continue;
            }

            shortestWait = Mathf.Min(shortestWait, nextLaneSpawnTime[lane] - Time.time);
        }

        return Mathf.Clamp(shortestWait, 0.05f, laneSpawnCooldown);
    }

    private void DespawnAllSkeletons()
    {
        for (int i = activeSkeletons.Count - 1; i >= 0; i--)
        {
            SkeletonScript skeleton = activeSkeletons[i];

            if (skeleton != null)
            {
                Destroy(skeleton.gameObject);
            }
        }

        activeSkeletons.Clear();
    }
}