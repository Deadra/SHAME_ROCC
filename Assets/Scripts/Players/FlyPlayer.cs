using UnityEngine;

public class FlyPlayer : BasePlayer
{
    public override void Start()
    {
        base.Start();

        if (isLocalPlayer)
        {
            if (transform.Find("Camera").GetComponentInChildren<Camera>())
                transform.Find("Camera").tag = "MainCamera";
            if (transform.Find("Camera/Canvas"))
                SetLayerRecursively(transform.Find("Camera/Canvas").gameObject, Layer.OwnedUI);
        }
    }

    protected override void Update()
    {
        base.Update();
        Vector3 ground = transform.position;
        ground.z = 0;
        if (transform.Find("Minimap/MiniMapCameraHolder"))
            transform.Find("Minimap/MiniMapCameraHolder").LookAt(ground);

        Debug.Log(this.GetComponent<Rigidbody>().velocity.magnitude);
    }
}
