using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Controller : MonoBehaviour
{
    const int agentKernel = 0;
    const int diffuseKernel = 1;
    // const int colorKernel = 2;
    
    public SimSettings _settings;
    RenderTexture trailTexture, diffuseTrailTexture, renderTexture;
    
    public ComputeShader _agentComputeShader;
    ComputeBuffer agentsBuffer;
    private Agent[] agents;
    
    ComputeBuffer speciesBuffer;
    
    // public Material mat;
    public Vector2 pos, smoothPos;
    public float scale, smoothScale, angle, smoothAngle;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    
    void FixedUpdate()
    {
        HandleInputs();
        UpdateShader();
        for (int i = 0; i < _settings.stepsPerFrame; i++)
        {
            runSim();
        }
    }
    
    private void UpdateShader()
    {
        smoothScale = Mathf.Lerp(smoothScale, scale, 0.1f);
        smoothAngle = Mathf.Lerp(smoothAngle, angle, 0.1f);
        smoothPos = Vector2.Lerp(smoothPos, pos, 0.1f);
        
        float aspect = (float)Screen.width / (float)Screen.height;

        float scaleX = smoothScale;
        float scaleY = smoothScale;

        if (aspect > 1f)
            scaleY /= aspect;
        else
            scaleX *= aspect;
        
        // mat.SetVector("_Area", new Vector4(smoothPos.x, smoothPos.y, scaleX, scaleY));
        // mat.SetFloat("_Angle", smoothAngle);
    }

    private void HandleInputs()
    {
        if(Input.GetKey(KeyCode.Equals))
            scale *= 0.98f;
        if(Input.GetKey(KeyCode.Minus))
            scale *= 1.02f;

        Vector2 dir = new Vector2(0.005f * scale, 0);
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);
        dir = new Vector2(dir.x*c, dir.x*s);

        if(Input.GetKey(KeyCode.A))
            pos -= dir;
        else if(Input.GetKey(KeyCode.D))
            pos += dir;

        dir = new Vector2(-dir.y, dir.x);
        if(Input.GetKey(KeyCode.S))
            pos -= dir;
        else if(Input.GetKey(KeyCode.W))
            pos += dir;
        
        if(Input.GetKey(KeyCode.E))
            angle -= 0.01f;
        else if(Input.GetKey(KeyCode.Q))
            angle += 0.01f;

    }

    private void Init()
    {
        renderTexture = Util.createRenderTexture(_settings.width, _settings.height, 24);
        trailTexture = Util.createRenderTexture(_settings.width, _settings.height, 24);
        diffuseTrailTexture = Util.createRenderTexture(_settings.width, _settings.height, 24);
        
        GetComponent<RawImage>().texture = diffuseTrailTexture;
        
        int speciesCount = _settings.species.Length;

        Vector2 screenMid = new Vector2(_settings.width / 2f, _settings.height / 2f);
        
        agents = new Agent[_settings.agentCount];
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i] = new Agent()
            {
                // pos = new Vector2(Random.Range(0, _settings.width), Random.Range(0, _settings.height)),
                // pos = new Vector2(_settings.width/2f, _settings.height/2f),
                pos = screenMid + Random.insideUnitCircle*(Math.Min(_settings.width, _settings.height)*0.25f),
                angle = Util.getRandomAngle(),
                speciesIndex = Random.Range(0, speciesCount)
            };
        }

        Util.createAndSetBuffer(ref agentsBuffer, agents, _agentComputeShader, "agents", kernelIndex: agentKernel);
        Util.createAndSetBuffer(ref speciesBuffer, _settings.species, _agentComputeShader, "species", kernelIndex: agentKernel);
        // Util.createStructuredBuffer(ref speciesBuffer, species);
        // _agentComputeShader.SetBuffer(agentKernel, "species", speciesBuffer);

        _agentComputeShader.SetInt("agentCount", _settings.agentCount);
        _agentComputeShader.SetTexture(agentKernel, "TrailMap", trailTexture);
        
        _agentComputeShader.SetTexture(diffuseKernel, "TrailMap", trailTexture);
        _agentComputeShader.SetTexture(diffuseKernel, "DiffusedTrailMap", diffuseTrailTexture);
        
        // _agentComputeShader.SetTexture(colorKernel, "TrailMap", trailTexture);
        // _agentComputeShader.SetTexture(colorKernel, "DiffusedTrailMap", diffuseTrailTexture);
    }

    private void runSim()
    {
        //Agents
        _agentComputeShader.SetFloat("width", _settings.width);
        _agentComputeShader.SetFloat("height", _settings.height);
        _agentComputeShader.SetFloat("midX", _settings.width/2f);
        _agentComputeShader.SetFloat("midY", _settings.height/2f);
        _agentComputeShader.SetFloat("deltaTime", Time.fixedDeltaTime);
        _agentComputeShader.SetFloat("time", Time.fixedTime);
        
        _agentComputeShader.Dispatch(agentKernel, Math.Min(65000, _settings.agentCount), 1, 1);
        _agentComputeShader.Dispatch(diffuseKernel, _settings.width, _settings.height, 1);
        // _agentComputeShader.Dispatch(colorKernel, _settings.width, _settings.height, 1);
        
        Util.copyRenderTexture(diffuseTrailTexture, trailTexture);
    }
    
    void OnDestroy()
    {
        Util.releaseBuffers(agentsBuffer, speciesBuffer);
    }
    
    struct Agent
    {
        public Vector2 pos;
        public float angle;
        public int speciesIndex;
    };
}
