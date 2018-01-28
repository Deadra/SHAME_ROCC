using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BasePlayer : BaseEntity {

    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject minimapCamera;
    protected Vector3 startingPosition;

    public override void Start()
    {
        base.Start();
        startingPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        if (!isLocalPlayer)
            return;

        healthBar.value = currentHealth / maxHealth;

        if (minimapCamera)
        {
            Vector3 pos = transform.position;
            pos.y += 500;
            minimapCamera.transform.position = pos;
            minimapCamera.transform.rotation = Quaternion.LookRotation(Vector3.down, transform.forward);
        }

    }

    [ClientRpc]
    protected override void RpcOnDeath()
    {
        //EHub.SignalEntityDeath(this);
        currentHealth = maxHealth;
        transform.position = startingPosition;
    }

}
