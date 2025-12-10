Shader "Custom/Distortion"
{
    Properties
    {
        [PerRendererData] _MainTex("Main Texture", 2D) = "white" {}
        _DistortionMap("Distortion Map", 2D) = "white" {}
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
            TEXTURE2D(_DistortionMap);
            SAMPLER(sampler_DistortionMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _DistortionMapST;
                half _MainTexUVRatio;
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
                half4 distortionColor = SAMPLE_TEXTURE2D(_DistortionMap, sampler_DistortionMap, IN.uv * _DistortionMapST.xy + _DistortionMapST.zw);
                float2 uv = 0.5 + (IN.uv - 0.5) * _MainTexUVRatio;
                uv.x += lerp(-distortionColor.r, distortionColor.r, step(0.5, uv.x)) * _MainTexUVRatio * 0.2;
                // uv.y += lerp(-distortionColor.g, distortionColor.g, step(0.5, uv.y));
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                color.a = distortionColor.a;

                return color;
            }
            ENDHLSL
        }
    }
}
