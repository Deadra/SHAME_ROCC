using UnityEngine;

public class StandartBullet : BaseBullet
{
    [SerializeField] private float damage;
    [SerializeField] private ParticleSystem hitParticle;

    protected override void SetOff(Collision col)
    {
        BaseEntity entityHit = col.collider.gameObject.GetComponentInParent<BaseEntity>();

        if (entityHit != null && entityHit.Team != Team)
                entityHit.TakeDamage(damage, Holder);

        Instantiate(hitParticle, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));
    }
}