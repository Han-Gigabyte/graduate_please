using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Progress")]
    [SerializeField] private int score;
    [SerializeField] private int stage;
    [SerializeField] private int chapter;
    [SerializeField] private bool isPlayerInRange;

    // 프로퍼티를 통한 접근
    public int Score => score;
    public int Stage
    {
        get => stage;
        set => stage = value;
    }
    public int Chapter => chapter;
    public bool IsPlayerInRange
    {
        get => isPlayerInRange;
        set => isPlayerInRange = value;
    }

    [Header("Player Settings")]
    public float playerMoveSpeed = 5f;
    public float playerJumpForce = 9.6f;
    public float playerGravityScale = 2.5f;
    public int playerMaxJumpCount = 2;
    public float playerDashForce = 15f;
    public float playerDashCooldown = 5f;
    public int playerMaxHealth = 100;

    [Header("Melee Enemy Settings")]
    public float meleeEnemyMoveSpeed = 3f;
    public float meleeEnemyDetectionRange = 5f;
    public int meleeEnemyDamage = 10;
    public float meleeEnemyKnockbackForce = 9f;
    public float meleeEnemyDamageCooldown = 0.5f;

    [Header("Ranged Enemy Settings")]
    public float rangedEnemyMoveSpeed = 3f;
    public float rangedEnemyAttackRange = 7f;
    public float rangedEnemyDetectionRange = 15f;
    public int rangedEnemyDamage = 10;
    public float rangedEnemyAttackCooldown = 2f;
    public float rangedEnemyProjectileSpeed = 8f;

    [Header("Enemy Projectile Settings")]
    public float enemyProjectileKnockbackForce = 8f;

    private const int MAX_STAGE = 10;  // 스테이지 최대값 상수 추가

    // 플레이어 현재 상태 저장용 변수
    private int currentPlayerHealth;
    public int CurrentPlayerHealth
    {
        get => currentPlayerHealth;
        set => currentPlayerHealth = value;
    }


    [Header("UI Prefabs")]
    public GameObject playerUIPrefab; // PlayerUI 프리팹을 위한 변수
    public SpriteRenderer playerSpriteRenderer; // 게임 캐릭터의 SpriteRenderer


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGameState()
    {
        chapter = 1;
        stage = 1;
        score = 0;
        isPlayerInRange = false;
        currentPlayerHealth = playerMaxHealth;
    }

    // 스테이지 진행 관련 메서드
    public void AdvanceStage()
    {
        SavePlayerState();  // 씬 전환 전에 플레이어 상태 저장
        stage++;
        
        if (stage > MAX_STAGE)
        {
            chapter++;
            stage = 1;
            Debug.Log($"Chapter {chapter} Started!");
        }
        else
        {
            Debug.Log($"Stage {stage} Started!");
        }

        SavePlayerState();
    }

    public void AddScore(int points)
    {
        score += points;
    }

    // 플레이어 상태 저장/복원 메서드!
    public void SavePlayerState()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            currentPlayerHealth = player.CurrentHealth;
            Debug.Log($"Saved player health: {currentPlayerHealth}");  // 디버그용
            // 필요한 다른 플레이어 상태도 여기서 저장
        }
        else
        {
            Debug.LogWarning("Player not found when trying to save state!");
        }
    }

    // 새로운 씬에서 호출될 메서드
    public void RestorePlayerState(IDamageable player)
    {
        if (player != null)
        {
            player.RestoreHealth(currentPlayerHealth); // 현재 체력을 복원
            Debug.Log($"Restored player health: {currentPlayerHealth}");  // 디버그용
        }
        else
        {
            Debug.LogWarning("Player not found when trying to restore state!");
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstance()
    {
        if (Instance == null && !FindObjectOfType<GameManager>())
        {
            GameObject go = new GameObject("GameManager");
            Instance = go.AddComponent<GameManager>();
        }
    }

    public void LoadNextStage()
    {
        SavePlayerState();  // 현재 플레이어 상태 저장
        AdvanceStage();     // 스테이지 증가

        // 현재 씬에서 새로운 스테이지 생성
        MapManager.Instance.GenerateStage();
        
        // 모든 적 제거
        DestroyAllEnemies();
        
        // 플레이어 위치 리셋
        ResetPlayerPosition();
        
        // 새로운 적 스폰
        SpawnManager.Instance.SpawnEntities();
    }

    private void DestroyAllEnemies()
    {
        foreach (var enemy in FindObjectsOfType<MeleeEnemy>())
        {
            Destroy(enemy.gameObject);
        }
        foreach (var enemy in FindObjectsOfType<RangedEnemy>())
        {
            Destroy(enemy.gameObject);
        }
    }

    private void ResetPlayerPosition()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            // 맵의 왼쪽 시작 지점으로 플레이어 이동
            Vector3 startPosition = MapManager.Instance.GetStartPosition();
            player.transform.position = startPosition;
        }
    }
    private void Start()
    {
        
    }
}
