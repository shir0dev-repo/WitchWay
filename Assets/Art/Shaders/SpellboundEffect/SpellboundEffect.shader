//Shader "Unlit/SpellboundEffect"
//{
//    Properties
//    {
//        _Smoke ("Smoke", Range(0.01, 100)) = 0.01
//        _Speed ("Speed", Range(0.0, 1.0)) = 0.25
//        _Melty ("Melt", Range(0, 100)) = 50.0
//        _Flow ("Flow", Range(0, 100)) = 10.0
//        _Color0 ("Color 0", Color) = (1.0, 0.0, 0.0, 1.0)
//        _Color1 ("Color 1", Color) = (0.0, 1.0, 0.0, 1.0)
//        _Color2 ("Color 2", Color) = (0.0, 0.0, 1.0, 1.0)
//        _Color3 ("Color 3", Color) = (1.0, 1.0, 0.0, 1.0)
//        _Color4 ("Color 4", Color) = (1.0, 0.0, 1.0, 1.0)
//        _Color5 ("Color 5", Color) = (1.0, 1.0, 1.0, 1.0)
//    }
//    SubShader
//    {
//        Tags { "RenderType"="Opaque" }
//        LOD 100
//
//        Pass
//        {
//            CGPROGRAM
//            const float2 randConst = float2(432.14159, 528.14159);
//            const float randMultiplier = 3.14159;
//
//            float mix(float x, float y, float a) {
//                return mul(x, (1.0 - a)) + mul(y, a);
//            }
//
//            float2 mix(float2 x, float2 y, float a) {
//                return mul(x, (1.0 - a)) + mul(y, a);
//            }
//
//            float3 mix(float3 x, float3 y, float a) {
//                return mul(x, (1.0 - a)) + mul(y, a);
//            }
//
//            float4 mix(float4 x, float4 y, float a) {
//                return mul(x, (1.0 - a)) + mul(y, a);
//            }
//
//            float2 hash2(float n) {
//                return frac(sin(float2(n, n + 1.0)) * randConst);
//            }
//
//            float rand(const float2 co) {
//                return frac(sin(co.x * randConst.x + co.y * randConst.y) * randMultiplier);
//            }
//
//            float custom_smoothstep(float edge0, float edge1, float x) {
//                float t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
//                return t * t * (3.0 - 2.0 * t);
//            }
//
//            float3 custom_smoothstep(float3 edge0, float3 edge1, float x) {
//                float3 x3 = float3(x, x, x);
//                float3 t = clamp((x3 - edge0) / (edge1 - edge0), 0.0, 1.0);
//                return t * t * (3.0 - mul(t, 2.0));
//            }
//
//            float noise(float2 x) {
//                float2 p = floor(x);
//                float2 f = frac(x);
//                f = f * f * (3.0 - 2.0 * f);
//
//                float2 n = p + float2(2.5, -2.5);
//                float a = mix(rand(n), rand(n + float2(1.0, 0.0)), f.x);
//                float b = mix(rand(n + float2(0.0, 1.0)), rand(n + float2(1.0, 1.0)), f.x);
//
//                return mix(a, b, f.y);
//            }
//
//            float2 turbulence(float2 p, float t, float scale) {
//                float sum = 0.1;
//                float freq = scale;
//                float smoothness;
//                float2 noise_coord;
//
//                for (int i = 0; i < 5; i++) {
//                    smoothness = custom_smoothstep(0.0, 10.0, float(i));
//                    noise_coord = float2(p + t * 0.25) + float2(cos(float(i) * 0.5), sin(float(i) * 0.5)) * smoothness;
//                    sum += abs(noise(noise_coord)) / freq;
//                    freq *= 0.25;
//                }
//
//                return float2(sum, sum) * 0.1;
//            }
//
//            const float2x2 mtx = float2x2(float2(0.87, -0.5), float2(0.5, 0.87));
//
//            float fbm(float2 p) {
//                float f = 0.05;
//                f += 0.950000*noise( p ); p = mul(mtx, p) * 3.0;
//                f += 0.200000*noise( p ); p = mul(mtx, p) * 2.0;
//                f += 0.100000*noise( p ); p = mul(mtx, p) * 2.0;
//                f += 0.050000*noise( p ); p = mul(mtx, p) * 2.0;
//                f += 0.025000*noise( p ); p = mul(mtx, p) * 1.0;
//                f += 0.005000*noise( p );
//                p = mul(mtx, p) * 2.0;
//                f += 0.004*noise( p );
//                p = mul(mtx, p) * 2.0;
//                f += 0.002*noise( p );
//                
//                return f/0.95000;
//            }
//
//            
//
//            float pattern(float2 p, float t, float2 uv, float melt, float flow, out float2 q, out float2 r, out float2 g) {
//                float s = dot(uv + 0.5, uv + 0.5);
//                float l = custom_smoothstep(0.0, melt, sin(t * flow));
//                q = mix(float2(fbm(p + float2(t * 1. + sin(t), t * 0.2 + cos(t))),
//                    fbm(p + float2(t * 0.5 + sin(t + 0.5), t * 0.5 + cos(t + 1.5)))),
//                    float2(fbm(p), fbm(p + float2(10.5, 1.5))),
//                    l);
//                r = mix(float2(fbm(p + 3.14159 * q + float2(t * 0.25 + sin(t * 0.25), t * 0.25 + cos(t * 0.50)) + float2(1.5, 10.5)),
//                    fbm(p + 2.0 * q + float2(t * 0.5 + sin(t * 0.3), t * 0.4 + cos(t * 0.9)) + float2(8.5, 4.8))),
//                    float2(fbm(p + 5.0 * q + float2(t, t) + float2(33.66, 66.33)), fbm(p + 4.0 * q + float2(t, t) + float2(8.5, 2.5))),
//                    l);
//                g = mix(float2(fbm(p + 2.0 * r + float2(t * 0.5 + sin(t * 0.5), t * 0.5 + cos(t * 0.75)) + float2(2.5, 5)),
//                    fbm(p + 1.5 * r + float2(t * 0.75 + sin(t * 0.25), t * 0.5 + cos(t * 0.5)) + float2(5, 2.5))),
//                    float2(fbm(p + 2.5 * r + float2(t * 5.0, t * 5.0) + float2(2, 5)), fbm(p + 2. * r + float2(t * 11.0, t * 11.0) + float2(5, 2.5))),
//                    l);
//                
//                float2 v = turbulence(p * 0.1, t * 0.1, 20.);
//                float2 m = float2(fbm(p * 0.5 + float2(t * 0.9, t * 0.9) + v * 0.5),
//                    fbm(p * 0.5 + float2(t * 0.9, t * 0.9) + v * 0.5));
//                
//                return mix(
//                    fbm(p + 2.5 * g + float2(-t * 0.75 + sin(t * 0.5), -t * 0.5 + cos(t * 0.25)) + v * 2.5 + m * 0.25),
//                    fbm(p + 5.0 * g + float2(-t * 5.0, -t * 5.0) + v * 2.5),
//                    l);
//            }
//
//            #pragma vertex vert
//            #pragma fragment frag
//            // make fog work
//            #pragma multi_compile_fog
//
//            #include "UnityCG.cginc"
//
//            struct appdata
//            {
//                float4 vertex : POSITION;
//                float2 uv : TEXCOORD0;
//            };
//
//            struct v2f
//            {
//                float2 uv : TEXCOORD0;
//                float4 worldPos : TEXCOORD1;
//                UNITY_FOG_COORDS(2)
//                float4 vertex : SV_POSITION;
//            };
//
//            sampler2D _MainTex;
//            float _Smoke;
//            float _Speed;
//            float _Melty, _Flow;
//
//            float4 _Color0, _Color1, _Color2, _Color3, _Color4, _Color5;
//
//            v2f vert (appdata v)
//            {
//                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                o.uv = v.uv / _ScreenParams.xy;
//                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
//                UNITY_TRANSFER_FOG(o,o.vertex);
//                return o;
//            }
//
//            fixed4 frag (v2f i) : SV_Target
//            {
//                float2 q, r, g;
//                float noise = pattern(float2(i.vertex.x, 1 - i.vertex.y) * float2(_Smoke, _Smoke), _Time.y * _Speed, i.uv, _Melty, _Flow, q, r, g);
//                float3 col = mix(_Color0.xyz, _Color1.xyz, custom_smoothstep(0.1, 1.0, noise));
//                col = mix(col, _Color2.xyz, dot(q, q) * 1.5);
//                col = mix(col, _Color3.xyz, 0.25 * g.y * g.y);
//                col = mix(col, _Color4.xyz, custom_smoothstep(0.2, 0.5, 1.0 * r.g * r.g));
//                col = mix(col, _Color5.xyz, 0.5 * g.x);
//                float timeScale = 0.25;
//                float xDrift = sin(i.uv.x * 3.14159 + _Time.y * timeScale);
//                float yDrift = cos(i.uv.y * 3.14159 + _Time.y * timeScale);
//                float3 drift = mul(float3(xDrift, yDrift, -xDrift - yDrift), 0.1);
//                col += drift;
//                col = mix(col, float3(1, 1, 1), custom_smoothstep(0.0, 1.0, noise) * custom_smoothstep(0.0, 10.0, noise));
//                col *= noise * 2.0;
//                
//                return float4(col, 1.);
//            }
//            ENDCG
//        }
//    }
//}
//

Shader "Custom/ProceduralSmoke"
{
    Properties
    {
        _Smoke  ("Smoke Scale",  Float) = 0.01    // iSmoke on Shadertoy
        _Speed  ("Flow Speed",   Float) = 0.25    // iSpeed on Shadertoy
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            // If you’re on URP/HDRP you can add the usual blend/state tags.
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma target   3.0        // needs loops & sin/frac

            #include "UnityCG.cginc"

            //--------------------------------
            //  VERTEX PROGRAM
            //--------------------------------
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv  : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;                      // 0‑1 range
                return o;
            }

            //--------------------------------
            //  CONSTANTS / UNIFORMS
            //--------------------------------
            float  _Smoke;
            float  _Speed;

            //--------------------------------
            //  HELPERS (GLSL → HLSL)
            //--------------------------------
            // fract → frac
            #define fract(x) frac(x)

            const float2 randConst     = float2(432.14159, 528.14159);
            const float  randMultiplier = 3.14159;

            float rand (float2 co)
            {
                return fract(sin(dot(co, randConst)) * randMultiplier);
            }

            float custom_smoothstep(float edge0, float edge1, float x)
            {
                float t = saturate((x - edge0) / (edge1 - edge0));
                return t * t * (3.0 - 2.0 * t);
            }

            float noise(float2 x)
            {
                float2 p = floor(x);
                float2 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);

                float2 n = p + float2(2.5, -2.5);
                float a  = lerp(rand(n),
                                rand(n + float2(1.0, 0.0)),
                                f.x);
                float b  = lerp(rand(n + float2(0.0, 1.0)),
                                rand(n + float2(1.0, 1.0)),
                                f.x);
                return lerp(a, b, f.y);
            }

            float2 turbulence(float2 p, float t, float scale)
            {
                float  sum  = 0.1;
                float  freq = scale;

                [unroll]
                for (int i = 0; i < 5; ++i)
                {
                    float smooth = custom_smoothstep(0.0, 10.0, i);
                    float2 nUV   = (p + t * 0.25) +
                                   float2(cos(i * 0.5), sin(i * 0.5)) * smooth;

                    sum  += abs(noise(nUV)) / freq;
                    freq *= 0.25;
                }
                return float2(sum, sum) * 0.1;
            }

            // 2×2 rotation-ish matrix from Shadertoy
            float2x2 mtx = float2x2( 0.87,  0.5,
                                     -0.5,  0.87 );

            float fbm(float2 p)
            {
                float f = 0.05;

                f += 0.950000 * noise(p);          p = mul(p, mtx) * 3.0;
                f += 0.200000 * noise(p);          p = mul(p, mtx) * 2.0;
                f += 0.100000 * noise(p);          p = mul(p, mtx) * 2.0;
                f += 0.050000 * noise(p);          p = mul(p, mtx) * 2.0;
                f += 0.025000 * noise(p);          /* p unchanged */
                f += 0.005000 * noise(p);          p = mul(mtx, p) * 2.0;
                f += 0.004000 * noise(p);          p = mul(mtx, p) * 2.0;
                f += 0.002000 * noise(p);
                return f / 0.95;
            }

            float pattern(float2 p, float t, float2 uv,
                          out float2 q, out float2 r, out float2 g)
            {
                float iMelty   = 50.0;
                float iFlowing = 10.0;
                float l        = custom_smoothstep(0.0, iMelty, sin(t * iFlowing));

                // q
                q = lerp(float2(fbm(p + float2(t + sin(t), t * 0.2 + cos(t))),
                                fbm(p + float2(t * 0.5 + sin(t + 0.5), t * 0.5 + cos(t + 1.5)))),
                         float2(fbm(p),
                                fbm(p + float2(10.5, 1.5))),
                         l);

                // r
                r = lerp(float2(fbm(p + 3.14159 * q +
                                    float2(t * 0.25 + sin(t * 0.25),
                                           t * 0.25 + cos(t * 0.50)) +
                                    float2(1.5, 10.5)),
                                fbm(p + 2.0 * q +
                                    float2(t * 0.5 + sin(t * 0.3),
                                           t * 0.4 + cos(t * 0.9)) +
                                    float2(8.5, 4.8))),
                         float2(fbm(p + 5.0 * q + float2(t, t) + float2(33.66, 66.33)),
                                fbm(p + 4.0 * q + float2(t, t) + float2(8.5, 2.5))),
                         l);

                // g
                g = lerp(float2(fbm(p + 2.0 * r +
                                    float2(t * 0.5 + sin(t * 0.5),
                                           t * 0.5 + cos(t * 0.75)) +
                                    float2(2.5, 5.0)),
                                fbm(p + 1.5 * r +
                                    float2(t * 0.75 + sin(t * 0.25),
                                           t * 0.5  + cos(t * 0.5)) +
                                    float2(5.0, 2.5))),
                         float2(fbm(p + 2.5 * r + float2(t *  5.0, t * 5.0) + float2(2, 5)),
                                fbm(p + 2.0 * r + float2(t * 11.0, t * 11.0) + float2(5, 2.5))),
                         l);

                // v & m helpers
                float2 v = turbulence(p * 0.1, t * 0.1, 2000.0);
                float2 m = float2(fbm(p * 0.5 + float2(t * 0.9, t * 0.9) + v * 0.5),
                                  fbm(p * 0.5 + float2(t * 0.9, t * 0.9) + v * 0.5));

                return lerp(fbm(p + 2.5 * g +
                                float2(-t * 0.75 + sin(t * 0.5),
                                       -t * 0.5  + cos(t * 0.25)) +
                                v * 2.5 + m * 0.25),
                            fbm(p + 5.0 * g + float2(-t * 5.0, -t * 5.0) + v * 2.5),
                            l);
            }

            //--------------------------------
            //  FRAGMENT PROGRAM
            //--------------------------------
            float4 frag(v2f i) : SV_Target
            {
                float  time = _Time.y;
                float2 uv   = i.uv;

                // Shadertoy’s fragCoord in pixels
                float2 fragCoord = uv * _ScreenParams.xy;

                // --- MAIN PATTERN ---
                float2 q, r, g;
                float  n = pattern(fragCoord * _Smoke,
                                   time * _Speed,
                                   uv, q, r, g);

                // --- COLOR MIXING (identical to original) ---
                float3 col = lerp(float3(0.2, 0.4, 0.2),
                                  float3(0.0, 0.25, 0.5),
                                  custom_smoothstep(0.1, 1.0, n));
                col = lerp(col, float3(0.4, 0.2, 0.2), dot(q, q) * 1.5);
                col = lerp(col, float3(0.2, 0.4, 0.0), 0.25 * g.y * g.y);
                col = lerp(col, float3(0.4, 0.2, 0.2),
                           custom_smoothstep(0.2, 0.5, r.y * r.y));
                col = lerp(col, float3(0.2, 0.4, 0.6), 0.5 * g.x);

                // extra drift just like Shadertoy
                float timeScale = 0.25;
                float xDrift    = sin(uv.x * 3.14159 + time * timeScale);
                float yDrift    = cos(uv.y * 3.14159 + time * timeScale);
                col += float3(xDrift, yDrift, -xDrift - yDrift) * 0.1;

                col  = lerp(col, 1.0.xxx,
                            custom_smoothstep(0.0, 1.0, n) *
                            custom_smoothstep(0.0, 10.0, n));
                col *= n * 2.0;

                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}