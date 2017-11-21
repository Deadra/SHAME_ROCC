using UnityEngine;
using UnityEngine.Networking;

public class PlayerVR : PlayerDesktop {

    [SerializeField] Transform cameraRig;
    [SerializeField] Transform leftShoulder;
    [SerializeField] Transform leftHandEnd;
    [SerializeField] Transform rightShoulder;
    [SerializeField] Transform rightHandEnd;
    public GameObject limbPrefab;
    GameObject leftHand;
    GameObject rightHand;
    GameObject neck;

    public override void Start()
    {
        base.Start();
        startingPosition = cameraRig.transform.position;

        if (isLocalPlayer)
        {
            if(transform.Find("Avatar/Head/HeadModel"))
                SetLayerRecursively(transform.Find("Avatar/Head/HeadModel").gameObject, Layer.OwnedBody);
            if(transform.Find("Avatar/Body"))
                SetLayerRecursively(transform.Find("Avatar/Body").gameObject, Layer.OwnedBody);
            if(transform.Find("[CameraRig]/Camera (head)/Camera (eye)/CameraBase/Canvas"))
                SetLayerRecursively(transform.Find("[CameraRig]/Camera (head)/Camera (eye)/CameraBase/Canvas").gameObject, Layer.OwnedUI);
        }
        CmdSpawnLimbs();
    }

    [ClientRpc]
    protected override void RpcOnDeath()
    {
        base.RpcOnDeath();
        cameraRig.transform.position = startingPosition;
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
            //Destroy(limb);
            return;
        }
        if (limb == null)
        {
            //CmdSpawnLimbs();
            return;
        }
        limb.transform.position = Vector3.Lerp(start.position, end.position, 0.5f);
        limb.transform.LookAt(end.position);
        limb.transform.localScale = new Vector3(limb.transform.localScale.x, limb.transform.localScale.y, Vector3.Distance(start.position, end.position));
    }

    public void SpawnLimbSetup(GameObject limb, bool isLeft)
    {
    }

    /// <summary>
    /// Спаунит пушку и отправляет информацию об этом клиенту. Выполняется на стороне сервера.
    /// </summary>
    [Command]
    void CmdSpawnLimbs()
    {
        leftHand = Instantiate(limbPrefab, leftShoulder);
        rightHand = Instantiate(limbPrefab, rightShoulder);
        NetworkServer.Spawn(leftHand);
        NetworkServer.Spawn(rightHand);

        RpcSpawnLimbs(leftHand, rightHand);
        SpawnLimbsSetup(leftHand, rightHand);
    }

    /// <summary>
    /// Обновляет пушку в соответствии с инфой, пришедшей от сервера. Выполняется на стороне клиента.
    /// </summary>
    [ClientRpc]
    void RpcSpawnLimbs(GameObject leftHand, GameObject rightHand)
    {
        SpawnLimbsSetup(leftHand, rightHand);
    }

    /// <summary>
    /// Настраивает параметры пушки, помещает её в руку игрока
    /// </summary>
    private void SpawnLimbsSetup(GameObject leftHand, GameObject rightHand)
    {
        leftHand.transform.parent = leftShoulder;
        rightHand.transform.parent = rightShoulder;
        DrawLimb(leftHand, leftShoulder, leftHandEnd);
        DrawLimb(rightHand, rightShoulder, rightHandEnd);

        if (isLocalPlayer)
        {
            SetLayerRecursively(leftHand, Layer.OwnedBody);
            SetLayerRecursively(rightHand, Layer.OwnedBody);
        }
    }
}
