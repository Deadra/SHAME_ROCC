using UnityEngine;

/// <summary>
/// Обычная пуля, способная наносить повреждения BaseEntity
/// </summary>
public class StandartBullet : BaseBullet
{
    [SerializeField] private float damage;
    [SerializeField] private ParticleSystem hitParticle;

    /// <summary>
    /// Обработчик столкновения
    /// </summary>
    protected override void OnHit(RaycastHit hitInfo)
    {
        BaseEntity entityHit = hitInfo.collider.gameObject.GetComponentInParent<BaseEntity>();

        if (entityHit != null && entityHit.Team != Team)
            entityHit.TakeDamage(damage);

        Instantiate(hitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
    }
}