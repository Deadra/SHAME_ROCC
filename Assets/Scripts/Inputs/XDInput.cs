using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// В зависимости от ввода игрока этот класс вызывает функции управления игровым персонажем 
/// </summary>
[RequireComponent(typeof(XDPlayer), typeof(CarController))]
public class XDInput : NetworkBehaviour
{
    [SerializeField] private XDPlayer player;
    [SerializeField] private CarController mover;
    [SerializeField] private ObjectResetter objectResetter;
    [SerializeField] private XDCameraController cameraController;

    void FixedUpdate()
    {
        float steering     = CrossPlatformInputManager.GetAxis("XDHorizontal");
        float acceleration = CrossPlatformInputManager.GetAxis("XDVertical");
        float footBrake    = CrossPlatformInputManager.GetAxis("XDBrake");
        float handbrake    = CrossPlatformInputManager.GetAxis("Jump");

        mover.Move(steering, acceleration, footBrake, handbrake);

        if (Input.GetButtonDown("Reset"))
            objectResetter.DelayedReset(0.2f);

        if (Input.GetButton("XDFire"))
            player.FireGun();

        if (Input.GetButtonDown("XDWeaponSwitch"))
            player.SwitchGun();

        if (Input.GetButton("XDLookLeft"))
            cameraController.LookLeft();
        else if (Input.GetButton("XDLookRight"))
            cameraController.LookRight();
        else
            cameraController.LookStraight();
    }
}