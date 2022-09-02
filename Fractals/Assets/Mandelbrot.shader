Shader "Explorer/Fractal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Area("Area", vector) = (0, 0, 4, 4)
        _MaxIter("MaxIter", range(4, 1023)) = 255
        _Angle("Angle", range(-3.1415, 3.1415)) = 0
        _Color("Color", range(0, 1)) = 0.5
        _Repeat("Repeat", float) = 1
        _R("R", float) = 2
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/Util/Util.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 _Area;
            float _Color, _Angle, _MaxIter, _Repeat, _R;
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 c = _Area.xy + (i.uv-.5)*_Area.zw;
                c = rot(c, _Area.xy, _Angle);
                float2 z;
                float iter;
                float dist;
                for(iter = 0; iter < _MaxIter; ++iter)
                {
                    z = float2(z.x*z.x-z.y*z.y, 2*z.x*z.y) + c;
                    dist = length(z);
                    if(dist > 2)
                        break;
                }
                
                if(iter > _MaxIter)
                    return 0;
                
                float r2 = _R*_R;
                    
                // float fracIter = (dist-_R) / (r2 - _R); //Linear Interpolation
                float fracIter = log(r2, dist) - 1;
                
                iter -= fracIter;
                
                float m = sqrt(iter / _MaxIter) * _Repeat;
                float4 col = sin(float4(.3 * _Color, .45 * _Color, .65 * _Color, 1)*m*15.23)*0.5+0.5;
                // col = tex2D(_MainTex, float2(m*_Repeat + _Time.y));
                return col;
            }
            ENDCG
        }
    }
}