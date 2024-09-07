Shader "Cross"
{
    Properties
    {
        _SubTex("Ext", 2D) = "white" {}
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

            TEXTURE2D(_SubTex);
            SAMPLER(sampler_SubTex);
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
                half4 subColor = SAMPLE_TEXTURE2D(_SubTex, sampler_SubTex, i.uv),
                    mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                if(_Fade < 0.5) clip(i.uv[0] + 2 * _Fade - 1);
                else if(2 * _Fade - i.uv[0] - 1 < 0) mainColor.a = 0;
                return subColor + (_Fade < 0.5 ? (0, 0, 0, 0): mainColor);
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
