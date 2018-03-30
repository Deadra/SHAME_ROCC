using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
/// <summary>
/// Этот класс отвечает за перемещения и повороты игрока на компьютере
/// </summary>
public class SpectatorController : DesktopController
{
    [SerializeField] private float followingInterpolation = 0.5f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform lookDirection;
    private NetManager netManager;
    [SerializeField] private Text uiText;
    private List<GameObject> connectedPlayers;

    private bool attached = false;
    private Transform targetPlayer;
    private Vector3 targetOffset;
    private Vector3 deltaMove;

    public override void Strafe(float horValue, float vertValue)
    {
        deltaMove = playerTransform.position + lookDirection.forward * horValue + lookDirection.right * vertValue;
        if (!attached)
        {
            playerTransform.position += lookDirection.forward * horValue + lookDirection.right * vertValue;
            deltaMove = Vector3.zero;
        }
        else
        {
            deltaMove = lookDirection.forward * horValue + lookDirection.right * vertValue;
        }
    }

    public override void Jump()
    {
        if (attached)
        {
            attached = false;
            return;
        }

        if (netManager == null)
        {
            netManager = FindObjectOfType<NetManager>();
        }

        connectedPlayers = netManager.GetConnectedPlayers();
        //uiText.text = connectedPlayers.Count.ToString();

        float minVal = float.MaxValue;
        int minInd = 0;
        for (int i = 0; i<connectedPlayers.Count; i++)
        {
            if (connectedPlayers[i] != this.gameObject)
            {
                float dist = Vector3.SqrMagnitude(connectedPlayers[i].transform.position - this.transform.position);
                if (dist != 0 && dist < minVal)
                {
                    minVal = dist;
                    minInd = i;
                }
            }
        }

        AttachTo(connectedPlayers[minInd]);
        uiText.text = minVal.ToString();
        
    }

    private void AttachTo(GameObject go)
    {
        targetPlayer = go.transform;
        targetOffset = go.transform.position - this.transform.position;
        attached = true;
    }

    private void LateUpdate()
    {
        if (attached)
        {
            targetOffset -= deltaMove;
            this.transform.position = Vector3.Lerp(this.transform.position, targetPlayer.position - targetOffset, followingInterpolation);
        }
    }


}