using UnityEngine;
using UnityEngine.Networking;

public class PlayerVR : PlayerDesktop {

    [SerializeField] Transform cameraRig;

    public override void Start()
    {
        base.Start();
        startingPosition = cameraRig.transform.position;
    }

    [ClientRpc]
    protected override void RpcOnDeath()
    {
        base.RpcOnDeath();
        cameraRig.transform.position = startingPosition;
    }
}
