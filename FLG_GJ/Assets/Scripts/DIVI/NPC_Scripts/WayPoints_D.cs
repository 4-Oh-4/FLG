// Waypoint.cs

using UnityEngine;

// This makes the class visible in the Unity Inspector when used in an array.
[System.Serializable]
public class WayPoints_D
{
    public Transform waypointTransform; // The position the NPC will move to
    public float waitTime = 5f;         // How long the NPC will wait at this point
}