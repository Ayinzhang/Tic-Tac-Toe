Shader "Circle"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
        _Fade("Fade", Range(0, 1)) = 0.0
    }

    SubShader
    {
        Tags { "IgnoreProjector" = "True" "PreviewType" = "Plane"}
        LOD 100

        Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Fade;

            v2f vert(appdata v)
            {
                v2f i;
                i.position = TransformObjectToHClip(v.vertex);
                i.uv = v.uv;
                return i;
            }

            half4 frag(v2f i) : SV_Target
            {
                float k = - tan(2 * PI * _Fade + PI / 4);
                if(_Fade < 0.125) { clip(i.uv[0] + i.uv[1] - 1); clip(k * i.uv[0] - i.uv[1] - 0.5 * k + 0.5);}
                else if(_Fade < 0.5) { clip(i.uv[0] + i.uv[1] - 1); clip(- k * i.uv[0] + i.uv[1] + 0.5 * k - 0.5);}
                else if(_Fade < 0.625) { clip(i.uv[0] + i.uv[1] - 1 < 0 && - k * i.uv[0] + i.uv[1] + 0.5 * k - 0.5 < 0 ? -1: 1);}
                else { clip(i.uv[0] + i.uv[1] - 1 < 0 && k * i.uv[0] - i.uv[1] - 0.5 * k + 0.5 < 0 ? -1: 1);}
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
