using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Класс, занимающийся спауном объектов. Должен висеть на корневом элементе префаба игрока.
/// </summary>
/// <remarks> 
/// Command функции могут вызываться только клиентом, т.е. скриптом, висящем на корневом элементе префаба, 
/// содержащего NetworkIdentity с галкой Local Player Authority (обычно это просто префаб игрока).
/// Этот скрипт вешается на такой префаб и позволяет другим скриптам спаунить различным объекты.
/// Например, оружие не может само спаунить пули, потому что оно не является игроком, но зато
/// оно может обратиться к SpawnManager, чтобы тот заспаунил пулю.
/// </remarks>
public class SpawnManager : NetworkBehaviour
{
    void Awake()
    {
        if (transform.root != transform ||
            !gameObject.GetComponent<NetworkIdentity>() ||
            !gameObject.GetComponent<NetworkIdentity>().localPlayerAuthority)
            Debug.LogWarningFormat("The {0} class on the {1} object must belong to the client, otherwise Command functions will not be called!\n" +
                                   "See {0} documentation", this.GetType(), gameObject.name);
    }

    #region SpawnBullet
    /// <summary>
    /// Спаун пули из оружия типа Firearm
    /// </summary>
    [Command]
    public void CmdSpawnBullet(GameObject caller)
    {
        var weapon = caller.GetComponent<Firearm>();
        GameObject bullet = Instantiate(weapon.bulletPrefab, weapon.bulletSpawnPoint.position, weapon.bulletSpawnPoint.rotation);

        weapon.SpawnBulletSetup(bullet);
        NetworkServer.Spawn(bullet);
        RpcSpawnBullet(caller, bullet);
    }

    /// <summary>
    /// Обновляет скорость и направление пули на клиенте
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="bullet"></param>
    [ClientRpc]
    private void RpcSpawnBullet(GameObject caller, GameObject bullet)
    {
        var weapon = caller.GetComponent<Firearm>();

        weapon.SpawnBulletSetup(bullet);
        bullet.transform.position = weapon.bulletSpawnPoint.position;
        bullet.transform.rotation = weapon.bulletSpawnPoint.rotation;
    }

    #endregion SpawnBullet

    #region EnableLight
    /// <summary>
    /// Включение фонарика на сервере
    /// </summary>
    [Command]
    public void CmdEnableLight(GameObject caller, bool enabled)
    {
        var weapon = caller.GetComponent<Flashlight>();
        weapon.EnableLight(enabled);
        RpcEnableLight(caller, enabled);
    }

    /// <summary>
    /// Включение фонарика на клиенте
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="bullet"></param>
    [ClientRpc]
    private void RpcEnableLight(GameObject caller, bool enabled)
    {
        var weapon = caller.GetComponent<Flashlight>();
        weapon.EnableLight(enabled);
    }

    #endregion EnableLight
}
