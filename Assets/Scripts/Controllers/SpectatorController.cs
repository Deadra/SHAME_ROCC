using UnityEngine;

/// <summary>
/// Этот класс отвечает за перемещения и повороты игрока на компьютере
/// </summary>
public class SpectatorController : DesktopController
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform lookDirection;

   public override void Strafe(float horValue, float vertValue)
    {
        playerTransform.position += lookDirection.forward * horValue + lookDirection.right * vertValue;
       
    }
}