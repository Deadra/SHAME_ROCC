
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Класс для игрока на компьютере.
/// </summary>
public class PlayerDesktop : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("CameraBase/Camera");

            gameObject.SetLayerRecursively("CameraBase/Head/HeadModel", Layer.OwnedBody);
            gameObject.SetLayerRecursively("Body", Layer.OwnedBody);
            gameObject.SetLayerRecursively("CameraBase/Canvas", Layer.OwnedUI);

            spawnGunsInAllSlots();
        }
    }

    
}