Shader "Custom/StarNest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Zoom ("Zoom ",float) = 0.800
        _Tile ("Tile ",float) = 0.850
        _Speed ("Speed ",float) = 0.001
        _Brightness ("brightness ",float) =0.0015
        _Darkmatter ("darkmatter ",float) =0.300
        _Distfading ("distfading ",float) =0.730
        _Saturation ("saturation ",float) =0.850

    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Parameters
            #define iterations 17
            #define formuparam 0.53

            #define volsteps 20
            #define stepsize 0.1

            float _Zoom;
            float _Tile;
            float _Speed;
            float _Brightness;
            float _Darkmatter;
            float _Distfading;
            float _Saturation;


            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 objvertex : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.objvertex = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 rotate2D(float2 coord, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                return float3(coord.x * c - coord.y * s, coord.x * s + coord.y * c, 0);
            }

            float4 frag(v2f i) : SV_Target
            {
                fixed2 uv;
                float xz = sqrt(i.objvertex.x * i.objvertex.x + i.objvertex.z * i.objvertex.z);
                float latitude = atan2(i.objvertex.y, xz);
                float longitude = atan2(i.objvertex.z, i.objvertex.x);
                
                uv.y = 0.5 + latitude / 3.14159;
                uv.x = longitude / (2 * 3.14159);

                float3 dir = float3(uv * _Zoom, 1.0);
                float time = _Time.y * _Speed + 0.25;

                float3 from = float3(1.0, 0.5, 0.5);
                from += float3(time * 2.0, time, -2.0);

                float s = 0.1, fade = 1.0;
                float3 v = 0;

                for (int r = 0; r < volsteps; r++)
                {
                    float3 p = from + s * dir * 0.5;
                    p = abs(_Tile - fmod(p, _Tile * 2.0)); // tiling fold
                    float pa = 0, a = 0;
                    for (int i = 0; i < iterations; i++)
                    {
                        p = abs(p) / dot(p, p) - formuparam;
                        a += abs(length(p) - pa);
                        pa = length(p);
                    }
                    float dm = max(0.0, _Darkmatter - a * a * 0.001);
                    a *= a * a;
                    if (r > 6) fade *= 1.0 - dm;
                    v += fade;
                    v += float3(s, s * s, pow(s, 4)) * a * _Brightness * fade;
                    fade *= _Distfading;
                    s += stepsize;
                }
                v = lerp(float3(1,1,1) * length(v), v, _Saturation);
                return float4(v * 0.01, 1.0);
            }
            ENDCG
        }
    }
}