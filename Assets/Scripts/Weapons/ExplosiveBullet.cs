using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Пуля, взрывающаяся и наносящая урон по площади
/// </summary>
public class ExplosiveBullet : BaseBullet
{
    [SerializeField] protected float radius = 5.0F;
    [SerializeField] protected float power = 1000.0F;
    [SerializeField] protected float baseDamage = 20.0F;
    [SerializeField] protected ParticleSystem explosion;

    /// <summary>
    /// Обработчик столкновения
    /// </summary>
    protected override void OnHit(RaycastHit hitInfo)
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.gameObject.GetComponentInParent<Rigidbody>();

            if (rb != null && rb.tag != "Projectile")
                rb.AddExplosionForce(power, transform.position, radius, 3.0F);

            BaseEntity entityHit = hit.gameObject.GetComponentInParent<BaseEntity>();

            if (entityHit != null && (Settings.friendlyFire || entityHit.Team != Team))
            {
                Vector3 direction = rb.transform.position - transform.position;
                float amountOfDamage = baseDamage - baseDamage * (direction.magnitude / radius);
                entityHit.TakeDamage(amountOfDamage, Holder);
            }
        }

        if (explosion != null)
        {
            ParticleSystem particle = Instantiate(explosion, explosionPos, Quaternion.Euler(Vector3.up)); 
            particle.transform.Rotate(new Vector3(-90, 0, 0));
            particle.Play();
        }
    }
}
