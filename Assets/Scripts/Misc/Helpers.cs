using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers
{
    public static Vector3 GetPointInCircle(Vector3 center, float radius)
    {
        float x, z;
        do
        {
            x = Random.Range(center.x - radius, center.x + radius);
            z = Random.Range(center.z - radius, center.z + radius);
        }
        while ((x - center.x) * (x - center.x) + (z - center.z) * (z - center.z) >= radius * radius);
        //Debug.Log(string.Format("{0} units need a circle with radius {1}. Giving coordinates {2}.", numberOfUnits, circleRadius, new Vector3(x, center.y, z)));
        return new Vector3(x, center.y, z);
    }
}
