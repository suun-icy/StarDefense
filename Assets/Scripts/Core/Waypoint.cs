using UnityEngine;

/// <summary>
/// 敌人路径点系统
/// </summary>
public class Waypoint : MonoBehaviour
{
    public static Waypoint[] waypoints;
    
    private void Awake()
    {
        // 自动注册所有路径点
        waypoints = FindObjectsOfType<Waypoint>();
        System.Array.Sort(waypoints, (a, b) => a.transform.name.CompareTo(b.transform.name));
    }
    
    public static Vector3 GetNextPosition(Vector3 currentPosition, int currentIndex)
    {
        if (waypoints == null || waypoints.Length == 0)
            return currentPosition;
            
        int nextIndex = (currentIndex + 1) % waypoints.Length;
        return waypoints[nextIndex].transform.position;
    }
    
    public static int GetTotalWaypoints()
    {
        return waypoints != null ? waypoints.Length : 0;
    }
}
