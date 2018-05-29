using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarBodyFollow : MonoBehaviour {

    [SerializeField] Transform objectToFollow;
	
	// Update is called once per frame
	void Update () {
        transform.position = objectToFollow.position;
        Vector3 newRotation = objectToFollow.rotation.eulerAngles;
        newRotation.x = 0;
        newRotation.z = 0;
        transform.localRotation = Quaternion.Euler(newRotation);
	}
}
