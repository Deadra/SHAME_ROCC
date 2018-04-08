using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class GenerateHeightmapSettings : ScriptableObject {

    [System.Serializable]
    public class MaterialHitSettings
    {
        public string pairName;
        public Material referenceMaterial;
        public Color color;
    }

    public float lowestDetectableAltitude;
    public float raycastAltitude;
    public Gradient gradient;
    public Color waterColor;

    public List<MaterialHitSettings> matColorList;

    public Dictionary<Material, Color> matColor;

    void OnValidate()
    {
        matColor = new Dictionary<Material, Color>();
        foreach (var pair in matColorList)
        {
            if (!matColor.ContainsKey(pair.referenceMaterial))
                matColor.Add(pair.referenceMaterial, pair.color);
        }
    }
}
