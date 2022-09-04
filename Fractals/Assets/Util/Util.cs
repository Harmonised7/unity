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

    public static Vector2 getDir(float angle, float length = 1)
    {
        return new Vector2(MathF.Cos(angle) * length, MathF.Sin(angle) * length);
    }

    public static float getRandomAngle()
    {
        return Random.value * Mathf.PI * 2;
    }

    public static Vector2 getRandomPos(Vector2 boundA, Vector2 boundB)
    {
        return getRandomPos(boundA.x, boundB.x, boundA.y, boundB.y);
    }
    
    public static Vector2 getRandomPos(float aX, float aY, float bX = 0, float bY = 0)
    {
        return new Vector2(Random.Range(aX, bX), Random.Range(aY, bY));
    }
    
    public static float approachValue(float input, float goal, float step)
    {
        if(input == goal) 
            return input;
        return input < goal ?
            MathF.Min(input + step, goal) :
            MathF.Max(input - step, goal);
    }

    public static void createAndSetBuffer<T>(ref ComputeBuffer buffer, T[] data, ComputeShader cs, string nameID, int kernelIndex = 0)
    {
        // int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        createStructuredBuffer(ref buffer, data);
        cs.SetBuffer(kernelIndex, nameID, buffer);
    }
    
    public static void createStructuredBuffer<T>(ref ComputeBuffer buffer, T[] data)
    {
        createStructuredBuffer<T>(ref buffer, data.Length);
        buffer.SetData(data);
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

    public static RenderTexture createRenderTexture(int width, int height, int depth, bool defaultBlack = true)
    {
        RenderTexture renderTexture = new RenderTexture(width, height, depth);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        if(defaultBlack)
            clearRenderTexture(renderTexture);
        return renderTexture;
    }
    
    public static void copyRenderTexture(Texture source, RenderTexture target)
    {
        Graphics.Blit(source, target);
    }
}
