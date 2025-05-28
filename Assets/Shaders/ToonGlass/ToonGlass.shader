Shader "Unlit/ToonGlass"
{
    Properties
    {
        [Header(Main)]
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _Opacity ("Opacity", Range(0, 1)) = 0.3
        _MainTex ("Texture", 2D) = "white" {}

        [Header(Lacunarity)]
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularStrength ("Specular Strength", Float) = 10
        
        [Header(Fresnel)]
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 1, 1)
        _FresnelStrength ("Fresnel Strength", Range(0, 10)) = 0

    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        Zwrite Off
        Cull Back
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir : COLOR;
                float3 normal : COLOR2;
                float3 worldNormal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint, _SpecularColor, _FresnelColor;
            float _SpecularStrength, _FresnelStrength;
            float _Opacity;


            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.normal = v.normal;
                o.worldNormal = mul((float4x4)unity_ObjectToWorld, v.normal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldNormal = mul(unity_ObjectToWorld, float4(i.normal, 0.0)).xyz;
                float fresnel = pow(1 - saturate(dot(worldNormal, i.viewDir)), _FresnelStrength);
                float3 FresnelResult = fresnel * _FresnelColor;

                fixed4 col = tex2D(_MainTex, i.uv) * _Tint;
                col.rgb += FresnelResult;
                col.a = _Opacity;

                return col;
            }
            ENDCG
        }
    }
}
