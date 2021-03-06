﻿using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Базовый класс для оружия
/// </summary>
public class BaseWeapon : NetworkBehaviour
{
    [SerializeField] protected int ammo = 99;
    [SerializeField] protected float coolDown = 0.5f;
    private float nextShotTime;
    public BaseEntity Holder { get; set; }
    public TeamList Team { get; set; }

    [HideInInspector] [SyncVar] public NetworkInstanceId parentNetID;
    [HideInInspector] [SyncVar] public int slotIndex;

    /// <summary>
    /// В момент появления пушки на клиенте ищет предка по netID и становится его дитём.
    /// Это нужно, когда пушка появилась на сервере до того, как клиент подключился к игре
    /// </summary>
    public override void OnStartClient()
    {
         GameObject parentObject = ClientScene.FindLocalObject(parentNetID);

        if (parentObject && parentObject.GetComponentInChildren<BasePlayer>())
            transform.SetParent(parentObject.GetComponentInChildren<BasePlayer>().getWeaponSlot(slotIndex));

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public virtual void Start()
    {
        nextShotTime = Time.time;
    }

    public void Fire()
    {
        if (Time.time > nextShotTime && ammo > 0)
        {
            nextShotTime = Time.time + coolDown;
            ammo--;
            OnPrimaryFire();
        }
    }

    protected virtual void OnPrimaryFire()
    {
    }
}