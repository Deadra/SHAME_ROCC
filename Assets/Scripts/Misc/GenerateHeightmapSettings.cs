using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class GenerateHeightmapSettings : ScriptableObject {

    public float lowestDetectableAltitude;
    public float raycastAltitude;
    public Gradient gradient;
    public Color waterColor;
}
