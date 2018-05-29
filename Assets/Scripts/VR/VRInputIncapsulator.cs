using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRInputIncapsulator : MonoBehaviour
{
    [SerializeField] Transform leftController;
    public Transform LeftController
    {
        get { return leftController; }
        set
        {
            leftController = value;
            Debug.Log(leftController.position);
        }
    }

    public Transform RightController { get; set; }

    public Transform Head { get; set; }

    bool leftTrigger;
    public bool LeftTrigger
    {
        get { return leftTrigger; }
        set
        {
            leftTrigger = value;
            if (leftTrigger)
                OnLeftTriggerPressed();
            else
                OnLeftTriggerReleased();
        }
    }

    bool rightTrigger;
    public bool RightTrigger
    {
        get { return rightTrigger; }
        set
        {
            rightTrigger = value;
            if (rightTrigger)
                OnRightTriggerPressed();
            else
                OnRightTriggerReleased();
        }
    }

    bool leftGrip;
    public bool LeftGrip
    {
        get { return leftGrip; }
        set
        {
            leftGrip = value;
            if (leftGrip)
                OnLeftGripPressed();
            else
                OnLeftGripReleased();
        }
    }

    bool rightGrip;
    public bool RightGrip
    {
        get { return rightGrip; }
        set
        {
            rightGrip = value;
            if (rightGrip)
                OnRightGripPressed();
            else
                OnRightGripReleased();
        }
    }

    bool leftTouch;
    public bool LeftTouch
    {
        get { return leftTouch; }
        set
        {
            leftTouch = value;
            if (leftTouch)
                OnLeftTouchPressed();
            else
                OnLeftTouchReleased();
        }
    }

    bool rightTouch;
    public bool RightTouch
    {
        get { return rightTouch; }
        set
        {
            rightTouch = value;
            if (RightTouch)
                OnRightTouchPressed();
            else
                OnRightTouchReleased();
        }
    }

    public virtual void OnRightTouchPressed() { }

    public virtual void OnLeftTouchPressed() { }

    public virtual void OnRightGripPressed() { }

    public virtual void OnRightGripDown() { }

    public virtual void OnLeftGripPressed() { }

    public virtual void OnLeftGripDown() { }

    public virtual void OnRightTriggerPressed() { }

    public virtual void OnLeftTriggerPressed() { }

    public virtual void OnRightTouchReleased() { }

    public virtual void OnLeftTouchReleased() { }

    public virtual void OnRightGripReleased() { }

    public virtual void OnLeftGripReleased() { }

    public virtual void OnRightTriggerReleased() { }

    public virtual void OnLeftTriggerReleased() { }
}