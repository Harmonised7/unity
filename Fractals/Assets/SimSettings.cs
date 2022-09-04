using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SimSettings : ScriptableObject
{
    [Header("Simulation Settings")] [Min(1)]
    public int stepsPerFrame = 1;
    public int width = 512;
    public int height = 512;
    
    [Header("Initial Spawn")]
    [Min(0)]
    public int agentCount = 100;

    public float spawnRadius = 100;
    public SpawnType spawnType;
    
    [Header("Trail Settings")]
    public float trailWeight = 1;
    public float decayRate = 1;
    public float diffusionRate = 0.25f;

    public Specie[] species;

    [System.Serializable]
    public struct Specie
    {
        [Header("Movement")]
        public float moveSpeed;
        public float turnSpeed;

        [Header("Sensor")]
        public Vector3 mask;
        [Min(0)]public float sensorAngleOffset;
        public float sensorDistanceOffset;
        public int sensorSize;
        
        [Header("Display")]
        public Vector3 color;
    };

    public enum SpawnType
    {
        random,
        edges,
        circle,
        hollow_circle
    }
}
