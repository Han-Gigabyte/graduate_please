using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    
    public Tilemap targetTilemap;
    [SerializeField] private GameObject stagePortalPrefab;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject playerPrefab;
    private List<GameObject> mapPrefabs = new List<GameObject>();
    private List<GameObject> currentMapSections = new List<GameObject>();
    private List<GameObject> droppedItems = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadMapPrefabs();
        GenerateStage();
        SpawnInitialEntities();
    }

    private void LoadMapPrefabs()
    {
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/Map");
        mapPrefabs.Clear();
        mapPrefabs.AddRange(loadedPrefabs);
        
        if (mapPrefabs.Count == 0)
        {
            Debug.LogError("No map prefabs found in Resources/Prefabs/Map folder!");
        }
        else
        {
            Debug.Log($"Successfully loaded {mapPrefabs.Count} map prefabs");
        }
    }

    private float GetMapWidth(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int min = bounds.min;
        Vector3Int max = bounds.max;
        
        // 실제 타일이 있는 영역만 계산
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        
        for (int x = min.x; x < max.x; x++)
        {
            for (int y = min.y; y < max.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(pos))
                {
                    minX = Mathf.Min(minX, x);
                    maxX = Mathf.Max(maxX, x);
                }
            }
        }
        
        if (minX != int.MaxValue)
        {
            return maxX - minX + 1;
        }
        
        return bounds.size.x;
    }

    public void GenerateStage()
    {
        // 기존 몬스터와 투사체 제거
        DestroyAllEnemies();
        DestroyAllProjectiles();

        // 기존 타일맵이 존재하는지 확인
        if (targetTilemap == null)
        {
            // 타일맵이 없으면 새로 생성
            targetTilemap = new GameObject("Tilemap").AddComponent<Tilemap>();
            // TilemapRenderer 추가
            targetTilemap.gameObject.AddComponent<TilemapRenderer>();
        }
        else
        {
            // 기존 타일맵 클리어
            targetTilemap.ClearAllTiles();
        }

        // 기존 맵 섹션들 제거
        foreach (var section in currentMapSections)
        {
            if (section != null) Destroy(section);
        }
        currentMapSections.Clear();

        // 기존 Ground 오브젝트들 제거
        foreach (var ground in GameObject.FindGameObjectsWithTag("Ground"))
        {
            Destroy(ground);
        }

        // 기존 포탈 제거
        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");  // Portal 태그가 있다고 가정
        foreach (var portal in portals)
        {
            Destroy(portal);
        }

        // 기존 타일맵 클리어
        if (targetTilemap != null)
        {
            targetTilemap.ClearAllTiles();
        }

        if (mapPrefabs.Count < 3)
        {
            Debug.LogError("Not enough map prefabs!");
            return;
        }

        // 3개의 랜덤한 맵 선택 및 생성
        List<GameObject> availablePrefabs = new List<GameObject>(mapPrefabs);
        float offsetX = 0;

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availablePrefabs.Count);
            GameObject mapPrefab = availablePrefabs[randomIndex];
            
            GameObject mapSection = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
            currentMapSections.Add(mapSection);

            Tilemap sourceTilemap = mapSection.GetComponentInChildren<Tilemap>();
            if (sourceTilemap != null)
            {
                BoundsInt bounds = sourceTilemap.cellBounds;
                Vector3Int offset = new Vector3Int(Mathf.RoundToInt(offsetX), 0, 0);
                
                // 타일맵 복사
                CopyTilemapToTarget(sourceTilemap, offset);
                
                // 동일한 offset으로 Ground 생성
                SpawnGround(offset, bounds.size.x);
                
                // 다음 맵을 위한 오프셋 계산 (간격 제거)
                offsetX += bounds.size.x;  // '+2' 제거
            }
            
            Destroy(mapSection);
            availablePrefabs.RemoveAt(randomIndex);
        }

        SpawnPortal();

        // 스테이지가 새로 생성될 때마다 적 소환 (첫 스테이지 제외)
        if (Time.timeSinceLevelLoad > 1f)  // 게임 시작 직후가 아닐 때만
        {
            SpawnManager.Instance.SpawnEntities();
        }
    }

    private void SpawnGround(Vector3Int offset, int width)
    {
        if (groundPrefab == null)
        {
            Debug.LogError("Ground Prefab not assigned!");
            return;
        }

        // 타일맵의 바닥 위치 계산
        BoundsInt targetBounds = targetTilemap.cellBounds;
        int bottomY = targetBounds.min.y;  // 타일맵의 가장 아래 Y 좌표

        // Ground의 위치 계산 (타일맵의 바닥을 따라)
        Vector3 groundPosition = new Vector3(
            offset.x - 5f,           // 왼쪽으로 5칸 이동
            bottomY + 0.5f,          // 위로 0.5 이동
            0
        );

        GameObject ground = Instantiate(groundPrefab, groundPosition, Quaternion.identity);
        
        // Ground의 pivot이 중앙에 있으므로, 위치를 왼쪽으로 조정
        ground.transform.position = new Vector3(
            groundPosition.x + (width / 2f),  // 너비의 절반만큼 오른쪽으로 이동
            groundPosition.y,
            0
        );

        // Ground의 크기 설정
        Vector3 scale = ground.transform.localScale;
        scale.x = width;    // 타일맵 너비만큼
        scale.y = 1;        // 높이는 1로 고정
        ground.transform.localScale = scale;

        Debug.Log($"Created ground at position: {groundPosition}, width: {width}, offset: {offset}, bottomY: {bottomY}");
    }

    private void CopyTilemapToTarget(Tilemap sourceTilemap, Vector3Int offset)
    {
        if (sourceTilemap == null)
        {
            Debug.LogWarning("Source Tilemap is null. Cannot copy tiles.");
            return; // Tilemap이 null인 경우 메서드 종료
        }

        BoundsInt bounds = sourceTilemap.cellBounds;

        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            TileBase tile = sourceTilemap.GetTile(position);
            if (tile != null)
            {
                Vector3Int targetPosition = position + offset;
                targetTilemap.SetTile(targetPosition, tile);
            }
        }
    }

    private void SpawnPortal()
    {
        if (stagePortalPrefab == null)
        {
            Debug.LogError("Stage Portal Prefab not assigned!");
            return;
        }

        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        if (grounds.Length > 0)
        {
            GameObject lastGround = grounds[grounds.Length - 1];
            
            // 마지막 Ground의 오른쪽 끝에서 약간 왼쪽으로 이동한 위치에 포탈 생성
            float groundRight = lastGround.transform.position.x + (lastGround.transform.localScale.x / 2f);
            float groundY = lastGround.transform.position.y;
            
            Vector3 portalPosition = new Vector3(
                groundRight - 2f,  // 오른쪽 끝에서 2칸 왼쪽
                groundY + 1.5f,    // Ground 위로 1.5칸
                0
            );
            
            Instantiate(stagePortalPrefab, portalPosition, Quaternion.identity);
            Debug.Log("Portal spawned at: " + portalPosition);
        }
    }

    public Vector3 GetStartPosition()
    {
        if (currentMapSections.Count > 0)
        {
            GameObject firstSection = currentMapSections[0];
            return new Vector3(
                firstSection.transform.position.x + 2f,
                firstSection.transform.position.y + 1f,
                0
            );
        }
        return Vector3.zero;
    }

    private void SpawnInitialEntities()
    {
        // 플레이어 소환
        if (playerPrefab != null)
        {
            Vector3 playerSpawnPosition = new Vector3(2f, 2f, 0f);
            GameObject player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            
            // 메인 카메라 찾기
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.target = player.transform;
                    Debug.Log("Camera target set to player");
                }
                else
                {
                    Debug.LogError("CameraFollow component not found on main camera!");
                }
            }
            else
            {
                Debug.LogError("Main camera not found!");
            }
        }
        else
        {
            Debug.LogError("Player Prefab not assigned!");
        }

        // SpawnManager를 통해 적 소환
        SpawnManager.Instance.SpawnEntities();
    }

    public void DestroyAllEnemies()
    {
        // 모든 몬스터 제거
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (Enemies.Length > 0)
        {
            foreach (var enemy in Enemies)
            {
                Destroy(enemy);
            }
        }
    }

    public void DestroyAllProjectiles()
    {
        // 모든 원거리 몬스터의 투사체 제거
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        if (projectiles.Length > 0)
        {
            foreach (var projectile in projectiles)
            {
                Destroy(projectile);
            }
        }
    }
}
