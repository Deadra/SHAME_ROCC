using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour {

    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private Text uiText;
    [SerializeField] private float checkDelay = 0.3f;

    private WaitForSeconds delay;

    private void Start()
    {
        delay = new WaitForSeconds(checkDelay);
        StartCoroutine(CheckSpeed());
    }

    private IEnumerator CheckSpeed()
    {
        while (true)
        {
            uiText.text = ((int)(targetRigidbody.velocity.magnitude * 3.6f)).ToString();
            yield return delay;
        }
    }
}
