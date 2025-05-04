Shader "Unlit/MandelBrot360"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Zoom ("Zoom", Float) = 1.0
        _Offset ("Offset", Vector) = (0,0,0,0)
        _MaxIterations ("Max Iterations", Int) = 100
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 objvertex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Zoom;
            float4 _Offset;
            int _MaxIterations;

            v2f vert(appdata v)
            {
                v2f o;
                                o.objvertex = v.vertex;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0; // remap from [0,1] to [-1,1]

                return o;
            }

            float3 palette(float t)
            {
                float3 a = float3(0.5, 0.5, 0.5);
                float3 b = float3(0.5, 0.5, 0.5);
                float3 c = float3(1.0, 1.0, 1.0);
                float3 d = float3(0.263, 0.416, 0.557);

                return a + b * cos(6.28318 * (c * t + d));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float3 finalColor;
                float2 uv0 = i.uv;
                float xz = sqrt(i.objvertex.x * i.objvertex.x + i.objvertex.z * i.objvertex.z);
                float latitude = atan2(i.objvertex.y, xz);
                float longitude = atan2(i.objvertex.z, i.objvertex.x);
                uv.y = 0.5 + latitude / 3.14159;
                uv.x = longitude / (2 * 3.14159);
                for (float i = 0.0; i < 4.0; i++)
                {
                    uv = frac(uv * 1.5) - 0.5;

                    float d = length(uv) * exp(-length(uv0));

                    float3 col = palette(length(uv0) + i * .4 + _Time.y * .4);

                    d = sin(d * 8. + _Time.y) / 8.;
                    d = abs(d);

                    d = pow(0.01 / d, 1.2);

                    finalColor += col * d;
                }

                // float t = iter / (float)_MaxIterations;
                return fixed4(finalColor, 1); // colorful gradient
            }
            ENDCG
        }
    }
}