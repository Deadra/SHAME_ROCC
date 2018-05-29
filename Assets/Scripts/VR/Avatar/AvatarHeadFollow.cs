using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarHeadFollow : MonoBehaviour {

    [SerializeField] Transform objectToFollow;
	
	// Update is called once per frame
	void Update () {
        transform.position = objectToFollow.position;
        transform.rotation = objectToFollow.rotation;
	}
}
