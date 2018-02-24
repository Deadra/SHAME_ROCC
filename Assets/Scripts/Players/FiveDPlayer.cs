using UnityEngine;

/// <summary>
/// Класс для игрока на 5D Motion.
/// </summary>
/// <remarks>
/// В FiveDPlayer нет прямой зависимости от PlatformDataSender, однако он нужен
/// для передачи данных на Simserver
/// </remarks>
[RequireComponent(typeof(PlatformDataSender))]
public class FiveDPlayer : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("Camera");
            gameObject.SetLayerRecursively("Tram_Blue", Layer.OwnedBody);
        }
    }
}
