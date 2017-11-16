using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : BaseBullet {

    [SerializeField]
    protected float radius = 5.0F;

    [SerializeField]
    protected float power = 1000.0F;

    [SerializeField]
    protected float baseDamage = 20.0F;

    [SerializeField]
    protected ParticleSystem ps;

    protected void SetOff(Collision col)
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && rb.tag != "Projectile")
                rb.AddExplosionForce(power, transform.position, radius, 3.0F);

            BaseEntity entityHit = hit.GetComponent<BaseEntity>();

            if (entityHit != null)
            {
                if (entityHit.Team != Team)
                {
                    Vector3 direction = rb.transform.position - transform.position;
                    float amountOfDamage = baseDamage - baseDamage * (direction.magnitude / radius);
                    entityHit.TakeDamage(amountOfDamage, Holder);
                    Debug.Log(string.Format("Entity {0} took {1} damage.", rb.gameObject, amountOfDamage));
                }
            }
                
        }
        ParticleSystem particle = Instantiate (ps, explosionPos, Quaternion.Euler(Vector3.up)); //Vector3.forward
        particle.transform.Rotate (new Vector3 (-90,0,0));
        particle.Play ();
    }
}
