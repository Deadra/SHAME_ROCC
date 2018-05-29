using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class AddMinimap : MonoBehaviour 
{
	
    [SerializeField] private string searchRegEx;
    [SerializeField] private string spawnedObjectTag = "SpawnedMinimap";
    [SerializeField] private SingleUnityLayer minimapLayer;
    [SerializeField] private Material minimapMaterial;
    [SerializeField] private Vector3 mapOffset;

    private GameObject[] targets;

    public List<GameObject> Parse(string regExString)
    {
        List<GameObject> result = new List<GameObject>();
        Regex regEx = new Regex(regExString);
        var everything = FindObjectsOfType<GameObject>();
        foreach (var thing in everything)
        {
            if (regEx.IsMatch(thing.name))
            {
                result.Add(thing);
            }
        }
        Debug.Log("Parsing result for " + regExString + ": " + result.Count.ToString() + " matches found");
        return result;
    }

    public void ClearMinimap()
    {
        List<GameObject> objects = Parse(searchRegEx);
        foreach (var go in objects)
        {
            ClearMinimapForObject(go);
        }
        Debug.Log("Minimaps cleared");
    }

	void ClearMinimapForObject(GameObject target)
	{
		for (int i = target.transform.childCount - 1; i >= 0; i--) 
		{
			var child = target.transform.GetChild(i);
			if (child.CompareTag(spawnedObjectTag))
				DestroyImmediate(child.gameObject);
		}
	}

    public void PutQuad()
    {
        List<GameObject> objects = Parse(searchRegEx);
        foreach (var go in objects)
        {
            PutQuadForObject(go);
        }
        Debug.Log("Minimaps added");
    }

    void PutQuadForObject(GameObject target)
	{
        if (target.GetComponent<Collider>() == null)
        {
            Debug.LogError("Object " + target.name + " has no collider attached to it! Can not spawn minimap!");
            return;
        }

        var spawnedQuad = new GameObject("GeneratedMinimap");
        Mesh mesh = spawnedQuad.AddComponent<MeshFilter>().mesh;
        var rend = spawnedQuad.AddComponent<MeshRenderer>();
		
        List<Vector3> verts = new List<Vector3> ();
		Bounds bounds = target.GetComponent<Collider> ().bounds;
        verts.Add(bounds.min);
        verts.Add(bounds.max);
        verts.Add(new Vector3 (bounds.max.x, bounds.min.y, bounds.min.z));
        verts.Add(new Vector3 (bounds.min.x, bounds.min.y, bounds.max.z));

		Vector3 positionOffset = spawnedQuad.transform.position - target.transform.position;
		spawnedQuad.transform.position = target.transform.position + mapOffset;
		for (int i = 0; i < verts.Count; i++) 
		{
			verts [i] = verts [i] + positionOffset;
		}
		mesh.SetVertices (verts);
        mesh.SetTriangles(new int[] { 0, 1, 2, 0, 3, 1 }, 0);

		List<Vector3> normals = new List<Vector3> ();
		mesh.GetNormals (normals);
		for (int i = 0; i < normals.Count; i++)
		{
			normals[i] = Vector3.up;
		}
		mesh.SetNormals (normals);

        spawnedQuad.transform.parent = target.transform;
		spawnedQuad.tag = spawnedObjectTag;
        rend.material = minimapMaterial;
        spawnedQuad.layer = minimapLayer.LayerIndex;
        spawnedQuad.isStatic = true;
	}
	
    [System.Serializable]
    public class SingleUnityLayer
    {
        [SerializeField]
        private int m_LayerIndex = 0;
        public int LayerIndex
        {
            get { return m_LayerIndex; }
        }

        public void Set(int _layerIndex)
        {
            if (_layerIndex > 0 && _layerIndex < 32)
            {
                m_LayerIndex = _layerIndex;
            }
        }

        public int Mask
        {
            get { return 1 << m_LayerIndex; }
        }
    }
}
