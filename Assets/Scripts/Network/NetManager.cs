using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

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
/// <details>
/// Позволяет указывать различные префабы игроков для различных платформ и целей.
/// Запускает игру в режиме онлайн или оффлайн, может подключаться к уже запущенной игре.
/// </details>
public class NetManager : NetworkManager
{
    [Header("Player Prefab Settings")]
    public GameObject XDMotionPrefab;
    public GameObject FlyMotionPrefab;
    public GameObject FiveDMotionPrefab;
    public GameObject DesktopPrefab;
    public GameObject HTCVivePrefab;
    public GameObject SpectatorPrefab;

    [Header("Player Spawn Points")]
    public NetworkStartPosition XDMotionSpawnPoint;
    public NetworkStartPosition FlyMotionSpawnPoint;
    public NetworkStartPosition FiveDMotionSpawnPoint;
    public NetworkStartPosition DesktopSpawnPoint;
    public NetworkStartPosition HTCViveSpawnPoint;
    public NetworkStartPosition SpectatorSpawnPoint;

    [Header("Other Prefabs")]
    public List<GameObject> enemyPrefabs;
    public List<GameObject> otherPrefabs;

    private static NetworkManager instance = null;
    private bool isServer;
    private List<NetworkStartPosition> spawnPoints;
    public short NetworkMessageID { get; private set; }
    public short SettingsMessageID { get; private set; } 

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

        NetworkMessageID = MsgType.Highest + 1;
        SettingsMessageID  = MsgType.Highest + 2;
    }

    private void Start()
    {   
        spawnPrefabs.Insert((int)PlatformType.XDMotion,     XDMotionPrefab);
        spawnPrefabs.Insert((int)PlatformType.FlyMotion,    FlyMotionPrefab);
        spawnPrefabs.Insert((int)PlatformType.FiveDMotion,  FiveDMotionPrefab);
        spawnPrefabs.Insert((int)PlatformType.Desktop,      DesktopPrefab);
        spawnPrefabs.Insert((int)PlatformType.HTCVive,      HTCVivePrefab);
        spawnPrefabs.Insert((int)PlatformType.Spectator,    SpectatorPrefab);

        spawnPoints = new List<NetworkStartPosition>();
        spawnPoints.Add(XDMotionSpawnPoint);
        spawnPoints.Add(FlyMotionSpawnPoint);
        spawnPoints.Add(FiveDMotionSpawnPoint);
        spawnPoints.Add(DesktopSpawnPoint);
        spawnPoints.Add(HTCViveSpawnPoint);
        spawnPoints.Add(SpectatorSpawnPoint);

        spawnPrefabs.AddRange(enemyPrefabs);
        spawnPrefabs.AddRange(otherPrefabs);

        spawnedPlayers = new List<GameObject>();

        if (Settings.gameMode == GameMode.Online)
            StartCoroutine(JoinGame());
        else
            StartOfflineGame();

        Debug.Log("Game Mode: " + Settings.gameMode.ToString() + ", Platform Type: " + Settings.platformType.ToString());

        if (!Application.isEditor)
            Cursor.visible = false;
    }

    /// <summary>
    /// Закрывает все сетевые соединения при закрытии программы
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
    /// Запускает хост, т.е. становится сервером игры.
    /// </summary>
    private void StartupHost()
    {
        Debug.Log("Пытаемся создать сервер");
        NetworkClient.ShutdownAll();
        StartHost();

        if (!NetworkServer.active)
        {
            Debug.LogError("Не удаётся запустить игру. Возможно, экземпляр игры уже запущен");
            Application.Quit();
        }
    }

    /// <summary>
    /// Подключается к серверу
    /// </summary>
    private IEnumerator JoinGame()
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
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage>();
        int selectedPrefab = message.prefabIndex;
        Debug.Log("Подключился клиент " + (PlatformType)selectedPrefab);

        GameObject player = Instantiate(spawnPrefabs[selectedPrefab],
                                        spawnPoints[selectedPrefab].transform.position,
                                        spawnPoints[selectedPrefab].transform.rotation) as GameObject;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        spawnedPlayers.Add(player);

        SettingsMessage settingsMessage = Settings.GetSettingsMessage();
        NetworkServer.SendToClient(conn.connectionId, SettingsMessageID, settingsMessage);
    }

    /// <summary>
    /// Вызывается на стороне клиента, когда он подключается к серверу.
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        NetworkMessage msg = new NetworkMessage();
        msg.prefabIndex = (short)Settings.platformType;
        client.RegisterHandler(SettingsMessageID, OnSettingsMessage);

        ClientScene.AddPlayer(conn, 0, msg);
    }

    /// <summary>
    /// Запускает оффлайн игру
    /// </summary>
    private void StartOfflineGame()
    {
        StartupHost();
        this.maxConnections = 1;
    }

    /// <summary>
    /// Обработчик сообщения с настройками
    /// </summary>
    /// <param name="netMsg"></param>
    private void OnSettingsMessage(UnityEngine.Networking.NetworkMessage netMsg)
    {
        SettingsMessage msg = netMsg.ReadMessage<SettingsMessage>();
        Settings.UpdateClientSettings(msg);
    }

    public List<GameObject> GetConnectedPlayers()
    {
        return spawnedPlayers.Where((x) => x != null).ToList();
    }
}