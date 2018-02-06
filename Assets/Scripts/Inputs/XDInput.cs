using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// В зависимости от ввода игрока этот класс вызывает функции управления игровым персонажем 
/// </summary>
[RequireComponent(typeof(XDPlayer), typeof(CarController))]
public class XDInput : NetworkBehaviour
{
    XDPlayer player;
    CarController mover;

    void Start()
    {
        player = GetComponent<XDPlayer>();
        mover = GetComponent<CarController>();
    }

    void FixedUpdate()
    {
        float h = CrossPlatformInputManager.GetAxis("XDHorizontal");
        float v = CrossPlatformInputManager.GetAxis("XDVertical");
        float b = CrossPlatformInputManager.GetAxis("XDBrake");
        float handbrake = CrossPlatformInputManager.GetAxis("Jump");

        mover.Move(h, v, b, handbrake);

        if (Input.GetButton("Fire1"))
            player.FireGun();

        if (Input.GetButtonDown("Fire3"))
            player.SwitchGun();
    }
}