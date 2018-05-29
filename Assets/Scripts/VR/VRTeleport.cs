using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRTeleport : MonoBehaviour
{
    [SerializeField] Transform cameraRig;
    [SerializeField] Transform head;
    [SerializeField] Vector3 teleportReticleOffset;
    [SerializeField] LayerMask teleportMask;
    [SerializeField] GameObject laserPrefab;
    GameObject laser;
    [SerializeField] GameObject teleportReticlePrefab;
    [SerializeField] float maxTeleportDistance = 30;
    [SerializeField] float teleportCooldown = 2.0f;
    Slider coolDownSlider;
    [SerializeField] GameObject teleporterPrefab;
    [SerializeField] Transform teleporterPlaceholder;
    float nextTeleport;
    GameObject reticle;
    Vector3 hitPoint;
    bool shouldTeleport;
    bool inRange;

    void Start()
    {
        //trackedObj = GetComponent <SteamVR_TrackedObject> ();
        laser = Instantiate(laserPrefab);
        reticle = Instantiate(teleportReticlePrefab);
        nextTeleport = Time.time;
        GameObject instantiatedTeleporter = Instantiate(teleporterPrefab, teleporterPlaceholder.position, teleporterPlaceholder.rotation);
        instantiatedTeleporter.transform.parent = teleporterPlaceholder;
        coolDownSlider = instantiatedTeleporter.GetComponentInChildren<Slider>();
        coolDownSlider.value = 1;
    }

    public void AimTeleportLaser(Transform source)
    {
        RaycastHit hit;
        if (Physics.Raycast(source.transform.position, source.transform.forward, out hit, 300, teleportMask))
        {
            if (Vector3.Distance(hit.point, source.position) < maxTeleportDistance)
            {
                inRange = true;
            }
            else
            {
                inRange = false;
            }
            hitPoint = hit.point;
            ShowLaser(source.position, hit);
            reticle.SetActive(true);
            reticle.transform.position = hitPoint + teleportReticleOffset;
            shouldTeleport = true;
        }
    }

    public void AttemptTeleportation()
    {
        laser.SetActive(false);
        reticle.SetActive(false);

        if (shouldTeleport && CooldownComplete() && inRange)
        {
            Teleport();
        }
    }

    private void ShowLaser(Vector3 sourcePosition, RaycastHit hit)
    {
        laser.SetActive(true);
        laser.transform.position = Vector3.Lerp(sourcePosition, hitPoint, .5f);
        laser.transform.LookAt(hitPoint);
        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, hit.distance);

        if (CooldownComplete() && inRange)
        {
            laser.GetComponent<Renderer>().material.color = Color.green;
            reticle.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            laser.GetComponent<Renderer>().material.color = Color.red;
            reticle.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    private void Teleport()
    {
        nextTeleport = Time.time + teleportCooldown;
        shouldTeleport = false;
        reticle.SetActive(false);
        Vector3 difference = cameraRig.transform.position - head.position;
        difference.y = 0;
        cameraRig.position = hitPoint + difference;
    }

    bool CooldownComplete()
    {
        if (Time.time > nextTeleport)
            return true;
        else
            return false;
    }

    void Update()
    {
        if (Time.time > nextTeleport)
        {
            coolDownSlider.value = 1;
        }
        else
        {
            coolDownSlider.value = 1 - (nextTeleport - Time.time) / teleportCooldown;
        }
    }
}