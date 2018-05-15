
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Класс для игрока на компьютере.
/// </summary>
public class DesktopPlayer : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer && !(this is VRPlayer))
        {
            gameObject.DefineMainCamera("CameraBase/Camera");

            gameObject.SetLayerRecursively("CameraBase/Head/HeadModel", Layer.OwnedBody);
            gameObject.SetLayerRecursively("Body", Layer.OwnedBody);
            gameObject.SetLayerRecursively("CameraBase/Canvas", Layer.OwnedUI);

            spawnGunsInAllSlots();
        }
    }

    
}