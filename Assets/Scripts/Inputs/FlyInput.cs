using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof(AeroplaneController), typeof(FlyPlayer), typeof(ObjectResetter))]
    public class FlyInput : NetworkBehaviour
    {
        [SerializeField] private AeroplaneController aeroplane;
        [SerializeField] private FlyPlayer player;
        [SerializeField] private ObjectResetter objectResetter;
        [SerializeField] private FlyCameraController cameraController;

        
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;

            float roll   = CrossPlatformInputManager.GetAxis("Roll");
            float pitch  = CrossPlatformInputManager.GetAxis("Pitch");
            float thrust = CrossPlatformInputManager.GetAxis("Thrust");
            bool  airBrakes = CrossPlatformInputManager.GetButton("Fire1");

            aeroplane.Move(roll, -pitch, 0, -thrust, airBrakes);

            float cameraX = CrossPlatformInputManager.GetAxis("FlyLookHorisontal");
            float cameraY = CrossPlatformInputManager.GetAxis("FlyLookVertical");

            cameraController.SetCameraRotation(cameraX, cameraY);

            if (Input.GetButtonDown("Reset"))
                objectResetter.DelayedReset(0.2f);

            if (Input.GetButton("FlyFire"))
                player.FireGun();

            if (Input.GetButtonDown("FlyWeaponSwitch"))
                player.SwitchGun();
        }
    }
}
