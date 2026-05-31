using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public sealed partial class TowerDefenseGame : MonoBehaviour
{
    public const int LaneCount = 4;
    public const int ColumnCount = 7;

    public static TowerDefenseGame Instance { get; private set; }

    [SerializeField] private int startingMilk = 175;
    [SerializeField] private int initialWaveMoney = 55;
    [SerializeField] private float waveMoneyGrowth = 1.35f;
    [SerializeField] private int waveBreakSeconds = 10;
    [SerializeField] private float spawnX = 10.25f;
    [SerializeField] private float offscreenSpawnPadding = 1.25f;
    [SerializeField] private float loseX = -6.45f;
    [SerializeField] private float laneSpawnCooldown = 1f;

    private readonly TowerTileScript[,] tiles = new TowerTileScript[LaneCount, ColumnCount];
    private readonly List<EnemyScript> activeEnemies = new List<EnemyScript>();
    private readonly List<TowerScript> activeTowers = new List<TowerScript>();
    private readonly List<SkeletonScript> activeSkeletons = new List<SkeletonScript>();
    private readonly Dictionary<TowerKind, Button> towerButtons = new Dictionary<TowerKind, Button>();
    private readonly Dictionary<EnemyKind, EnemyStats> enemyStatsByKind = new Dictionary<EnemyKind, EnemyStats>();

    private TowerKind? selectedBuildKind;
    private TowerScript selectedTower;
    private TowerTileScript selectedTile;
    private int milk;
    private int waveNumber;
    private bool gameOver;
    private Text milkText;
    private Text waveText;
    private Text statusText;
    private Text selectedText;
    private Text detailText;
    private Text countdownText;
    private Image milkFill;
    private Button upgradeButton;
    private Button rechargeButton;
    private Button sellButton;
    private Sprite squareSprite;
    private Font defaultFont;
    private TowerDefensePrefabLibrary prefabLibrary;

    public bool GameOver { get { return gameOver; } }
    public float SpawnX { get { return GetEnemySpawnX(); } }
    public float LoseX { get { return loseX; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindAnyObjectByType<TowerDefenseGame>() != null)
        {
            return;
        }

        // the scene can stay empty and still start the game
        GameObject gameObject = new GameObject("Tower Defense Game");
        gameObject.AddComponent<TowerDefenseGame>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        milk = startingMilk;
        defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont == null)
        {
            defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        squareSprite = CreateSquareSprite();
    }

    private void Start()
    {
        ConfigureCameraAndEvents();
        BuildBoard();
        BuildUi();
        UpdateUi();
        StartCoroutine(RunWaves());
    }

    private void Update()
    {
        HandleBoardClickInput();
        UpdateUi();
    }

    private void ConfigureCameraAndEvents()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5f;

            if (mainCamera.GetComponent<Physics2DRaycaster>() == null)
            {
                mainCamera.gameObject.AddComponent<Physics2DRaycaster>();
            }
        }

        if (EventSystem.current == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }
        else if (EventSystem.current.GetComponent<BaseInputModule>() == null)
        {
            EventSystem.current.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }
}
