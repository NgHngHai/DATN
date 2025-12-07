Shader "Custom/Glitch"
{
    Properties
    {
        [PerRendererData] _BaseMap("Base Map", 2D) = "white"
        _GlitchMap("Glitch Map", 2D) = "white" {}
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
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_GlitchMap);
            SAMPLER(sampler_GlitchMap);

            CBUFFER_START(UnityPerMaterial)
                float _MapUV_u;
                float _GlitchFactor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 mapValue = SAMPLE_TEXTURE2D(_GlitchMap, sampler_GlitchMap, float2(_MapUV_u, IN.uv.y));
                float uOffset = (mapValue.a - 0.5) * _GlitchFactor;
                if (uOffset < -2.0 || uOffset > 2.0) discard;
                float2 targetUV = float2(IN.uv.x + uOffset, IN.uv.y);

                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, targetUV);
                return color;
            }
            ENDHLSL
        }
    }
}
