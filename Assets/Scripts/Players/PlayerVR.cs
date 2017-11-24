﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerVR : PlayerDesktop {

    [SerializeField] Transform cameraRig;
    [SerializeField] Transform leftShoulder;
    [SerializeField] Transform leftHandEnd;
    [SerializeField] Transform rightShoulder;
    [SerializeField] Transform rightHandEnd;
    GameObject neck;
    GameObject leftHand;
    GameObject rightHand;

    public override void Start()
    {
        if (isLocalPlayer)
        {
            if (transform.Find("Avatar"))
                SetLayerRecursively(transform.Find("Avatar").gameObject, Layer.OwnedBody);
            /*if (transform.Find("Avatar/Head/HeadModel"))
                SetLayerRecursively(transform.Find("Avatar/Head/HeadModel").gameObject, Layer.OwnedBody);
            if (transform.Find("Avatar/HandBase"))
                SetLayerRecursively(transform.Find("Avatar/HandBase").gameObject, Layer.OwnedBody);
            if (transform.Find("Avatar/Body"))
                SetLayerRecursively(transform.Find("Avatar/Body").gameObject, Layer.OwnedBody);*/
            if (transform.Find("[CameraRig]/Camera (head)/Camera (eye)/CameraBase/Canvas"))
                SetLayerRecursively(transform.Find("[CameraRig]/Camera (head)/Camera (eye)/CameraBase/Canvas").gameObject, Layer.OwnedUI);

        }
        base.Start();
        startingPosition = cameraRig.transform.position;
        leftHand = transform.Find("Avatar/HandBase/HandL").gameObject;
        rightHand = transform.Find("Avatar/HandBase/HandR").gameObject;
    }

    [ClientRpc]
    protected override void RpcOnDeath()
    {
        base.RpcOnDeath();
        cameraRig.transform.position = startingPosition;
    }

    void Update()
    {
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
