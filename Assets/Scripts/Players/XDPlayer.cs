using UnityEngine;

/// <summary>
/// Класс для игрока на XD Motion.
/// </summary>
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
        var entity = col.gameObject.GetComponentInRootAndChildren<BaseEntity>();

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

        float relativeVelocity = System.Math.Abs(Vector3.Dot(entity.gameObject.GetComponentInRoot<Rigidbody>().velocity,
                                                             this.gameObject.GetComponentInRoot<Rigidbody>().velocity));
        if (relativeVelocity > 5)
            entity.TakeDamage(relativeVelocity * 20, this);
    }
}
