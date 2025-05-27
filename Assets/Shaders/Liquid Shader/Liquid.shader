Shader "FX/Liquid"
{
    Properties
    {
        [Header(Main)]
        [HDR]_Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_TopColor ("Top Color", Color) = (1, 1, 1, 1)
        _FillAmount ("Fill Amount", Vector) = (0, 0.5, 0, 0)
        [Header(Foam)]
        [HDR]_FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _Line ("Foam Line Width", Range(0, 0.1)) = 0.0
        _LineSmooth ("Foam Line Smoothness", Range(0, 0.1)) = 0.0
        
        [Header(Rim)]
        [HDR] _RimColor("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0, 10)) = 0.0

        [Header(Sine)]
        _Freq ("Frequency", Range(0, 15)) = 0
        _Amplitude ("Amplitude", Range(0, 0.5)) = 0.15
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "DisableBatching" = "True" }
        
        Pass
        {
            Zwrite On
            Cull Off
            AlphaToMask On


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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 viewDir : COLOR;
                float3 normal : COLOR2;
                float3 fillPosition : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _FillAmount;
            float _WobbleX, _WobbleZ;
            float4 _TopColor, _RimColor, _FoamColor, _Tint;
            float _Line, _RimPower, _LineSmooth;
            float _Freq, _Amplitude;

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

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);
                float3 worldPosOffset = worldPos - _FillAmount;
                
                float3 worldPosX = Unity_RotateAboutAxis_Degrees(worldPosOffset, float3(0, 0, 1), 90);
                float3 worldPosZ = Unity_RotateAboutAxis_Degrees(worldPosOffset, float3(1, 0, 0), 90);

                float3 worldPosAdjusted = worldPos + (worldPosX * _WobbleX) + (worldPosZ * _WobbleZ);

                o.fillPosition = worldPosAdjusted - _FillAmount;
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.normal = v.normal;
                o.worldNormal = mul((float4x4)unity_ObjectToWorld, v.normal);

                return o;
            }

            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {
                float3 worldNormal = mul(unity_ObjectToWorld, float4(i.normal, 0.0)).xyz;

                float fresnel = pow(1 - saturate(dot(worldNormal, i.viewDir)), _RimPower);
                float4 RimResult = fresnel * _RimColor;
                RimResult *= _RimColor;

                float wobbleIntensity = abs(_WobbleX) + abs(_WobbleZ);
                float wobble = sin((i.fillPosition.x * _Freq) + (i.fillPosition.z * _Freq) + (_Time.y)) * (_Amplitude * wobbleIntensity);

                float movingFillPosition = i.fillPosition.y + wobble;

                fixed4 col = tex2D(_MainTex, movingFillPosition) * _Tint;

                UNITY_APPLY_FOG(i.fogCoord, col);

                float cutoffTop = step(movingFillPosition, 0.5);
                float foam = cutoffTop * smoothstep(0.5 - _Line - _LineSmooth, 0.5 - _Line, movingFillPosition);
                float4 foamColored = foam * _FoamColor;

                float result = cutoffTop - foam;
                float4 resultColored = result * col;

                float4 finalResult = resultColored + foamColored;
                finalResult.rgb += RimResult;

                float backfaceFoam = (cutoffTop * smoothstep(0.5 - (0.2 * _Line) - _LineSmooth, 0.5 - (0.2 * _Line), movingFillPosition));
                float4 backfaceFoamColor = _FoamColor * backfaceFoam;

                float4 topColor = (_TopColor * (1 - backfaceFoam) + backfaceFoamColor) * (foam + result);

                clip(result + foam - 0.01);

                return facing > 0 ? finalResult : topColor;
            }
            ENDCG
        }
    }
}
