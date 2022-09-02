using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Util : MonoBehaviour
{
    static ComputeShader clearTextureCompute = (ComputeShader)Resources.Load("ClearTexture");
    
    public static double mapCapped(double input, double inLow, double inHigh, double outLow, double outHigh)
    {
        return map(limitWithinRange(input, inLow, inHigh), inLow, inHigh, outLow, outHigh);
    }
    
    public static double map(double input, double inLow, double inHigh, double outLow, double outHigh)
    {
        if (inLow == inHigh)
            return outHigh;
        else
            return ((input - inLow) / (inHigh - inLow)) * (outHigh - outLow) + outLow;
    }
    
    public static double limitWithinRange(double input, double bound1, double bound2)
    {
        return bound2 > bound1 ? Math.Min(bound2, Math.Max(bound1, input)) : Math.Min(bound1, Math.Max(bound2, input));
    }
    
    public static int limitWithinRange(int input, int bound1, int bound2)
    {
        return bound2 > bound1 ? Math.Min(bound2, Math.Max(bound1, input)) : Math.Min(bound1, Math.Max(bound2, input));
    }

    public static float2 rot(float2 p, float2 pivot, float angle)
    {
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);
    
        p -= pivot;
        p = new float2(p.x*c-p.y*s, p.x*s+p.y*c);
        p += pivot;
    
        return p;
    }

    public static float getRandomAngle()
    {
        return Random.value * Mathf.PI * 2;
    }
    
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
    
    public static void createAndSetBuffer<T>(ref ComputeBuffer buffer, T[] data, ComputeShader cs, string nameID, int kernelIndex = 0)
    {
        int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        createStructuredBuffer<T>(ref buffer, data.Length);
        buffer.SetData(data);
        cs.SetBuffer(kernelIndex, nameID, buffer);
    }
    
    public static void createStructuredBuffer<T>(ref ComputeBuffer buffer, int count)
    {
        int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        bool createNewBuffer = buffer == null
                               || !buffer.IsValid()
                               || buffer.count != count
                               || buffer.stride != stride;
        if (createNewBuffer)
        {
            releaseBuffers(buffer);
            buffer = new ComputeBuffer(count, stride);
        }
    }
    
    public static void releaseBuffers(params ComputeBuffer[] buffers)
    {
        for (int i = 0; i < buffers.Length; i++)
        {
            if (buffers[i] != null)
                buffers[i].Release();
        }
    }
    
    public static void dispatch(ComputeShader cs, int numIterationsX, int numIterationsY = 1, int numIterationsZ = 1, int kernelIndex = 0)
    {
        Vector3Int threadGroupSizes = getThreadGroupSizes(cs, kernelIndex);
        int numGroupsX = Mathf.CeilToInt(numIterationsX / (float)threadGroupSizes.x);
        int numGroupsY = Mathf.CeilToInt(numIterationsY / (float)threadGroupSizes.y);
        int numGroupsZ = Mathf.CeilToInt(numIterationsZ / (float)threadGroupSizes.y);
        cs.Dispatch(kernelIndex, numGroupsX, numGroupsY, numGroupsZ);
    }
    
    public static Vector3Int getThreadGroupSizes(ComputeShader compute, int kernelIndex = 0)
    {
        uint x, y, z;
        compute.GetKernelThreadGroupSizes(kernelIndex, out x, out y, out z);
        return new Vector3Int((int)x, (int)y, (int)z);
    }
    
    public static void clearRenderTexture(RenderTexture source)
    {
        clearTextureCompute.SetInt("width", source.width);
        clearTextureCompute.SetInt("height", source.height);
        clearTextureCompute.SetTexture(0, "Source", source);
        dispatch(clearTextureCompute, source.width, source.height);
    }
}
