using UnityEngine;
using UnityEngine.Networking;

public class Frog : BaseEntity
{
    [SerializeField] ParticleSystem deathParticle;

    public override void Start()
    {
        base.Start();

        if (!isServer)
            gameObject.GetComponent<Collider>("Trigger").enabled = false;
    }

    protected override void RpcOnDeath()
    {
        RpcOnDeathParticles();
        EHub.SignalEnemyDeath(this.gameObject);
        base.RpcOnDeath();
    }

    [ClientRpc]
    void RpcOnDeathParticles()
    {
        Instantiate(deathParticle, transform.position, Quaternion.LookRotation(Vector3.up));
    }
}