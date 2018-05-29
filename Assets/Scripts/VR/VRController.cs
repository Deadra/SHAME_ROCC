using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerSides
{ Left, Right };

public class VRController : MonoBehaviour
{
    [SerializeField] ControllerSides controllerSide;
    [SerializeField] VRInputIncapsulator inputManager;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Start()
    {
        

        trackedObj = GetComponent<SteamVR_TrackedObject>();
        // if (controllerSide == ControllerSides.Right)
        //    inputManager.RightController = this.transform;
        // else
        //     inputManager.LeftController = this.transform;
    }

    void Update()
    {
        if (controllerSide == ControllerSides.Right)
        {
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
                inputManager.RightTouch = true;
            else
                inputManager.RightTouch = false;

            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
                inputManager.RightTrigger = true;
            else
                inputManager.RightTrigger = false;

            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Grip))
                inputManager.RightGrip = true;
            else
                inputManager.RightGrip = false;

            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
                inputManager.OnRightGripDown();
        }
        else
        {
            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
                inputManager.LeftTouch = true;
            else
                inputManager.LeftTouch = false;

            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                //Debug.Log("left controller trigger pressed");
                inputManager.LeftTrigger = true;
            }
            else
                inputManager.LeftTrigger = false;

            if (Controller.GetPress(SteamVR_Controller.ButtonMask.Grip))
                inputManager.LeftGrip = true;
            else
                inputManager.LeftGrip = false;
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
                inputManager.OnLeftGripDown();
        }
    }
}