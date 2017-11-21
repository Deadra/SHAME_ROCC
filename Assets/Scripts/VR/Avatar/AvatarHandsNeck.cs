using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarHandsNeck : MonoBehaviour {

    [SerializeField] Transform leftShoulder;
    [SerializeField] Transform leftHandEnd;
    [SerializeField] Transform rightShoulder;
    [SerializeField] Transform rightHandEnd;
    [SerializeField] GameObject limbPrefab;
    GameObject leftHand;
    GameObject rightHand;
    GameObject neck;

    void Start()
    {
        leftHand = Instantiate(limbPrefab);
        rightHand = Instantiate(limbPrefab);
    }

    void Update()
    {
        DrawLimb(leftHand, leftShoulder, leftHandEnd);
        DrawLimb(rightHand, rightShoulder, rightHandEnd);
    }

    void DrawLimb(GameObject limb, Transform start, Transform end)
    {
        if (start == null || end == null)
        {
            Destroy(limb);
            return;
        }
        if (limb == null)
        {
            limb = Instantiate(limbPrefab);
        }
        limb.transform.position = Vector3.Lerp(start.position, end.position, 0.5f);
        limb.transform.LookAt(end.position);
        limb.transform.localScale = new Vector3(limb.transform.localScale.x, limb.transform.localScale.y, Vector3.Distance(start.position, end.position));
    }
}
