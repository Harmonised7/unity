using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SimSettings : ScriptableObject
{
    [Header("Simulation Settings")] [Min(1)]
    public int stepsPerFrame = 1;
    public int width = 1280;
    public int height = 720;
    public int spawnerCount = 100;
    
    [Header("Trail Settings")]
    public float trailWeight = 1;
    public float decayRate = 1;
    public float diffuseRate = 1;
}
