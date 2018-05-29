using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GenerateHeightmap : MonoBehaviour {

    enum TextureSizes
    {
        p512 = 512,
        p1024 = 1024,
        p2048 = 2048,
        p4096 = 4096,
        p9192 = 9192
    }

    enum SaveMode
    {
        ToTargetPlane,
        ToFile
    }

    [SerializeField] private Collider targetPlane;
    [SerializeField] private TextureSizes textureSize;
    [SerializeField] private GameObject waterGameObject;
    [SerializeField] private SaveMode saveMode;
    [SerializeField] private string fileName = "/ExportedTextures/Heightmap.png";

    [Header("Scriptable object settings: ")]
    public GenerateHeightmapSettings GenSettings;

    public void Generate()
    { 
        targetPlane.enabled = true;
        waterGameObject.GetComponent<Collider>().enabled = true;

        Bounds bounds = targetPlane.bounds;
        float width = bounds.size.x;
        float step = width / (float)textureSize;
        float maxDist = GenSettings.raycastAltitude - GenSettings.lowestDetectableAltitude;
        float normCoef = 1.0f / maxDist;

        Color[] colors = new Color[(int)textureSize * (int)textureSize];
        for (int row = 0; row < (int)textureSize; row++)
        {
            float z = bounds.min.z + row * step;
            for (int col = 0; col < (int)textureSize; col++)
            {
                float x = bounds.min.x + col * step;

                RaycastHit hit;
                bool gotHit = Physics.Raycast(new Vector3(x, GenSettings.raycastAltitude, z), Vector3.down, out hit, maxDist);
                float altitude = gotHit ? (maxDist - hit.distance) * normCoef : 0.0f;

                Color color;
                if (CheckHitSomethingSpecial(hit, out color))
                {
                    colors[row * (int)textureSize + col] = color;
                }
                else
                {
                    colors[row * (int)textureSize + col] = 
                        hit.collider.gameObject == waterGameObject ? GenSettings.waterColor : GenSettings.gradient.Evaluate(altitude);
                }
            }
        }
            
        Texture2D newMap = new Texture2D((int)textureSize, (int)textureSize);
        newMap.SetPixels(colors);
        newMap.Apply();

        if (saveMode == SaveMode.ToFile)
        {
            byte[] bytes = newMap.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + fileName, bytes);
        }
        else
        {
            targetPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = newMap;
        }

        waterGameObject.GetComponent<Collider>().enabled = false;
        targetPlane.enabled = false;
    }

    private bool CheckHitSomethingSpecial (RaycastHit hit, out Color col)
    {
        GameObject go = hit.collider.gameObject;
        if (go.GetComponentInChildren<TerrainCollider>() != null)
        {
            col = Color.black;
            return false;
        }

        Renderer rend = go.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            Material[] mats = rend.sharedMaterials;

            foreach (var mat in mats)
            {
                if (GenSettings.matColor.ContainsKey(mat))
                {
                    col = GenSettings.matColor[mat];
                    return true;
                }
            }
        }

        col = Color.black;
        return false;
    }

}
