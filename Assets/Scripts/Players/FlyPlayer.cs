using UnityEngine;
using UnityStandardAssets.Utility;

public class FlyPlayer : BasePlayer
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

    protected override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Reset"))
            GetComponentInChildren<ObjectResetter>().DelayedReset(0.2f);
    }
}
