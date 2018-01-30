using UnityEngine;

/// <summary>
/// Класс для игрока на 5D Motion.
/// </summary>
public class FiveDPlayer : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("Camera");
        }
    }
}
