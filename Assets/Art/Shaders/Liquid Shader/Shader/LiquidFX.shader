Shader "Shir0dev/Liquid"
{
    Properties
    {
        [Header(Main)]
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Tint ("Tint", Color) = (1, 1, 1, 1)
        _Cutoff ("Cutoff", Range(0, 1)) = 0.5
        
        [Header(Foam)]
        [HDR]_FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _LineWidth ("Foam Line Width", Range(0, 1)) = 0.0
        _LineSmoothness ("Foam Line Smoothness", Range(0, 0.1)) = 0.0

        [Header(Rim)]
        [HDR]_RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0, 10)) = 10.0

        [Header(Sine)]
        _Freq ("Frequency", Range(0, 15)) = 0
        _Amplitude ("Amplitude", Range(0, 0.5)) = 0.15
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent-1" }
        
        Pass
        {
            Zwrite On
            Cull Off
            AlphaToMask Off

            CGPROGRAM

            float3 Unity_RotateAboutAxis_Degrees(float3 In, float3 Axis, float Rotation) {
                Rotation = radians(Rotation);
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;

                Axis = normalize(Axis);

                float3x3 rot_mat = 
                {   one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s, one_minus_c * Axis.z * Axis.x + Axis.y * s,
                    one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c, one_minus_c * Axis.y * Axis.z - Axis.x * s,
                    one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s, one_minus_c * Axis.z * Axis.z + c
                };
                float3 Out = mul(rot_mat,  In);
                return Out;
            }

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
                float3 normal   : NORMAL;
            };

            struct v2f
            {
                float2 uv           : TEXCOORD0;
                float4 vertex       : SV_POSITION;
                float3 positionWS   : TEXCOORD1;
                float3 viewDir      : COLOR;
                float3 normal       : COLOR2;
                float3 fillPosition : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint;
            
            float4 _RimColor;
            float _RimPower;

            float4 _FoamColor;
            float _LineWidth;
            float _LineSmoothness;

            float _Cutoff;
            float _WobbleX, _WobbleZ;
            float3 _BoundsMin, _BoundsCenter, _BoundsMax;
            float3 _Forward, _Right;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                float3 worldPosX = Unity_RotateAboutAxis_Degrees(v.vertex, _Forward, 90);
                worldPosX = mul(unity_ObjectToWorld, worldPosX);
                float3 worldPosZ = Unity_RotateAboutAxis_Degrees(v.vertex, _Right, 90);
                worldPosZ = mul(unity_ObjectToWorld, worldPosZ);
                
                float3 worldPosAdjusted = worldPos + (worldPosX * _WobbleX) + (worldPosZ * _WobbleZ);

                o.positionWS = worldPos;
                o.fillPosition = worldPosAdjusted;

                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.normal = v.normal;

                return o;
            }

            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
                float3 worldNormal = mul(unity_ObjectToWorld, float4(i.normal, 0.0)).xyz;
                
                // Fresnel calculations
                float fresnel = pow(1 - saturate(dot(worldNormal, i.viewDir)), _RimPower);
                float4 RimResult = fresnel * _RimColor;
                RimResult *= _RimColor;
                
                // Wobble calculations
                float wobbleIntensity = abs(_WobbleX) + abs(_WobbleZ);
                float wobble = sin((i.fillPosition.x) + (i.fillPosition.z)) * (wobbleIntensity);
                float movingFillPosition = i.fillPosition.y + wobble;
                
                // The local cutoff value relative to the mesh bounds
                // Values above this cutoff will be transparent
                float cutoff = lerp(_BoundsMin.y, _BoundsMax.y, _Cutoff);
                float cutoffStep = step(movingFillPosition, cutoff);

                float foam = cutoffStep * smoothstep(cutoff - _LineWidth - _LineSmoothness, cutoff - _LineWidth, movingFillPosition);
                float4 foamColored = foam * _FoamColor;

                float result = cutoffStep - foam;
                fixed4 baseColor = tex2D(_MainTex, i.uv) * _Tint;
                float4 resultColored = result * baseColor;

                float4 finalResult = resultColored + foamColored;
                finalResult.rgb += RimResult;

                float backfaceFoam = cutoffStep * smoothstep(0.5 - (0.2 * _LineWidth) - _LineSmoothness, 0.5 - (0.2 * _LineWidth), movingFillPosition);
                float4 backfaceFoamColor = _FoamColor * backfaceFoam;

                float4 topColor = (_FoamColor * (1 - backfaceFoam) + backfaceFoamColor) * (foam + result);

                clip(result + foam - 0.01);

                return facing > 0 ? finalResult : topColor;
            }
            ENDCG
        }
    }
}
