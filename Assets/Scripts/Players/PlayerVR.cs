using UnityEngine;
using UnityEngine.Networking;

public class PlayerVR : PlayerDesktop {

    [SerializeField] Transform cameraRig;

    public override void Start()
    {
        base.Start();
        startingPosition = cameraRig.transform.position;

        if (isLocalPlayer)
        {
            if(transform.Find("[CameraRig]/Camera (head)/Camera (eye)/CameraBase/Head/HeadModel"))
                SetLayerRecursively(transform.Find("[CameraRig]/Camera (head)/Camera (eye)/CameraBase/Head/HeadModel").gameObject, Layer.OwnedBody);
            if(transform.Find("Avatar/Body"))
                SetLayerRecursively(transform.Find("Avatar/Body").gameObject, Layer.OwnedBody);
            if(transform.Find("[CameraRig]/Camera (head)/Camera (eye)/CameraBase/Canvas"))
                SetLayerRecursively(transform.Find("[CameraRig]/Camera (head)/Camera (eye)/CameraBase/Canvas").gameObject, Layer.OwnedUI);
        }
    }

    [ClientRpc]
    protected override void RpcOnDeath()
    {
        base.RpcOnDeath();
        cameraRig.transform.position = startingPosition;
    }
}
