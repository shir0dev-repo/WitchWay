Shader "Geometry/Deformation"
{
	Properties
	{

	}

	SubShader {
		Tags { "RenderType" = "Geometry" "RenderPipeline" = "UniversalRenderPipeline" }

		Pass {
			HLSLPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes {
				float4 positionOS : POSITION;
			};

			struct v2g {
				float4 positionHCS : SV_POSITION;
			};

			struct Varyings {
				float4 positionHCS : SV_POSITION;
			};

			Varyings vert(Attributes IN) {
				Varyings o;
				o.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

				return o;
			}
			[maxvertexcount(7)]
			void geom(triangle v2g inputs[3], inout TriangleStream<Varyings> triStream) {
				[unroll] for (int i = 0; i < 3; i++) {
					Varyings o;
					o.positionHCS = inputs[i].positionHCS;

					triStream.Append(o);
				}
			}

			half4 frag() :SV_Target {
				half4 c;
				c = half4(1, 1, 1, 1);

				return c;
			}
			ENDHLSL
		}
	}
}
