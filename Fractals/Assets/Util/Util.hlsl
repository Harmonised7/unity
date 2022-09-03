float map(float input, float inLow, float inHigh, float outLow, float outHigh)
{
    if (inLow == inHigh)
        return outHigh;
    else
        return ((input - inLow) / (inHigh - inLow)) * (outHigh - outLow) + outLow;
}

float limitWithinRange(float input, float bound1, float bound2)
{
    return bound2 > bound1 ? min(bound2, max(bound1, input)) : min(bound1, max(bound2, input));
}

int limitWithinRange(int input, int bound1, int bound2)
{
    return bound2 > bound1 ? min(bound2, max(bound1, input)) : min(bound1, max(bound2, input));
}

float mapCapped(float input, float inLow, float inHigh, float outLow, float outHigh)
{
    return map(limitWithinRange(input, inLow, inHigh), inLow, inHigh, outLow, outHigh);
}

float rand_1_05(in float2 uv)
{
    float2 noise = (frac(sin(dot(uv ,float2(12.9898,78.233)*2.0)) * 43758.5453));
    return abs(noise.x + noise.y) * 0.5;
}

float2 rand_2_10(in float2 uv) {
    float noiseX = (frac(sin(dot(uv, float2(12.9898,78.233) * 2.0)) * 43758.5453));
    float noiseY = sqrt(1 - noiseX * noiseX);
    return float2(noiseX, noiseY);
}

float2 rand_2_0004(in float2 uv)
{
    float noiseX = (frac(sin(dot(uv, float2(12.9898,78.233)      )) * 43758.5453));
    float noiseY = (frac(sin(dot(uv, float2(12.9898,78.233) * 2.0)) * 43758.5453));
    return float2(noiseX, noiseY) * 0.004;
}

float2 rot(float2 p, float angle)
{
    float s = sin(angle);
    float c = cos(angle);
    
    p = float2(p.x*c-p.y*s, p.x*s+p.y*c);
    
    return p;
}

float2 rot(float2 p, float2 pivot, float angle)
{
    float s = sin(angle);
    float c = cos(angle);
    
    p -= pivot;
    p = float2(p.x*c-p.y*s, p.x*s+p.y*c);
    p += pivot;
    
    return p;
}

float log(float base, float goal)
{
    return log2(goal) / log2(base);
}

uint hash(uint state)
{
    state ^= 2747636419u;
    state *= 2654435769u;
    state ^= state >> 16;
    state *= 2654435769u;
    state ^= state >> 16;
    state *= 2654435769u;
    return state;
}

float scaleToRange01(uint state)
{
    return state / 4294967295.0;
}

float2 getDir(float angle)
{
    return float2(cos(angle), sin(angle));
}

bool isOutside(float posX, float posY, float width, float height)
{
    return posX < 0 || posX >= width || posY < 0 || posY >= height;
}