using UnityEngine;

public class StandartBullet : BaseBullet
{
    [SerializeField] private float damage;
    [SerializeField] private ParticleSystem hitParticle;

    protected override void OnHit(RaycastHit hitInfo)
    {
        BaseEntity entityHit = hitInfo.collider.gameObject.GetComponentInParent<BaseEntity>();

        if (entityHit != null && entityHit.Team != Team)
            entityHit.TakeDamage(damage);

        Instantiate(hitParticle, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
    }
}