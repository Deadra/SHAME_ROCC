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
    [Header("Enemies Spawn Points")]
    
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<int>       enemyCount;

    private Dictionary<GameObject, Transform> spawnedEnemies;
    private List<Transform> pointsToRespawn;
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

        if (spawnPoints.Count != enemyCount.Count)
            Debug.LogErrorFormat("{0} at {1} object: Spawn Points size must match Enemy Count size", this.GetType(), gameObject.name);

        enemyPrefabs = new List<GameObject>();
        enemyPrefabs.AddRange(netManager.enemyPrefabs);

        spawnedEnemies  = new Dictionary<GameObject, Transform>();
        pointsToRespawn = new List<Transform>();

        EHub.EventEnemyDeath += new EnemyDeathHandler(EntityDeathDetected);
    }

    void Start()
    {
        if (isServer)
            CmdSpawnEnemies();

        if (isServer)
            StartCoroutine(RespawnEnemies());
    }

    /// <summary>
    /// Спаунит врагов в указанных точках в указанном количестве 
    /// </summary>
    [Command]
    void CmdSpawnEnemies()
    {
        for (var i = 0; i < spawnPoints.Count; i++)
        {
            var spawnPoint = spawnPoints[i];

            for (var j = 0; j < enemyCount[i]; j++)
                spawnRandomEnemy(spawnPoint);
        }
    }

    /// <summary>
    /// Спаунит столько врагов, сколько их было недавно убито
    /// </summary>
    [Command]
    void CmdRespawnEnemies()
    {
        foreach (var spawnPoint in pointsToRespawn)
            spawnRandomEnemy(spawnPoint);

        pointsToRespawn.Clear();
    }

    /// <summary>
    /// Спаунит случайного врага в круге с центром в spawnPoint и радиусом spawnRadius
    /// </summary>
    void spawnRandomEnemy(Transform spawnPoint)
    {
        var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        var enemy = Instantiate(enemyPrefab, Helpers.GetPointInCircle(spawnPoint.position, spawnRadius), spawnPoint.rotation);

        enemy.GetComponent<FrogAI>().patrolingCenter = GameObject.FindWithTag("FrogPatrolCenter").transform;
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
    /// Корутина, спаунящая столько врагов, сколько было недавно убито
    /// </summary>
    private IEnumerator RespawnEnemies()
    {
        while (true)
        { 
            yield return new WaitForSeconds(respawnTime);
            CmdRespawnEnemies();
        }
    }

    /// <summary>
    /// Обработчик, перемещающий точку спауна в список точек, где надо респаунить врага
    /// </summary>
    void EntityDeathDetected(GameObject died)
    {
        if (spawnedEnemies.ContainsKey(died))
        {
            pointsToRespawn.Add(spawnedEnemies[died]);
            spawnedEnemies.Remove(died);
        }
    }
}