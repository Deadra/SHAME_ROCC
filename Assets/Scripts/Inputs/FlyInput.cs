using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof(AeroplaneController), typeof(FlyPlayer))]
    public class FlyInput : MonoBehaviour
    {
        // reference to the aeroplane that we're controlling
        private AeroplaneController aeroplane;
        private FlyPlayer player;

        private void Awake()
        {
            // Set up the reference to the aeroplane controller.
            aeroplane = GetComponent<AeroplaneController>();
            player = GetComponent<FlyPlayer>();
        }
        
        private void FixedUpdate()
        {
            // Read input for the pitch, yaw, roll and throttle of the aeroplane.
            float roll = CrossPlatformInputManager.GetAxis("Roll");
            float pitch = CrossPlatformInputManager.GetAxis("Pitch");
            float thrust = CrossPlatformInputManager.GetAxis("Thrust");
            bool airBrakes = CrossPlatformInputManager.GetButton("Fire1");
            // Pass the input to the aeroplane
            aeroplane.Move(roll, -pitch, 0, -thrust, airBrakes);
        }
    }
}
