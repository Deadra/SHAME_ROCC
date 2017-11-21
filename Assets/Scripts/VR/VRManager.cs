using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRManager : VRInputIncapsulator
{
    private VRTeleport teleport;
    [SerializeField] private PlayerVR player;

    void Start()
    {
        if (player == null)
            player = GetComponent<PlayerVR>();
        teleport = GetComponent<VRTeleport>();
        //player = GetComponentInChildren<PlayerDesktop>();
    }

    public override void OnLeftTouchPressed()
    {
        //Debug.Log("Left trigger pressed");
        teleport.AimTeleportLaser(LeftController);
    }

    public override void OnLeftTouchReleased()
    {
        teleport.AttemptTeleportation();
        //Debug.Log("Left trigger released");
    }

    public override void OnRightTriggerPressed()
    {
        player.FireGun();
    }

    public override void OnRightGripDown()
    {
        player.SwitchGun();
    }
}