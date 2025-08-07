Shader "Unlit/GrayscaleShader"
{
    Properties
    {
        _Strength ("Effect Strength", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP

            float _Strength;

            float4 frag (Varyings IN) : SV_Target
            {
                float4 sceneColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
                float grayscale = dot(sceneColor.rgb, float3(0.299, 0.587, 0.114));
                float4 result = lerp(sceneColor, float4(grayscale, grayscale, grayscale, 1.0), _Strength);
                return result;
            }
            ENDHLSL
        }
    }
}
