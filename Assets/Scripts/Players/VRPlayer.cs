﻿using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Класс для игрока на HTC Vive. 
/// Обладает всеми возможностями класса игрока на компьютере
/// </summary>
public class VRPlayer : DesktopPlayer {
    [Header("VRPlayer settings: ")]
    [SerializeField] Transform cameraRig;
    [SerializeField] Transform leftShoulder;
    [SerializeField] Transform leftHandEnd;
    [SerializeField] Transform rightShoulder;
    [SerializeField] Transform rightHandEnd;
    GameObject neck;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            gameObject.DefineMainCamera("[CameraRig]/Camera (eye)");

            gameObject.SetLayerRecursively("Avatar", Layer.OwnedBody);
            gameObject.SetLayerRecursively("[CameraRig]/Controller (left)/Canvas", Layer.OwnedUI);
            spawnGunsInAllSlots();
        }

        startingPosition = cameraRig.transform.position;
        //leftHand = transform.Find("Avatar/HandBase/HandL").gameObject;
        //rightHand = transform.Find("Avatar/HandBase/HandR").gameObject;
    }

    [ClientRpc]
    protected override void RpcOnDeath()
    {
        base.RpcOnDeath();
        cameraRig.transform.position = startingPosition;
    }

    protected override void Update()
    {
        base.Update();
        DrawLimb(leftHand, leftShoulder, leftHandEnd);
        DrawLimb(rightHand, rightShoulder, rightHandEnd);
    }

    void DrawLimb(GameObject limb, Transform start, Transform end)
    {
        if (start == null || end == null)
            return;
 
        limb.transform.position = Vector3.Lerp(start.position, end.position, 0.5f);
        limb.transform.LookAt(end.position);
        limb.transform.localScale = new Vector3(limb.transform.localScale.x, limb.transform.localScale.y, Vector3.Distance(start.position, end.position));
    }
}
