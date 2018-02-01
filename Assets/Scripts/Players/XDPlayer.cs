using UnityEngine;

/// <summary>
/// Класс для игрока на XD Motion.
/// </summary>
public class XDPlayer : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("Camera");
            gameObject.SetLayerRecursively("Camera/Canvas", Layer.OwnedUI);
            gameObject.GetComponent<Collider>("BottomCollider").enabled = false;
        }
    }
}
