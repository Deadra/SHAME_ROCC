using UnityEngine;

/// <summary>
/// Класс, содержащий вспомогательные статические функции
/// </summary>
public class Helpers
{
    public static Vector3 GetPointInCircle(Vector3 center, float radius)
    {
        float x, z;
        do
        {
            x = Random.Range(center.x - radius, center.x + radius);
            z = Random.Range(center.z - radius, center.z + radius);
        } while (System.Math.Pow(x - center.x, 2) + System.Math.Pow(z - center.z, 2) >= System.Math.Pow(radius,2));

        return new Vector3(x, center.y, z);
    }

    public static bool IsPlayerInTheArea(Vector3 center, float radius)
    {
        var netManager = GameObject.FindObjectOfType<NetManager>();
        if (netManager == null)
        {
            Debug.LogWarning("Helpers: Can't find NetManager on the scene");
            return false;
        }

        foreach (var player in netManager.spawnedPlayers)
            if (Vector3.Distance(player.transform.position, center) < radius)
                return true;

        return false;
    }
}
