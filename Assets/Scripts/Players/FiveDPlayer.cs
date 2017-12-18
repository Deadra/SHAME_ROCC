using UnityEngine;

public class FiveDPlayer : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        { 
            //if (transform.Find("Camera/Canvas"))
            //    SetLayerRecursively(transform.Find("Camera/Canvas").gameObject, Layer.OwnedUI);
        }
    }
}
