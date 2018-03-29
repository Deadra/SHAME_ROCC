using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Altimeter : MonoBehaviour {

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Text uiText;
    [SerializeField] private float checkDelay = 0.3f;
    [SerializeField] private float maxHeight = 2000.0f;
    

    private WaitForSeconds delay;

    private void Start()
    {
        delay = new WaitForSeconds(checkDelay);
        StartCoroutine(CheckAltitude());
    }

    private IEnumerator CheckAltitude()
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(targetTransform.position, Vector3.down, out hit, maxHeight))
            {
                uiText.text = ((int)hit.distance).ToString();
            }
            yield return delay;
        }
    }
}
