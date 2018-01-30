using UnityEngine;

public class XDPlayer : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("Camera");

            gameObject.SetLayerRecursively("Camera/Canvas", Layer.OwnedUI);
        }
    }
}
