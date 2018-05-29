using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimsTest : MonoBehaviour
{
    [SerializeField] Animator headAnimator;
    [SerializeField] Animator bodyAnimator;

    void Start()
    {
        Physics.IgnoreCollision(this.GetComponent<Collider>(), this.GetComponentsInChildren<Collider>()[1]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            headAnimator.SetTrigger("Open1");
        }

        if (Input.GetKeyDown("k"))
        {
            headAnimator.SetTrigger("Close");
        }

        if (Input.GetKeyDown("l"))
        {
            headAnimator.SetTrigger("Open2");
        }

        if (Input.GetKeyDown("h"))
        {
            bodyAnimator.SetTrigger("Jump");
        }
    }
}