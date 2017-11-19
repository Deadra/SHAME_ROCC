using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Менеджер сцены. Спаунит врагов и следит за тем, чтобы они респаунились через некоторое время
/// </summary>
public class SceneManager : NetworkBehaviour
{
    private static SceneManager instance = null;
    private bool IAmUseless = false;

    [SerializeField] NetManager netManager;
    [SerializeField] EventHub EHub;
    [SerializeField] float respawnTime = 20f;
    [SerializeField] float spawnRadius = 2f;
    [SerializeField] float dontSpawnRadius = 20f;

    [System.Serializable]
    class pair
    {
        public Transform spawnPoint = null;
        public int enemyCount = 0;
    }
    [SerializeField]
    List<pair> spawnPoints;

   // [Header("Enemies Spawn Points")]
    
    //[SerializeField] List<Transform> spawnPoints;
    //[SerializeField] List<int>       enemyCount;

    private Dictionary<GameObject, Transform> spawnedEnemies;
    private List<GameObject> enemyPrefabs;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarningFormat("Multiple instances of {0} (singleton) on the scene (objects {1}, {2})! Exterminate!!!1",
                                    this.GetType(), instance.gameObject.name, gameObject.name);
            IAmUseless = true;
            Destroy(this);
        }
        
        enemyPrefabs = new List<GameObject>();
        enemyPrefabs.AddRange(netManager.enemyPrefabs);

        spawnedEnemies  = new Dictionary<GameObject, Transform>();

        EHub.EventEnemyDeath += new EnemyDeathHandler(EntityDeathDetected);
    }

    void Start()
    {
        if (isServer)
            CmdSpawnEnemies();
    }

    /// <summary>
    /// Спаунит врагов в указанных точках в указанном количестве 
    /// </summary>
    [Command]
    void CmdSpawnEnemies()
    {
        for (var i = 0; i < spawnPoints.Count; i++)
            for (var j = 0; j < spawnPoints[i].enemyCount; j++)
                spawnRandomEnemy(spawnPoints[i].spawnPoint);
    }

    /// <summary>
    /// Спаунит случайного врага в круге с центром в spawnPoint и радиусом spawnRadius
    /// </summary>
    void spawnRandomEnemy(Transform spawnPoint)
    {
        var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        var enemy = Instantiate(enemyPrefab, Helpers.GetPointInCircle(spawnPoint.position, spawnRadius), spawnPoint.rotation);

        enemy.GetComponent<FrogAI>().patrolingCenter = spawnPoint;
        NetworkServer.Spawn(enemy);
        spawnedEnemies.Add(enemy, spawnPoint);
    }

    public void OnDestroy()
    {
        StopAllCoroutines();

        if (!IAmUseless)
            instance = null;
    }
    
    /// <summary>
    /// Корутина, спаунящая врага в конкретной точке
    /// </summary>
    private IEnumerator RespawnEnemy(Transform spawnPoint)
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);

            if (!Helpers.IsPlayerInTheArea(spawnPoint.position, dontSpawnRadius))
            {
                CmdRespawnEnemy(spawnPoint.gameObject);
                yield break;
            }
        }
    }

    /// <summary>
    /// Спаунит врага в указанной точке
    /// </summary>
    [Command]
    void CmdRespawnEnemy(GameObject spawnPoint)
    {
        spawnRandomEnemy(spawnPoint.transform);
    }

    /// <summary>
    /// Обработчик, респаунящий врага при смерти
    /// </summary>
    void EntityDeathDetected(GameObject died)
    {
        if (spawnedEnemies.ContainsKey(died))
        {
            var spawnPoint = spawnedEnemies[died];
            spawnedEnemies.Remove(died);

            IEnumerator courutine = RespawnEnemy(spawnPoint);
            StartCoroutine(courutine);
        }
    }
}