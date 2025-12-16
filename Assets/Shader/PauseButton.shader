Shader "Custom/PauseButton"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _EffectMap("Effect Map", 2D) = "white" {}
        _TextMap("Text Map", 2D) = "white" {}
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
                float2 effectData : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 effectData : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_EffectMap);
            SAMPLER(sampler_EffectMap);
            TEXTURE2D(_TextMap);
            SAMPLER(sampler_TextMap);

            CBUFFER_START(UnityPerMaterial)
                // float _Factor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.effectData = IN.effectData;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half effectMapAlpha = SAMPLE_TEXTURE2D(_EffectMap, sampler_EffectMap, IN.uv).a;
                if (effectMapAlpha < 0.01) discard;
                if (effectMapAlpha > IN.effectData.x) discard;

                float2 screenUV = IN.positionHCS.xy / _ScreenParams.xy;
                screenUV.y = 1 - screenUV.y;
                half4 textMapColor = SAMPLE_TEXTURE2D(_TextMap, sampler_TextMap, screenUV);
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                color = color * (1 - textMapColor.a) + textMapColor;
                return color;
            }
            ENDHLSL
        }
    }
}
