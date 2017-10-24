﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Firearm : BaseWeapon
{
    [SerializeField] public Transform bulletSpawnPoint;
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] ParticleSystem muzzleFlashParticle;
    [SerializeField] Text ammoCounter;

    public override void Start()
    {
        base.Start();
        Team = this.transform.root.GetComponentInChildren<BaseEntity>().Team;
        if (ammoCounter != null)
            ammoCounter.text = string.Format("{0}", ammo);
    }

    protected override void OnPrimaryFire()
    {
        var spawnManager = transform.root.GetComponentInChildren<SpawnManager>();
        spawnManager.CmdSpawnBullet(this.gameObject);
        
        if (ammoCounter != null)
            ammoCounter.text = string.Format("{0}", ammo);
    }

    private Collider[] GetAllColliders()
    {
        return transform.root.GetComponentsInChildren<Collider>();
    }

    public void SpawnBulletSetup(GameObject bullet)
    {
        bullet.GetComponent<BaseBullet>().Team = Team;
        bullet.GetComponent<BaseBullet>().Holder = Holder;
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = bulletSpawnPoint.rotation;
        bullet.GetComponent<Rigidbody>().velocity += GetComponent<Rigidbody>().velocity;

        foreach (Collider col in GetAllColliders())
        {
            Physics.IgnoreCollision(col, bullet.GetComponent<Collider>());
        }

        muzzleFlashParticle.Play();
    }
}