using System;
using UnityEngine;

[Serializable]
public struct RoundSettings
{
    public float duration;      // seconds to survive this round
    public float spawnInterval; // time between cans
    public float minUpForce;    // min upward impulse
    public float maxUpForce;    // max upward impulse
    public float lateralForce;  // side impulse amount
    public float gravityScale;  // gravity per can
}
