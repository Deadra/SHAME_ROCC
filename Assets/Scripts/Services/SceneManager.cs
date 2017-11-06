using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SceneManager : NetworkBehaviour
{
    private static SceneManager instance = null;
    private bool IAmUseless = false;

    [SerializeField] NetManager netManager;
    [SerializeField] float respawnTime = 10f;
    [Header("Enemies Spawn Points")]
    List<GameObject> enemyPrefabs;
    [SerializeField] List<Transform> spawnPoints;

    List<GameObject> spawnedEnemies;

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

        spawnedEnemies = new List<GameObject>();
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
        foreach (var spawnPoint in spawnPoints)
            foreach (var enemyPrefab in enemyPrefabs)
            {
                var enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                enemy.GetComponent<FrogAI>().patrolingCenter = GameObject.FindWithTag("FrogPatrolCenter").transform;
                NetworkServer.Spawn(enemy);
                spawnedEnemies.Add(enemy);
            }

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
            spawnedEnemies.RemoveAll((GameObject enemy) => { return enemy == null; });

            if (spawnedEnemies.Count == 0)
                CmdSpawnEnemies();
        }
    }
}