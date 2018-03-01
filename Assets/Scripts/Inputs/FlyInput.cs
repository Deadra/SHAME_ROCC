using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof(AeroplaneController), typeof(FlyPlayer))]
    public class FlyInput : MonoBehaviour
    {
        private AeroplaneController aeroplane;
        private FlyPlayer player;

        private void Awake()
        {
            aeroplane = GetComponent<AeroplaneController>();
            player = GetComponent<FlyPlayer>();
        }
        
        private void FixedUpdate()
        {
            float roll = CrossPlatformInputManager.GetAxis("Roll");
            float pitch = CrossPlatformInputManager.GetAxis("Pitch");
            float thrust = CrossPlatformInputManager.GetAxis("Thrust");
            bool airBrakes = CrossPlatformInputManager.GetButton("Fire1");

            aeroplane.Move(roll, -pitch, 0, -thrust, airBrakes);

            /*if (Input.GetButton("FlyFire"))
                player.FireGun();

            if (Input.GetButtonDown("FlyWeaponSwitch"))
                player.SwitchGun();*/
        }
    }
}
