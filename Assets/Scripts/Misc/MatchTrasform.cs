using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTrasform : MonoBehaviour {

    [SerializeField] Transform objectToMatch;

    void Update () 
    {
        transform.rotation = objectToMatch.transform.rotation;
        transform.position = objectToMatch.transform.position;
	}
}
