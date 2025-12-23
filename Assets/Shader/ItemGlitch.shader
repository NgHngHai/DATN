Shader "Custom/ItemGlitch"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _GlitchMap("Glitch Map", 2D) = "white" {}
        _GlitchTime("Glitch Time", Range(0, 3)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 data : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 data : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_GlitchMap);
            SAMPLER(sampler_GlitchMap);

            CBUFFER_START(UnityPerMaterial)
                float _GlitchTime;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color = IN.color;
                OUT.uv = IN.uv;
                OUT.data = IN.data;
                return OUT;
            }


            half4 frag(Varyings IN) : SV_Target
            {
                float c = cos(IN.data.x);
                float s = sin(IN.data.x);
                float2 offset = float2(SAMPLE_TEXTURE2D(_GlitchMap, sampler_GlitchMap, 0.5 + (IN.uv - 0.5) * 5).a, 0);

                float elapsedTime = _Time.y - IN.data.y;
                if (elapsedTime < _GlitchTime) {
                    offset *= sin((0.25 + elapsedTime / _GlitchTime) * TWO_PI) * 0.7;
                }
                else if (elapsedTime < _GlitchTime * 1.5) {
                    offset *= sin(elapsedTime / _GlitchTime * TWO_PI) * 0.35;
                }
                else offset *= 0;
                
                // offset -= IN.uv;
                offset = float2(offset.x * c, offset.x * s);
                // offset += IN.uv;

                IN.uv -= offset;
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;
                return color;
            }

            ENDHLSL
        }
    }
}
