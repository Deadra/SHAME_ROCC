using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerAnalog : MonoBehaviour {

    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private float checkDelay = 0.3f;
    [SerializeField] private Transform dialPointer;
    [SerializeField] private float angleScaleFactor = 4.0f;
    [SerializeField] private float lerpSpeed = 0.5f;

    private WaitForSeconds delay;
    private float defaultZ;
    private Quaternion targetRotation;

    private void Start()
    {
        delay = new WaitForSeconds(checkDelay);
        StartCoroutine(CheckSpeed());
        defaultZ = dialPointer.localRotation.eulerAngles.z;
    }

    private IEnumerator CheckSpeed()
    {
        while (true)
        {
            targetRotation = Quaternion.Euler(0, 0, defaultZ - targetRigidbody.velocity.magnitude * angleScaleFactor);
            yield return delay;
        }
    }

    private void LateUpdate()
    {
        dialPointer.localRotation = Quaternion.Slerp(dialPointer.localRotation, targetRotation, lerpSpeed);
    }
}
