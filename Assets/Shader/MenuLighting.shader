Shader "Custom/MenuLighting"
{
    Properties
    {
        [PerRendererData] _MainTex("Main Texture", 2D) = "white" {}
        _LightTex1("Light Texture 1", 2D) = "white" {}
        _LightFactor1("Light Factor 1", Range(0, 1)) = 0.5
        _LightTex2("Light Texture 2", 2D) = "white" {}
        _LightFactor2("Light Factor 2", Range(0, 1)) = 0.5
        _FlickeringTex("Flickering Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
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
            TEXTURE2D(_LightTex1);
            SAMPLER(sampler_LightTex1);
            TEXTURE2D(_LightTex2);
            SAMPLER(sampler_LightTex2);
            TEXTURE2D(_FlickeringTex);
            SAMPLER(sampler_FlickeringTex);

            CBUFFER_START(UnityPerMaterial)
                float _LightFactor1;
                float _LightFactor2;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                float4 light1 = SAMPLE_TEXTURE2D(_LightTex1, sampler_LightTex1, IN.uv);
                float4 light2 = SAMPLE_TEXTURE2D(_LightTex2, sampler_LightTex2, IN.uv);
                float factor = SAMPLE_TEXTURE2D(_FlickeringTex, sampler_FlickeringTex, float2(frac(_Time.y) * 16, frac(_Time.y))).a;
                color.rgb = lerp(color.rgb, saturate(color.rgb / (max(1.0 - light1.rgb, 0.001))), light1.a * _LightFactor1 * (sin(_Time.y * 2.5) + 1) / 2);
                color.rgb = lerp(color.rgb, saturate(color.rgb / (max(1.0 - light2.rgb, 0.001))), light2.a * _LightFactor2 * factor);
                color.rgb = pow(abs(color.rgb), 2.2);
                return color;
            }
            ENDHLSL
        }
    }
}
