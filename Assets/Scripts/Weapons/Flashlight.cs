using UnityEngine;
using UnityEngine.Networking;

public class Flashlight : BaseWeapon {

    [SerializeField] Light lightSource;
    private bool switchedOn = true;

    protected override void OnPrimaryFire()
    {
        ammo = 99;
        switchedOn = !switchedOn;
        var spawnManager = transform.root.GetComponentInChildren<SpawnManager>();
        spawnManager.CmdEnableLight(this.gameObject, switchedOn);
    }
    
    public void EnableLight(bool enabled)
    {
        lightSource.enabled = enabled;
    }
}
