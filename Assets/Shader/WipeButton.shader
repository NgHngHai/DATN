Shader "Custom/WipeButton"
{
    Properties
    {
        _TextColor("Text Color", Color) = (1, 1, 1, 1)
        _MainTex("Main Texture", 2D) = "white" {}
        _WipeMap("Wipe Map", 2D) = "white" {}
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
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_WipeMap);
            SAMPLER(sampler_WipeMap);
            TEXTURE2D(_TextMap);
            SAMPLER(sampler_TextMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _TextColor;
                float _WipeProgress;
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
                half wipeAlpha = SAMPLE_TEXTURE2D(_WipeMap, sampler_WipeMap, IN.uv).a;
                if (wipeAlpha < 0.01) discard;
                if (wipeAlpha > _WipeProgress) discard;

                float2 screenUV = IN.positionHCS.xy / _ScreenParams.xy;
                half textMapAlpha = SAMPLE_TEXTURE2D(_TextMap, sampler_TextMap, screenUV).a;
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                color = color * (1 - textMapAlpha) + _TextColor * textMapAlpha;
                return color;
            }
            ENDHLSL
        }
    }
}
