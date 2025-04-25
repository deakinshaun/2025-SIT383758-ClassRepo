Shader "Unlit/StereoPhotoShader"
{
    Properties
    {
        _LeftMainTex ("Texture", 2D) = "white" {}
        _RightMainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                UNITY_FOG_COORDS(0)
                float4 vertex : SV_POSITION;
                float4 objvertex : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _LeftMainTex;
            float4 _LeftMainTex_ST;
            sampler2D _RightMainTex;
            float4 _RightMainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.objvertex = v.vertex;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                // Calculate UV coordinates
                fixed2 uv;
                float xz = sqrt(i.objvertex.x * i.objvertex.x + i.objvertex.z * i.objvertex.z);
                float latitude = atan2(i.objvertex.y, xz);
                float longitude = atan2(i.objvertex.z, i.objvertex.x);
                uv.x = -longitude / (2 * 3.14159);
                uv.y = 0.5 + latitude / 3.14159;

                // Sample the texture
                fixed4 col;
                if (unity_StereoEyeIndex == 0)
                {
                    col = tex2D(_LeftMainTex, uv);
                }
                else
                {
                    col = tex2D(_RightMainTex, uv);
                }

                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}