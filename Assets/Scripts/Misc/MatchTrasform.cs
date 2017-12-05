using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchTrasform : MonoBehaviour {

    [SerializeField] Transform objectToMatch;

    void Start()
    {

    }

    void Update () 
    {
        this.transform.rotation = objectToMatch.transform.rotation;
        this.transform.position = objectToMatch.transform.position;
		
	}
}
