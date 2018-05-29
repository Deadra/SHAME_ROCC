using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCameraController : MonoBehaviour
{
    [SerializeField] private Transform initialTransform;
    private Quaternion targetRotation;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float maxLookAngle = 100;
    [SerializeField] private float turnSpeed = 0.2f;
    
    public void SetCameraRotation(float inputX, float inputY)
    {
        inputX *= maxLookAngle;
        inputY *= maxLookAngle;

        targetRotation = initialTransform.rotation * Quaternion.Euler(new Vector3(inputY, inputX, 0));
    }
           

    private void LateUpdate()
    {
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, turnSpeed);
        //cameraTransform.rotation = targetRotation;
    }
}
