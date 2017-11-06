using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SceneManager : NetworkBehaviour
{
    private static SceneManager instance = null;
    private bool IAmUseless = false;

    [SerializeField] NetManager netManager;
    [SerializeField] EventHub EHub;
    [SerializeField] float respawnTime = 10f;
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

    [Command]
    void CmdRespawnEnemies()
    {
        foreach (var spawnPoint in pointsToRespawn)
            spawnRandomEnemy(spawnPoint);

        pointsToRespawn.Clear();
    }

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
        if (!IAmUseless)
            instance = null;
    }

    private IEnumerator RespawnEnemies()
    {
        while (true)
        { 
            yield return new WaitForSeconds(respawnTime);
            CmdRespawnEnemies();
        }
    }

    void EntityDeathDetected(GameObject died)
    {
        if (spawnedEnemies.ContainsKey(died))
        {
            pointsToRespawn.Add(spawnedEnemies[died]);
            spawnedEnemies.Remove(died);
        }
    }
}