using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    [SerializeField] Transform objectToMatch;

    void Update()
    {
        this.transform.rotation = objectToMatch.transform.rotation;
    }
}