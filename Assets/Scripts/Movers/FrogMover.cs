﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogMover : BaseMover
{
    private Rigidbody rb;
    [SerializeField] float jumpForce;
    [SerializeField] Transform head;
    public Transform CurrentTarget { get; set; }
    float moveTime = 4;
    float moveStarted;
    float nextMove;
    float jumpCooldown = 3f;
    float nextJump;
    const float jumpForceFactor = 0.9f;

    [SerializeField] Animator headAnimator;
    [SerializeField] Animator bodyAnimator;

    public bool Aggressive { get; set; }

    bool canBite = false;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        Aggressive = true;
        rb = this.GetComponent<Rigidbody>();
        nextMove = Time.time;
        nextJump = Time.time;
        Physics.IgnoreCollision(this.GetComponent<Collider>(), GameObject.Find("Frog/FrogHead").GetComponent<Collider>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    public void JumpTowards()
    {
        if (Time.time > nextMove)
        {
            nextMove = Time.time + moveTime;
            moveStarted = Time.time;
        }
        if (Time.time > moveStarted + 1 && Time.time < moveStarted + 2)
        {
            BodyMatchHead();
        }
        if (Time.time > moveStarted + 2) //&& LooksAtTarget(currentTarget.position))
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (Time.time > nextJump)
        {
            nextJump = Time.time + jumpCooldown;
            Vector3 forward = head.forward;
            forward.y = 0;
            forward = forward.normalized;
            rb.AddForce((forward + Vector3.up * jumpForceFactor).normalized * jumpForce);
            bodyAnimator.SetTrigger("Jump");
            if (Aggressive)
                Bite();
        }
    }

    public bool LookAtTarget()
    {
        return LookAtTarget(CurrentTarget.position);
    }

    public bool LookAtTarget(Transform lookTargetTransform)
    {
        return LookAtTarget(lookTargetTransform.position);
    }

    public bool LookAtTarget(Vector3 lookTarget)
    {
        float rotSpeed = 360f;
        Vector3 D = lookTarget - head.transform.position;
        Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(D), rotSpeed * Time.deltaTime);
        head.rotation = rot;

        float minRotation = -80;
        float maxRotation = 80;
        Vector3 currentRotation = head.localRotation.eulerAngles;
        if (currentRotation.y < 180)
        {
            currentRotation.y = Mathf.Clamp(currentRotation.y, minRotation, maxRotation);
        }
        else
        {
            currentRotation.y = Mathf.Clamp(currentRotation.y, 360 + minRotation, 360);
        }
        head.localRotation = Quaternion.Euler(currentRotation);

        return LooksAtTarget(lookTarget);
    }

    public void BodyMatchHead()
    {
        float rotSpeed = 4f;
        Quaternion rot = Quaternion.Slerp(transform.rotation, head.rotation, rotSpeed * Time.deltaTime);
        //rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, 0);
        transform.eulerAngles = new Vector3(0, rot.eulerAngles.y, 0);
    }

    public bool LooksAtTarget(Vector3 lookTarget)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - head.position);
        Quaternion currentRotation = head.rotation;
        float rotatingError = Mathf.Abs(targetRotation.eulerAngles.y - currentRotation.eulerAngles.y);
        if (rotatingError < 5)
            return true;
        else
            return false;
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log(string.Format("Frog collided with {0}", col.collider.gameObject));
        headAnimator.SetTrigger("Close");
        if (canBite)
        {
            BaseEntity enemy = col.collider.gameObject.GetComponent<BaseEntity>();
            //Debug.Log("pam");
            if (enemy != null && enemy.Team != this.GetComponent<BaseEntity>().Team)
            {
                enemy.TakeDamage(15);
                //Debug.Log("poom");
            }
        }
        canBite = false;
    }

    void Bite()
    {
        canBite = true;
        if (Physics.Raycast(head.position, head.forward, 8.0f))
        {
            int c = Random.Range(0, 2);
            switch (c)
            {
                case 0:
                    headAnimator.SetTrigger("Open1");
                    break;

                case 1:
                    headAnimator.SetTrigger("Open2");
                    break;
            }
        }
    }
}