using UnityEngine;
using UnityStandardAssets.Utility;

public class FlyPlayer : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            if (transform.Find("Camera").GetComponentInChildren<Camera>())
                transform.Find("Camera").tag = "MainCamera";
            if (transform.Find("Camera/Canvas"))
                SetLayerRecursively(transform.Find("Camera/Canvas").gameObject, Layer.OwnedUI);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Reset"))
            GetComponentInChildren<ObjectResetter>().DelayedReset(0.2f);
    }
}
