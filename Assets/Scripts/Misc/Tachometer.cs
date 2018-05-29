using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class Tachometer : MonoBehaviour
{
    [SerializeField] private Transform dialPointer;
    [SerializeField] private float angleScaleFactor = 4.0f;
    [SerializeField] private float lerpSpeed = 0.5f;
    [SerializeField] private CarController carController;

    private float defaultZ;
    private Quaternion targetRotation;

    private void Start()
    {
        defaultZ = dialPointer.localRotation.eulerAngles.z;
    }
    
    private void LateUpdate()
    {
        targetRotation = Quaternion.Euler(0, 0, defaultZ - carController.Revs * angleScaleFactor);
        dialPointer.localRotation = Quaternion.Slerp(dialPointer.localRotation, targetRotation, lerpSpeed);
    }
}
