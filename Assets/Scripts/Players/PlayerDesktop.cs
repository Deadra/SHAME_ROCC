using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Класс для игрока на компьютере. Обладает оружием и может его менять.
/// </summary>
public class PlayerDesktop : BasePlayer
{
    [SerializeField] private List<BaseWeapon> weapons;
    [SerializeField] private Transform weaponSlot;
    [HideInInspector] public Transform weaponSlotTransform { get {return weaponSlot.transform;} }

    private BaseWeapon currentWeapon;
    private int currentWeaponIndex = 0;

    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("CameraBase/Camera");

            gameObject.SetLayerRecursively("CameraBase/Head/HeadModel", Layer.OwnedBody);
            gameObject.SetLayerRecursively("Body", Layer.OwnedBody);
            gameObject.SetLayerRecursively("CameraBase/Canvas", Layer.OwnedUI);

            CmdSpawnGun(currentWeaponIndex);
        }
    }

    public void FireGun()
    {
        if (currentWeapon != null)
            currentWeapon.Fire();
    }

    public void SwitchGun()
    {
        currentWeaponIndex += 1;
        currentWeaponIndex = currentWeaponIndex % weapons.Count;

        CmdSpawnGun(currentWeaponIndex);
        SpawnGunSetup();
    }

    /// <summary>
    /// Спаунит пушку и отправляет информацию об этом клиенту. Выполняется на стороне сервера.
    /// </summary>
    [Command]
    void CmdSpawnGun(int weaponIndex)
    {
        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);

        currentWeaponIndex = weaponIndex;
        currentWeapon = Instantiate(weapons[currentWeaponIndex], weaponSlot);
        currentWeapon.parentNetID = this.netId;

        NetworkServer.Spawn(currentWeapon.gameObject);
        RpcSpawnGun(currentWeapon.gameObject, currentWeaponIndex);
        SpawnGunSetup();
    }

    /// <summary>
    /// Обновляет пушку в соответствии с инфой, пришедшей от сервера. Выполняется на стороне клиента.
    /// </summary>
    [ClientRpc]
    void RpcSpawnGun(GameObject weapon, int weaponIndex)
    {
        currentWeaponIndex = weaponIndex;
        currentWeapon = weapon.GetComponent<BaseWeapon>();
        SpawnGunSetup();
    }

    /// <summary>
    /// Настраивает параметры пушки, помещает её в руку игрока
    /// </summary>
    private void SpawnGunSetup()
    {
        currentWeapon.Team = Team;
        currentWeapon.Holder = this;
        currentWeapon.transform.parent = weaponSlot;
    }
}