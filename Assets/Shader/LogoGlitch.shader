Shader "Custom/LogoGlitch"
{
    Properties
    {
        [PerRendererData] _MainTex("Main Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
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

            CBUFFER_START(UnityPerMaterial)
                float _RandomOffsetU;
                float _RandomOffsetV;
                float _AlphaValue;
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
                float2 uvFirstPass = float2(IN.uv.x + _RandomOffsetU, IN.uv.y + _RandomOffsetV);
                float2 uvSecondPass = float2(uvFirstPass.x - _RandomOffsetU * 0.7, uvFirstPass.y - _RandomOffsetV * 1.2);
                float2 uvThirdPass = float2(uvFirstPass.x + _RandomOffsetU + _RandomOffsetV, uvFirstPass.y + _RandomOffsetU - _RandomOffsetV);
                
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvSecondPass);
                half4 tmpColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvThirdPass);
                color = lerp(half4(color.r, 0, 0, 1), half4(0, tmpColor.g, 0, 1), tmpColor.a * 0.8);
                color.a = 1.0;
                tmpColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvFirstPass);;
                color = lerp(color, tmpColor, tmpColor.a);
                color.a = _AlphaValue;

                return color;
            }
            ENDHLSL
        }
    }
}
