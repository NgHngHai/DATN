Shader "Sprites/GroundEdgeGradient"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _EdgeWorldPos ("Edge World Pos", Vector) = (0,0,0,0)
        _EdgeNormal ("Edge Normal (xy)", Vector) = (0,1,0,0)
        _MaxDistance ("Max Distance", Float) = 6
        _Softness ("Softness", Range(0.001,1)) = 0.35
        _MaxDarkness ("Max Darkness", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex:POSITION; float4 color:COLOR; float2 texcoord:TEXCOORD0; };
            struct v2f { float4 vertex:SV_POSITION; fixed4 color:COLOR; float2 uv:TEXCOORD0; float2 worldPos:TEXCOORD1; };

            sampler2D _MainTex; float4 _MainTex_ST; fixed4 _Color;
            float4 _EdgeWorldPos;
            float4 _EdgeNormal;
            float _MaxDistance, _Softness, _MaxDarkness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                float3 w = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldPos = w.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * i.color;

                float2 edgePos = _EdgeWorldPos.xy;
                float2 n = normalize(_EdgeNormal.xy);

                // Signed distance along the edge normal (only darken on the positive side)
                float d = max(0.0, dot(i.worldPos - edgePos, n));

                // 0 near edge, 1 when d >= _MaxDistance
                float t = saturate(d / max(_MaxDistance, 1e-4));

                // Soft ramp near the edge
                float ramp = smoothstep(0.0, 1.0, t) * smoothstep(0.0, _Softness, t);

                float dark = saturate(ramp) * _MaxDarkness;

                c.rgb *= (1.0 - dark); // multiplicative darkening
                return c;
            }
            ENDCG
        }
    }
}