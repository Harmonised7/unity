using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Util : MonoBehaviour
{
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
}
