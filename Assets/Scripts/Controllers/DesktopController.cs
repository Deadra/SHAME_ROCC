using UnityEngine;

/// <summary>
/// Этот класс отвечает за перемещения и повороты игрока на компьютере
/// </summary>
public class DesktopController : BaseController
{
    [SerializeField] Transform desktopCamera;
    [SerializeField] float minCameraPitch = -80;
    [SerializeField] float maxCameraPitch = 80;
    [SerializeField] float verticalSens = 1.2f;
    [SerializeField] float horizontalSens = 1.2f;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float jumpForce = 300;

    public void Rotate(float value)
    {
        objectToMove.transform.Rotate(new Vector3(0, value * horizontalSens, 0));
    }

    public void TiltHead(float value)
    {
        desktopCamera.Rotate(new Vector3(-value * verticalSens, 0, 0));
        Vector3 currentRotation = desktopCamera.localRotation.eulerAngles;
        currentRotation.x = Mathf.Clamp(currentRotation.x, minCameraPitch, maxCameraPitch);
        desktopCamera.localRotation = Quaternion.Euler(currentRotation);
    }

    public void Strafe(float horValue, float vertValue)
    {
        float fallingSpeed = objectToMove.velocity.y;
        Vector3 newSpeed = Vector3.ClampMagnitude(transform.forward * horValue * moveSpeed + transform.right * vertValue * moveSpeed, moveSpeed);
        objectToMove.velocity = newSpeed + new Vector3(0, fallingSpeed, 0);
    }

    public void Jump()
    {
        if (CheckLanded())
        {
            objectToMove.AddForce(new Vector3(0, jumpForce, 0));
        }
    }

    bool CheckLanded()
    {
        if (Physics.Raycast(transform.position, -transform.up, 1.0f))
            return true;
        else
            return false;
    }
}