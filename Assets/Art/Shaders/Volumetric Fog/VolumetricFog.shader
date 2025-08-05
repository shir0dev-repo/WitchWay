Shader "Shir0dev/Volumetric Fog"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MinDistance ("Min Distance", float) = 10
        _MaxDistance ("Max Distance", float) = 100
        _StepSize ("Step Size", Range(0.1, 20)) = 1
        _DensityMultiplier ("Density Multiplier", Range(0, 10)) = 1
        _NoiseOffset ("Noise Offset", float) = 0
        _Height ("Fog Height Cutoff", float) = 2
        _FogNoise ("Fog Noise", 3D) = "white" {}
        _FogNoiseOverlay ("Fog Noise Overlay", 3D) = "white" {}
        _NoiseTiling ("Noise Tiling", float) = 1
        _DensityThreshold ("Density Threshold", Range(0, 1)) = 0.1
        [HDR]_LightContribution ("Light Contribution", Color) = (1, 1, 1, 1)
        _LightScattering ("Light Scattering", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS _ADDITIONAL_LIGHT_SHADOWS_CASCADE _ADDITIONAL_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP 

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float4 _Color;
            float _MinDistance;
            float _MaxDistance;
            float _StepSize;
            float _DensityMultiplier;
            float _NoiseOffset;
            TEXTURE3D(_FogNoise);
            TEXTURE3D(_FogNoiseOverlay);
            float _NoiseTiling;
            float _DensityThreshold;
            float4 _LightContribution;
            float _LightScattering;
            float _Height;

            float inverse_lerp(float a, float b, float t)
            {
                return (t - a) / (b - a);
            }

            float light_scattering(float angle, float scattering)
            {
                return (1.0 - angle * angle) / (4.0 * PI * pow(1.0 + scattering * scattering - (2.0 * scattering) * angle, 1.5));
            }

            float get_density(float3 worldPos) {
                float4 noise = _FogNoise.SampleLevel(sampler_TrilinearRepeat, worldPos * 0.01 * _NoiseTiling, 0);
                noise += _FogNoiseOverlay.SampleLevel(sampler_TrilinearRepeat, worldPos * 0.01 * _NoiseTiling, 0);
                float density = dot(noise, noise);

                density = saturate(density - _DensityThreshold) * _DensityMultiplier;
                return density;
            }

            float3 light_contribution(float density, float3 worldPos, float3 worldDir, Light light, uint lightIndex)
            {

                float4 lightPosition = _AdditionalLightsPosition[lightIndex];
                float3 toLight = lightPosition - worldPos;
                float viewDist = length(lightPosition - _WorldSpaceCameraPos);
                float dist = length(toLight);
                toLight = normalize(toLight);

                float NdotL = (dot(worldDir, toLight) + 1) * 0.5f;
                float scattering = light_scattering(NdotL, _LightScattering);
                float atten = max(0.5, inverse_lerp(_MinDistance, _MaxDistance, viewDist)) * light.shadowAttenuation * scattering * light.distanceAttenuation;
                float3 color = atten * light.color.rgb * _LightContribution.rgb;
                return color;
            }

            float3 get_additional_light_contribution(float density, float3 worldPos, float3 worldDir)
            {
                float3 lighting = 0;
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(worldPos));
                float scattering = light_scattering(dot(worldDir, mainLight.direction), _LightScattering);
                lighting += mainLight.color.rgb * _LightContribution.rgb * mainLight.shadowAttenuation * scattering;

                // Get additional lights
                #if defined(_ADDITIONAL_LIGHTS)

                #if USE_CLUSTER_LIGHT_LOOP
                int count = min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS);
                UNITY_LOOP for (uint lightIndex = 0; lightIndex < count; lightIndex++)
                {
                    Light additionalLight = GetAdditionalLight(lightIndex, worldPos, half4(1,1,1,1));
                    lighting += light_contribution(density, worldPos, worldDir, additionalLight, lightIndex) / float(count);
                }
                #endif

                uint pixelLightCount = GetAdditionalLightsCount();
                InputData inputData = (InputData)0;
                inputData.positionWS = worldPos;
                inputData.normalWS = worldDir;
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(inputData.positionWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(TransformWorldToHClip(inputData.positionWS));
                LIGHT_LOOP_BEGIN(pixelLightCount)
                    Light additionalLight = GetAdditionalLight(lightIndex, worldPos);
                    lighting += light_contribution(density, worldPos, worldDir, additionalLight, lightIndex);
                LIGHT_LOOP_END
                #endif

                return saturate(lighting);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 sceneColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);

                float depth = SampleSceneDepth(IN.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);

                float3 entryPoint = _WorldSpaceCameraPos;
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float viewLength = length(viewDir);
                float3 rayDir = normalize(viewDir);
                entryPoint += rayDir * _MinDistance;

                float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw;
                float distLimit = min(viewLength, _MaxDistance);
                float distTravelled = InterleavedGradientNoise(pixelCoords, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _NoiseOffset;
                float transmittance = 1.0;
                float4 fogCol = _Color;

                while (distTravelled < distLimit)
                {
                    float3 rayPos = entryPoint + rayDir * distTravelled;
                    
                    float density = get_density(rayPos);
                    if (density > 0)
                    {
                        //Light mainLight = GetMainLight(TransformWorldToShadowCoord(rayPos));
                        //float scattering = light_scattering(dot(rayDir, mainLight.direction), _LightScattering);
                        //float3 lightContribution = mainLight.color.rgb * _LightContribution.rgb * mainLight.shadowAttenuation * scattering;

                        float3 lightContribution = get_additional_light_contribution(transmittance, rayPos, rayDir);
                        float3 color = lightContribution * density * _StepSize;//lerp(0, , saturate(_Height - rayPos.y));
                        fogCol.rgb += color;
                        
                        float fogVal = exp(-density * _StepSize);///* exp(-pow(density, 2) * _StepSize);// */lerp(1, exp(-density * _StepSize), saturate(_Height - rayPos.y));
                        transmittance *= fogVal;
                    }

                    distTravelled += _StepSize;
                }

                return lerp(sceneColor, fogCol, 1.0 - saturate(transmittance));
            }
            ENDHLSL
        }
    }
}
