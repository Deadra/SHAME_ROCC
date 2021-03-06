﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Оружие, стреляющее пулями
/// </summary>
/// <remarks>
/// Чтобы пули можно было спаунить, оружию нужен SpawnManager,
/// прикреплённый к префабу игрока
/// </remarks>
public class Firearm : BaseWeapon
{
    [SerializeField] public Transform bulletSpawnPoint;
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] ParticleSystem muzzleFlashParticle;
    [SerializeField] Text ammoCounter;
    [SerializeField] AudioSource audioSource;

    public override void Start()
    {
        base.Start();
        Team = GetComponentInParent<BaseEntity>().Team;

        if (bulletSpawnPoint == null)
            Debug.LogErrorFormat("BaseWeapon: no bulletSpawnPoint at {0}", gameObject.name);

        if (ammoCounter != null)
            ammoCounter.text = string.Format("{0}", ammo);
    }

    /// <summary>
    /// Отвечает за выстрел: спаунит пулю через SpawnManager
    /// </summary>
    protected override void OnPrimaryFire()
    {
        var spawnManager = GetComponentInParent<SpawnManager>();
        spawnManager.CmdSpawnBullet(this.gameObject);
        
        if (ammoCounter != null)
            ammoCounter.text = string.Format("{0}", ammo);
    }

    private Collider[] GetAllColliders()
    {
        return transform.root.GetComponentsInChildren<Collider>();
    }

    /// <summary>
    /// Задаёт параметры пули сразу после того, как она появилась на сцене.
    /// </summary>
    /// <remarks>Эту функцию вызывает SpawnManager</remarks>
    public void SpawnBulletSetup(GameObject bullet)
    {
        if (bullet == null)
        {
            Debug.LogErrorFormat("Firearm.SpawnBulletSetup: got null object at {0}", gameObject.name);
            return;
        }

        bullet.GetComponent<BaseBullet>().Team = Team;
        bullet.GetComponent<BaseBullet>().Holder = Holder;
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = bulletSpawnPoint.rotation;
        bullet.GetComponent<Rigidbody>().velocity += GetComponent<Rigidbody>().velocity;

        foreach (Collider col in GetAllColliders())
        {
            Physics.IgnoreCollision(col, bullet.GetComponent<Collider>());
        }

        if (muzzleFlashParticle != null)
            muzzleFlashParticle.Play();

        if (audioSource != null)
            audioSource.PlayOneShot(audioSource.clip);
    }
}