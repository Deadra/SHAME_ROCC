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
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("Camera");
            gameObject.SetLayerRecursively("Camera/Canvas", Layer.OwnedUI);
            gameObject.GetComponent<Collider>("BottomCollider").enabled = false;

            spawnGunsInAllSlots();
        }
    }
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {
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

        float relativeVelocity = System.Math.Abs(Vector3.Dot(entity.gameObject.GetComponentInParent<Rigidbody>().velocity,
                                                             this.gameObject.GetComponentInParent<Rigidbody>().velocity));
        if (relativeVelocity > 5)
            entity.TakeDamage(relativeVelocity * 20, this);
    }
}
