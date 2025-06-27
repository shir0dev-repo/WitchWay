Shader "Toon/Cel-Shading"
{
    Properties
    {
        [Header(Main)]
        _MainTex ("Main Texture", 2D) = "white" {}
        [MainColor] _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
        [NoScaleOffset][Normal] _NormalTex ("Normal", 2D) = "bump" {}
        [NoScaleOffset] _RoughnessTex ("Roughness", 2D) = "white" {}
        [NoScaleOffset] _EmissiveTex ("Emissive", 2D) = "white" {}
        
        [Header(Emissive)]
        [HDR]_EmissiveColour ("Emissive Color", Color) = (0, 0, 0, 1)
        _EmissiveStrength ("Emissive Strength", Range(0, 100)) = 0
        
        [Header(Lighting)]
        _AmbientStrength ("Ambient Strength", Range(0, 1)) = 0.3
        _SpecEdge0 ("Lighting Cutoff", Range(0, 1)) = 0.0
        _SpecEdge1 ("Lighting Smoothness", Range(0, 1)) = 0.01

        [Header(Specular Highlights)]
        _SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
        _Glossiness ("Glossiness", Float) = 32

        [Header(Rim Highlights)]
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimAmount("Rim Amount", Range(0, 1)) = 0.65
        _RimThreshold ("Rim Threshold", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { 
            "RenderPipeline" = "UniversalRenderPipeline" 
            "RenderType"="Opaque" 
            "Queue" = "Transparent"
            "DisableBatching" = "True"
        }
        
        Pass
        {
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            // make shadows work
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(6)
                SHADOW_COORDS(7)
                float3 worldPos : TEXCOORD0;
                half3 tspace0 : TEXCOORD1;
                half3 tspace1 : TEXCOORD2;
                half3 tspace2 : TEXCOORD3;
                float2 uv : TEXCOORD4;
                
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD5;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _RoughnessTex;
            float4 _RoughnessTex_ST;
            sampler2D _NormalTex;
            float4 _NormalTex_ST;
            sampler2D _EmissiveTex;
            float4 _EmissiveTex_ST;

            float4 _Diffuse;
            float _AmbientStrength;
            
            float4 _EmissiveColour;
            float _EmissiveStrength;

            float _SpecEdge0;
            float _SpecEdge1;
            float _Glossiness;

            float4 _RimColor;
            float _RimAmount;
            float _RimThreshold;

            static float4 defaultBump = float4(0.5,0.5,1,0.5);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                TRANSFER_SHADOW(o)

                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                o.worldNormal = wNormal;
                
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(o.worldNormal, wTangent) * tangentSign;
                
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // normals
                fixed4 normSample = tex2D(_NormalTex, i.uv);
                float3 normal;

                if (length(normSample - defaultBump) != 0) 
                {
                    half3 tNormal = UnpackNormal(normSample);
                    normal.x = dot(i.tspace0, tNormal);
                    normal.y = dot(i.tspace1, tNormal);
                    normal.z = dot(i.tspace2, tNormal);
                }
                else
                {
                    normal = normalize(i.worldNormal);
                }
                
                // Lighting
                float NdotL = dot(_WorldSpaceLightPos0, normal);
                float shadow = SHADOW_ATTENUATION(i);

                float lightIntensity = smoothstep(_SpecEdge0, _SpecEdge1, NdotL * shadow);
                float4 lightColor = lightIntensity * _LightColor0;

                float3 viewDir = normalize(i.viewDir);

                float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
                float NdotH = dot(normal, halfVector);
                
                // Specular
                float specIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
                float specIntensitySmooth = smoothstep(_SpecEdge0, _SpecEdge1, specIntensity);
                fixed4 specSample = tex2D(_RoughnessTex, i.uv);
                float4 specularResult = _SpecColor * specIntensitySmooth * specSample;

                // Fresnel
                float4 rimDot = 1 - dot(viewDir, normal);
                float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
                rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
                fixed4 rimResult = rimIntensity * _RimColor;

                // Emissive
                fixed4 emissiveSample = tex2D(_EmissiveTex, i.uv);
                float4 emissiveResult = _EmissiveColour * emissiveSample * _EmissiveStrength;

                // sample the texture
                fixed4 sample = tex2D(_MainTex, i.uv);

                fixed4 col = _Diffuse * sample * (_AmbientStrength + lightColor + specularResult + rimResult);
                col += emissiveResult;
                // apply fog

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
