using UnityEngine;

/// <summary>
/// Класс, содержащий вспомогательные статические функции
/// </summary>
public class Helpers
{
    public static Vector3 GetPointInCircle(Vector3 center, float radius)
    {
        Vector2 point = Random.insideUnitCircle * radius;

        center.x += point.x;
        center.z += point.y;

        return center;
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

    public static T GetComponentInRoot<T>(GameObject gameObject)
    {
        return gameObject.transform.root.gameObject.GetComponent<T>();
    }

    public static T GetComponentInRootAndChildren<T>(GameObject gameObject)
    {
        return gameObject.transform.root.gameObject.GetComponentInChildren<T>();
    }
}
