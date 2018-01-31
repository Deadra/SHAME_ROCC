using UnityEngine;
using UnityEngine.Networking;

public delegate void EntityDeathHandler(BaseEntity killedUnit);
public delegate void EnemyDeathHandler(GameObject enemy);

/// <summary>
/// Концентратор событий. При возникновении события, о котором ему сообщает некий объект,
/// он рассылает уведомление о событии всем желающим. 
/// </summary>
/// <remarks>
/// Пример работы: в момент смерти лягухи она вызывает SignalEnemyDeath, а поскольку
/// SceneManager подписан на событие EventEnemyDeath, он получает уведомление о смерти лягухи
/// и запускает её респаун.
/// </remarks>
public class EventHub : NetworkBehaviour
{
    public event EntityDeathHandler EventEntityDeath;
    public event EnemyDeathHandler  EventEnemyDeath;

    public void SignalEntityDeath(BaseEntity killedEntity)
    {
        if (EventEntityDeath != null && isServer)
        {
            EventEntityDeath(killedEntity);
        }
    }
    public void SignalEnemyDeath(GameObject enemy)
    {
        if (EventEnemyDeath != null && isServer)
        {
            EventEnemyDeath(enemy);
        }
    }

    private static EventHub instance = null;
    private bool IAmUseless = false;

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
    }

    public void OnDestroy()
    {
        if (!IAmUseless)
            instance = null;
    }
}