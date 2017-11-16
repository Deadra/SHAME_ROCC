using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDesktop : BasePlayer
{
    [SerializeField] private List<BaseWeapon> weapons;
    [SerializeField] private Transform weaponSlot;
    [HideInInspector] public Transform weaponSlotTransform { get {return weaponSlot.transform;} }
    //private DesktopUIManager uiManager;

    private BaseWeapon currentWeapon;
    private int currentWeaponIndex = 0;

    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            SetLayerRecursively(transform.Find("CameraBase/Head/HeadModel").gameObject, Layer.OwnedBody);
            SetLayerRecursively(transform.Find("Body").gameObject, Layer.OwnedBody);
            SetLayerRecursively(transform.Find("CameraBase/Canvas").gameObject, Layer.OwnedUI);
            CmdSpawnGun(currentWeaponIndex);
        }
    }

    public void FireGun()
    {
        if (currentWeapon != null)
            currentWeapon.Fire();
    }

    protected override void OnDamageTaken(float value)
    {
        //uiManager.SetShownHealth(currentHealth);
    }

    public void SwitchGun()
    {
        currentWeaponIndex += 1;
        currentWeaponIndex = currentWeaponIndex % weapons.Count;

        CmdSpawnGun(currentWeaponIndex);
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
        SpawnGun();
    }

    /// <summary>
    /// Обновляет пушку в соответствии с инфой, пришедшей от сервера. Выполняется на стороне клиента.
    /// </summary>
    [ClientRpc]
    void RpcSpawnGun(GameObject weapon, int weaponIndex)
    {
        currentWeaponIndex = weaponIndex;
        currentWeapon = weapon.GetComponent<BaseWeapon>();
        SpawnGun();
    }

    /// <summary>
    /// Настраивает параметры пушки, помещает её в руку игрока
    /// </summary>
    private void SpawnGun()
    {
        currentWeapon.Team = Team;
        currentWeapon.Holder = this;
        currentWeapon.transform.parent = weaponSlot;
    }
}