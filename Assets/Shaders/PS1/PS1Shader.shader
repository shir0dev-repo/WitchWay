Shader "Custom/PS1Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Banding ("Bit Reduction", Integer) = 8
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalRenderPipeline" "LightMode" = "UniversalForward" }
        Pass
        {
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            float4 TruncateToHClip(float4 posOS) {
                float4 clip = TransformObjectToHClip(posOS.xyz);
                float w = 1.0 / clip.w;
                clip.x *= w * _ScreenParams.x;
                clip.y *= w * _ScreenParams.y;
                clip.x = floor(clip.x) / _ScreenParams.x * clip.w;
                clip.y = floor(clip.y) / _ScreenParams.y * clip.w;

                return clip;
            }

            float4 QuantizeColor(float4 inColor, int bits) {
                float4 c = inColor * 255;
                c /= bits;

                return c / 255;
            }

            float3 ClipToWorldPos(float4 positionHCS) {
                float3 ndc = positionHCS.xyz / positionHCS.w;

                ndc = float3(ndc.x, ndc.y * _ProjectionParams.x, (1.0 - ndc.z) * 2.0 - 1.0);
                float3 viewPos = mul(unity_CameraInvProjection, float4(ndc * positionHCS.w, positionHCS.w)).xyz;

                return mul(unity_MatrixInvV, float4(viewPos, 1.0)).xyz;
            }

            struct Attributes 
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 normalOS     : NORMAL;

                float2 uv           : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            int _Banding;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings o;

                o.positionHCS = TruncateToHClip(IN.positionOS);
                o.normalOS = IN.normalOS;
                o.uv = IN.uv;
                return o;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 positionWS = ClipToWorldPos(IN.positionHCS);
                half3 l = VertexLighting(positionWS, IN.normalOS);
                Light ld = GetMainLight();
                
                half4 c = QuantizeColor(half4(LightingLambert(ld.color, ld.direction, IN.normalOS) * _Color, 1.0), _Banding);
                return c;
            }
            ENDHLSL
        }
    }
}
