Shader "Unlit/HeightToNormal"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _NormalIntensity("NormalIntensity",Float) = 1
        _HeightMapSizeX("HeightMapSizeX",Float) = 1024
        _HeightMapSizeY("HeightMapSizeY",Float) = 1024
    }
    SubShader
    {

        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct VertexInput {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
                float3 tangent : TANGENT;

            };

            struct VertexOutput {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 uv1 : TEXCOORD1;
                float4 normals : NORMAL;

                //float3 tangentSpaceLight: TANGENT;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;
            float _NormalIntensity;
            float _HeightMapSizeX;
            float _HeightMapSizeY;

            VertexOutput vert(VertexInput v) {

                VertexOutput o;
                o.uv = TRANSFORM_TEX( v.uv, _MainTex ); // used for texture
                o.uv1 = v.uv;
                o.normals = v.normal;
                o.vertex = UnityObjectToClipPos(v.vertex);

                 return o;
             }

            float4 frag(VertexOutput i) : COLOR
            {
                float me = tex2D(_MainTex,i.uv1).x;
                float n = tex2D(_MainTex,float2(i.uv1.x, i.uv1.y + 1.0 / _HeightMapSizeY)).x;
                float s = tex2D(_MainTex,float2(i.uv1.x, i.uv1.y - 1.0 / _HeightMapSizeY)).x;
                float e = tex2D(_MainTex,float2(i.uv1.x + 1.0 / _HeightMapSizeX,i.uv1.y)).x;
                float w = tex2D(_MainTex,float2(i.uv1.x - 1.0 / _HeightMapSizeX,i.uv1.y)).x;

                // defining starting normal as color has some interesting effects, generally makes this more flexible
                float3 norm = _Color;
                float3 temp = norm; //a temporary vector that is not parallel to norm
                if (norm.x == 1)
                temp.y += 0.5;
                else
                temp.x += 0.5;

                //form a basis with norm being one of the axes:
                float3 perp1 = normalize(cross(i.normals,temp));
                float3 perp2 = normalize(cross(i.normals,perp1));

                //use the basis to move the normal i its own space by the offset
                float3 normalOffset = -_NormalIntensity * (((n - me) - (s - me)) * perp1 + ((e - me) - (w - me)) * perp2);
                norm += normalOffset;
                norm = normalize(norm);

                // it's also interesting to output temp, perp1, and perp1, or combinations of the float samples.
                return float4(norm, 1);
            }
            ENDCG
         }
    }
}