Shader "Custom/Map"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white" {}
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
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            // CBUFFER_START(UnityPerMaterial)
            //     half4 _TextColor;
            //     float _Progress;
            // CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, IN.uv);
                color.a = lerp(0, 1, step(0.01, mask.a));
                return color;
            }
            ENDHLSL
        }
    }
}
