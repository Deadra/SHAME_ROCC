﻿using UnityEngine;
using UnityStandardAssets.Utility;

/// <summary>
/// Класс для игрока на Fly Motion. Может возрождаться на стартовой позиции по нажатию кнопки
/// </summary>
/// <remarks>
/// В FlyPlayer нет прямой зависимости от PlatformDataSender, однако он нужен
/// для передачи данных на Simserver
/// </remarks>
[RequireComponent(typeof(PlatformDataSender))]
public class FlyPlayer : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("CameraParent/Camera");

            gameObject.SetLayerRecursively("CameraParent/Canvas", Layer.OwnedUI);

            spawnGunsInAllSlots();
        }
    }
}
