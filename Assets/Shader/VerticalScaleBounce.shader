Shader "Custom/VerticalScaleBounce"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _EffectTime("Effect Time", Range(0, 3)) = 0.2
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
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 data : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _EffectTime;
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
                float elapsedTime = _Time.y - IN.data.y;
                float factor;
                if (elapsedTime < _EffectTime) {
                    factor = sin(elapsedTime * PI / _EffectTime) * IN.data.x + 1;
                }
                else if (elapsedTime < _EffectTime * 1.5) {
                    factor = -sin(elapsedTime * 2 * PI / _EffectTime) * IN.data.x / 2 + 1;
                }
                else factor = 1;

                IN.uv = 0.5 + (IN.uv - 0.5) * factor;
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;
                return color;
            }
            ENDHLSL
        }
    }
}
