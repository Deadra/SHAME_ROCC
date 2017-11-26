using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDPlayer : BasePlayer
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
}
