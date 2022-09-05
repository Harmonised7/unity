using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu()]
public class SimSettings : ScriptableObject
{
    private Controller controller;
    
    public const float deg180inRad = 1.57075f;
    
    [Header("Simulation Settings")]
    [Range(1, 10)]public int stepsPerFrame = 1;
    [Min(16)]public int width = 512;
    [Min(16)]public int height = 512;
    
    [Header("Initial Spawn")]
    [Min(1)]public int agentCount = 100;

    [Min(0)]public float spawnRadius = 100;
    public SpawnType spawnType;
    
    [Header("Trail Settings")]
    [Range(1, 5)]public int trailSize = 1;
    [Range(0, 100)]public float trailWeight = 0.5f;
    [Range(0, 5f)]public float diffusionRate = 0.25f;

    public Specie[] species;

    [System.Serializable]
    public struct Specie
    {
        [Header("Movement")]
        [Range(0, 500)]public float moveSpeed;
        [Range(0, 100f)]public float turnSpeed;
        [Range(0, 1)] public int dejavuTurn;
        [Range(0, 2)]public float wanderIntensity;

        [Header("Sensor")]
        // public Vector3 wants;
        [Range(0, deg180inRad*2)]public float sensorAngleOffset;
        [Range(0, 500)]public float sensorDistanceOffset;
        [Range(1, 5)]public int sensorSize;
        
        [Header("Display")]
        public Vector3 color;
    };

    public enum SpawnType
    {
        Random,
        Edges,
        CircleFull,
        CircleRandom,
        CircleInside,
        CircleOutside,
        CircleEye
    }

    public void setController(Controller controller)
    {
        this.controller = controller;
    }

    public void OnValidate()
    {
        if(controller != null)
            controller.checkUpdate();
    }
}
