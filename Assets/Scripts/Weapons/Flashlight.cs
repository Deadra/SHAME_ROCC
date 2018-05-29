using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Фонарик
/// </summary>
public class Flashlight : BaseWeapon {

    [SerializeField] Light lightSource;
    private bool switchedOn = true;

    /// <summary>
    /// Включает или выключает фонарь через SpawnManager
    /// </summary>
    protected override void OnPrimaryFire()
    {
        ammo = 99;
        switchedOn = !switchedOn;
        var spawnManager = transform.root.GetComponentInChildren<SpawnManager>();
        spawnManager.CmdEnableLight(this.gameObject, switchedOn);
    }
    /// <summary>
    /// Включает или выключает фонарь
    /// </summary>
    /// <remarks>Эту функцию вызывает SpawnManager</remarks>
    public void EnableLight(bool enabled)
    {
        lightSource.enabled = enabled;
    }
}
