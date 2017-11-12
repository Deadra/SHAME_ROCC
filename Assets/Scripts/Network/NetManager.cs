using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Сообщение, содержащее информацию о том, какой префаб выбрал клиент
/// </summary>
public class NetworkMessage : MessageBase
{
    public short prefabIndex;
}

/// <summary>
/// Основной класс для создания сетевой игры.
/// </summary>
public class NetManager : NetworkManager
{
    [Header("Player Prefab Settings")]
    public GameObject XDMotionPrefab;
    public GameObject FlyMotionPrefab;
    public GameObject FiveDMotionPrefab;
    public GameObject DesktopPrefab;
    public GameObject HTCVivePrefab;

    [Header("Player Spawn Points")]
    public NetworkStartPosition XDMotionSpawnPoint;
    public NetworkStartPosition FlyMotionSpawnPoint;
    public NetworkStartPosition FiveDMotionSpawnPoint;
    public NetworkStartPosition DesktopSpawnPoint;
    public NetworkStartPosition HTCViveSpawnPoint;

    [Header("Other Prefabs")]
    public List<GameObject> enemyPrefabs;
    public List<GameObject> otherPrefabs;

    private static NetworkManager instance = null;
    private bool isServer;
    private List<NetworkStartPosition> spawnPoints;

    public List<GameObject> spawnedPlayers { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarningFormat("Multiple instances of {0} (singleton) on the scene (objects {1}, {2})! Exterminate!!!1",
                                    this.GetType(), instance.gameObject.name, gameObject.name);
            Destroy(this);
        }
    }

    private void Start()
    {
        spawnPrefabs.Insert((int)PlatformType.XDMotion, XDMotionPrefab);
        spawnPrefabs.Insert((int)PlatformType.FlyMotion, FlyMotionPrefab);
        spawnPrefabs.Insert((int)PlatformType.FiveDMotion, FiveDMotionPrefab);
        spawnPrefabs.Insert((int)PlatformType.Desktop, DesktopPrefab);
        spawnPrefabs.Insert((int)PlatformType.HTCVive, HTCVivePrefab);

        spawnPoints = new List<NetworkStartPosition>();
        spawnPoints.Add(XDMotionSpawnPoint);
        spawnPoints.Add(FlyMotionSpawnPoint);
        spawnPoints.Add(FiveDMotionSpawnPoint);
        spawnPoints.Add(DesktopSpawnPoint);
        spawnPoints.Add(HTCViveSpawnPoint);

        spawnPrefabs.AddRange(enemyPrefabs);
        spawnPrefabs.AddRange(otherPrefabs);

        spawnedPlayers = new List<GameObject>();

        if (Settings.gameMode == GameMode.Online)
            StartCoroutine(JointGame());
        else
            StartOfflineGame();

        Debug.Log("Game Mode: " + Settings.gameMode.ToString() + ", Platform Type: " + Settings.platformType.ToString());

        if (!Application.isEditor)
            Cursor.visible = false;
    }

    /// <summary>
    /// Обязательно перед закрытием программы делаем полный Disconnect, иначе будут ошибки.
    /// </summary>
    private void OnApplicationQuit()
    {
        try
        {
            Debug.Log("Закрываем приложение");
            StopHost();
            StopClient();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    /// <summary>
    /// Запускаем хост.
    /// </summary>
    private void StartupHost()
    {
        Debug.Log("Пытаемся создать сервер");
        NetworkClient.ShutdownAll();

        StartHost();
    }

    /// <summary>
    /// Подключаемся к серверу.
    /// </summary>
    /// <returns></returns>
    private IEnumerator JointGame()
    {
        string[] address = Settings.networkServersIP.ToArray();

        for (int i = 0; i < address.Length; i++)
        {
            if (address[i] == "0.0.0.0") continue;

            Debug.LogFormat("Подключаемся к серверу {0}", address[i]);

            networkAddress = address[i];

            StartClient();

            yield return new WaitForSeconds(0.2f);

            if (!IsClientConnected())
            {
                NetworkClient.ShutdownAll();
                continue;
            }
            else
            {
                Debug.LogFormat("Удалось подключится к серверу {0}", address[i]);

                yield break;
            }
        }

        StartupHost();
    }

    private void Disconnect()
    {
        if (!IsClientConnected())
        {
            Debug.Log("Вы сервер. Отключаемся.");
            NetworkManager.singleton.StopHost();
        }
        else
        {
            Debug.Log("Вы клиент. Отключаемся.");
            NetworkManager.singleton.StopClient();
        }
    }

    /// <summary>
    /// Вызывается на стороне сервера, когда клиент подключается к нему
    /// с помощью ClientScene.AddPlayer
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    /// <param name="extraMessageReader"></param>
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage>();
        int selectedClass = message.prefabIndex;
        Debug.Log("Подключился клиент " + (PlatformType)selectedClass);

        GameObject player = Instantiate(spawnPrefabs[selectedClass],
                                        spawnPoints[selectedClass].transform.position,
                                        spawnPoints[selectedClass].transform.rotation) as GameObject;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        spawnedPlayers.Add(player);
    }

    /// <summary>
    /// Вызывается на стороне клиента, когда он подключается к серверу.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        NetworkMessage msg = new NetworkMessage();
        msg.prefabIndex = (short)Settings.platformType;

        ClientScene.AddPlayer(conn, 0, msg);
    }

    /// <summary>
    /// Запускает оффлайн игру, т.е. просто спаунит нужный префаб
    /// на сцену без установки сетевого соединения
    /// </summary>
    private void StartOfflineGame()
    {
        int platformType = (int)Settings.platformType;
        var player = Instantiate(spawnPrefabs[platformType],
                                 spawnPoints[platformType].transform.position,
                                 spawnPoints[platformType].transform.rotation);
        spawnedPlayers.Add(player);
    }
}