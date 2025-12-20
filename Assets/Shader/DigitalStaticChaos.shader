Shader "Custom/DigitalStaticChaos"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _OffsetTex("Offset Texture", 2D) = "white" {}
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
            TEXTURE2D(_OffsetTex);
            SAMPLER(sampler_OffsetTex);

            // CBUFFER_START(UnityPerMaterial)
            //     float _GlitchTime;
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
                float elapsedTime = _Time.y - (int)_Time.y + fmod((int)_Time.y, 2);

                half4 offset = SAMPLE_TEXTURE2D(_OffsetTex, sampler_OffsetTex, float2(frac(_Time.y) * 16, frac(_Time.y)));
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(IN.uv.x + offset.x, IN.uv.y + elapsedTime / 2 - offset.x));
                color.a *= offset.a;

                color = lerp(half4(0, 0, 0, 1), color, step(0.5, fmod((floor(IN.positionHCS.y) + 5) / 5, 4)));

                return color;
            }

            ENDHLSL
        }
    }
}

