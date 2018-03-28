using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDCameraController : MonoBehaviour
{
    [SerializeField] private Transform initialTransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookAngle = 45;
    [SerializeField] private float turnSpeed = 0.1f;

    private Vector3 leftDirection, rightDirection;
    private enum LookDirection { LookingStraight, LookingLeft, LookingRight}
    private LookDirection lookDirection = LookDirection.LookingStraight;

    private void Start()
    {
        leftDirection = new Vector3(0, -lookAngle, 0);
        rightDirection = new Vector3(0, lookAngle, 0);
    }

    public void LookLeft()
    {
        if (lookDirection == LookDirection.LookingStraight)
        {
            targetTransform.localRotation = Quaternion.Euler(leftDirection);
            lookDirection = LookDirection.LookingLeft;
        }
    }

    public void LookRight()
    {
        if (lookDirection == LookDirection.LookingStraight)
        {
            targetTransform.localRotation = Quaternion.Euler(rightDirection);
            lookDirection = LookDirection.LookingRight;
        }
    }

    public void LookStraight()
    {
        if (lookDirection != LookDirection.LookingStraight)
        {
            targetTransform.localRotation = initialTransform.localRotation;
            lookDirection = LookDirection.LookingStraight;
        }
    }

    private void LateUpdate()
    {
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetTransform.rotation, turnSpeed);
    }
}
