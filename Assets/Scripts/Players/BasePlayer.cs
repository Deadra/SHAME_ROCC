using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Основной класс для игровых персонажей. В момент сметри возрождается на стартовой позиции,
/// обладает интерфейсом: миникартой и полосой здоровья. Обладает оружием и может его менять.
/// </summary>
[RequireComponent(typeof(SpawnManager))]
public class BasePlayer : BaseEntity
{
    [SerializeField] private List<BaseWeapon> weapons;
    private int currentWeaponIndex = 0;
    [SerializeField] private List<Transform> weaponSlots;
    private Dictionary<Transform, BaseWeapon> currentWeapons;

    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject minimapCamera;
    protected Vector3 startingPosition;
    protected Quaternion startingRotation;

    public override void Start()
    {
        base.Start();

        currentWeapons = new Dictionary<Transform, BaseWeapon>();
        foreach (var slot in weaponSlots)
            currentWeapons.Add(slot, null);

        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    protected override void Update()
    {
        base.Update();

        if (!isLocalPlayer)
            return;

        healthBar.value = currentHealth / maxHealth;

        if (minimapCamera)
        {
            Vector3 pos = transform.position;
            pos.y += 500;
            minimapCamera.transform.position = pos;
            minimapCamera.transform.rotation = Quaternion.LookRotation(Vector3.down, transform.forward);
        }
    }

    [ClientRpc]
    protected override void RpcOnDeath()
    {
        currentHealth = maxHealth;
        transform.position = startingPosition;
        transform.rotation = startingRotation;
    }

    public void FireGun()
    {
        foreach(var currentWeapon in currentWeapons)
            currentWeapon.Value.Fire();
    }

    public void SwitchGun()
    {
        currentWeaponIndex += 1;
        currentWeaponIndex = currentWeaponIndex % weapons.Count;

        spawnGunsInAllSlots(currentWeaponIndex);
    }

    /// <summary>
    /// Спаунит пушку сразу во всех слотах
    /// </summary>
    /// <param name="weaponIndex"></param>
    public void spawnGunsInAllSlots(int weaponIndex = 0)
    {
        if (weaponIndex >= weapons.Count)
            return;

        for (int i = 0; i < weaponSlots.Count; i++)
            CmdSpawnGun(weaponIndex, i);
    }

    /// <summary>
    /// Спаунит пушку в указанном слоте и отправляет информацию об этом клиенту. Выполняется на стороне сервера.
    /// </summary>
    [Command]
    void CmdSpawnGun(int weaponIndex, int slotIndex)
    {
        var slot = weaponSlots[slotIndex];
        if(currentWeapons[slot] != null)
            Destroy(currentWeapons[slot].gameObject);
        currentWeapons[slot] = null;

        currentWeaponIndex = weaponIndex;
        var weapon = Instantiate(weapons[currentWeaponIndex], slot);
        weapon.parentNetID = this.netId;
        weapon.slotIndex = slotIndex;

        NetworkServer.Spawn(weapon.gameObject);
        RpcSpawnGun(weapon.gameObject, weaponIndex, slotIndex);
        SpawnGunSetup(weapon);
    }

    /// <summary>
    /// Обновляет пушку в соответствии с инфой, пришедшей от сервера. Выполняется на стороне клиента.
    /// </summary>
    [ClientRpc]
    void RpcSpawnGun(GameObject weapon, int weaponIndex, int slotIndex)
    {
        currentWeaponIndex = weaponIndex;
        var slot = weaponSlots[slotIndex];
        currentWeapons[slot] = weapon.GetComponent<BaseWeapon>();
        SpawnGunSetup(weapon.GetComponent<BaseWeapon>());
    }

    /// <summary>
    /// Настраивает параметры пушки, помещает её в руку игрока
    /// </summary>
    private void SpawnGunSetup(BaseWeapon weapon)
    {
        weapon.Team = Team;
        weapon.Holder = this;
        weapon.transform.parent = weaponSlots[weapon.slotIndex];
    }

    public Transform getWeaponSlot(int index)
    {
        return weaponSlots[index];
    }
}
