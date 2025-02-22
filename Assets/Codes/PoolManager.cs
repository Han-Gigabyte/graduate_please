using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string key;              // 풀링할 오브젝트의 키
        public GameObject prefab;       // 실제 프리팹
        public int initialSize = 10;    // 초기 풀 크기
    }

    public List<Pool> pools;  // Inspector에서 설정할 풀 목록
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary;

    public GameObject playerProjectilePrefab; // 플레이어 투사체 프리팹
    public GameObject enemyProjectilePrefab; // 적 투사체 프리팹
    public int poolSize = 10; // 풀 크기

    private List<GameObject> playerProjectilePool; // 플레이어 투사체 풀
    private List<GameObject> enemyProjectilePool; // 적 투사체 풀

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Initialize()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();

        // 각 풀에 대해 초기화
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            // 프리팹 딕셔너리에 저장
            prefabDictionary.Add(pool.key, pool.prefab);

            // 초기 오브젝트 생성
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = CreateNewObject(pool.key);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.key, objectPool);
        }

        playerProjectilePool = new List<GameObject>();
        enemyProjectilePool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject playerProjectile = Instantiate(playerProjectilePrefab);
            playerProjectile.SetActive(false); // 비활성화 상태로 풀에 추가
            playerProjectilePool.Add(playerProjectile);

            GameObject enemyProjectile = Instantiate(enemyProjectilePrefab);
            enemyProjectile.SetActive(false); // 비활성화 상태로 풀에 추가
            enemyProjectilePool.Add(enemyProjectile);
        }
    }

    private GameObject CreateNewObject(string key)
    {
        if (!prefabDictionary.ContainsKey(key))
        {
            Debug.LogError($"프리팹을 찾을 수 없습니다: {key}");
            return null;
        }

        GameObject obj = Instantiate(prefabDictionary[key]);
        obj.SetActive(false);
        obj.transform.SetParent(transform); // PoolManager의 자식으로 설정
        return obj;
    }

    public GameObject GetObject(string key)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogError($"해당 키의 풀을 찾을 수 없습니다: {key}");
            return null;
        }

        // 사용 가능한 오브젝트가 없으면 새로 생성
        if (poolDictionary[key].Count == 0)
        {
            GameObject newObj = CreateNewObject(key);
            poolDictionary[key].Enqueue(newObj);
        }

        GameObject obj = poolDictionary[key].Dequeue();
        
        // 객체가 null인지 확인
        if (obj == null)
        {
            Debug.LogError($"풀에서 가져온 객체가 null입니다: {key}");
            return null;
        }

        obj.SetActive(true);
        
        // 비활성화될 때 자동으로 풀로 반환되도록 이벤트 추가
        var returnToPool = obj.GetComponent<ReturnToPool>();
        if (returnToPool == null)
        {
            returnToPool = obj.AddComponent<ReturnToPool>();
        }
        returnToPool.Initialize(key, this);

        return obj;
    }

    public void ReturnObject(string key, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogError($"해당 키의 풀을 찾을 수 없습니다: {key}");
            return;
        }

        obj.SetActive(false);
        poolDictionary[key].Enqueue(obj);
    }

    public GameObject GetProjectile(string type)
    {
        if (type == "Player")
        {
            foreach (var projectile in playerProjectilePool)
            {
                if (projectile == null)
                {
                    Debug.LogError("Found a null projectile in the pool!");
                    continue; // null인 경우 건너뜁니다.
                }
                if (!projectile.activeInHierarchy)
                {
                    return projectile; // 비활성화된 플레이어 투사체 반환
                }
            }
        }
        else if (type == "Enemy")
        {
            foreach (var projectile in enemyProjectilePool)
            {
                if (projectile == null)
                {
                    Debug.LogError("Found a null projectile in the pool!");
                    continue; // null인 경우 건너뜁니다.
                }
                if (!projectile.activeInHierarchy)
                {
                    return projectile; // 비활성화된 적 투사체 반환
                }
            }
        }
        Debug.LogWarning($"No available projectiles of type {type} in the pool.");
        return null; // 사용할 수 있는 투사체가 없으면 null 반환
    }
}

// 오브젝트가 비활성화될 때 자동으로 풀로 반환되도록 하는 컴포넌트
public class ReturnToPool : MonoBehaviour
{
    private string poolKey;
    private PoolManager poolManager;

    public void Initialize(string key, PoolManager manager)
    {
        poolKey = key;
        poolManager = manager;
    }

    private void OnDisable()
    {
        if (poolManager != null)
        {
            poolManager.ReturnObject(poolKey, gameObject);
        }
    }
}
