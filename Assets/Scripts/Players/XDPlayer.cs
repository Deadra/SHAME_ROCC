using System;
using UnityEngine;

/// <summary>
/// Класс для игрока на XD Motion.
/// </summary>
/// <remarks>
/// В XDPlayer нет прямой зависимости от PlatformDataSender, однако он нужен
/// для передачи данных на Simserver
/// </remarks>
[RequireComponent(typeof(PlatformDataSender))]
public class XDPlayer : BasePlayer
{
    [Header("XDPlayer settings: ")]
    [SerializeField] private float collisionDamage = 80;
    [SerializeField] private float collisionDamageSpeed = 5;

    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("CameraParent/Camera");
            gameObject.SetLayerRecursively("CameraParent/Canvas", Layer.OwnedUI);
            gameObject.GetComponent<Collider>("BottomCollider").enabled = false;

            spawnGunsInAllSlots();
        }
    }
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {
        if (!isLocalPlayer || col.isTrigger)
            return;
        
        var entity = col.gameObject.GetComponentInParentAndChildren<BaseEntity>();

        if (entity != null)
            CauseDamage(entity);
    }

    /// <summary>
    /// Нанесение повреждений при столкновении
    /// </summary>
    void CauseDamage(BaseEntity entity)
    {
        if (entity.Team == this.Team && !Settings.friendlyFire)
            return;

        if (Math.Abs(this.gameObject.GetComponentInParent<Rigidbody>().velocity.magnitude) > collisionDamageSpeed)
            entity.TakeDamage(collisionDamage, this);
    }
}
