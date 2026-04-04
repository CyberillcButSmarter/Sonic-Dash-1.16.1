Shader "Sonic Dash/Lightshafts" {
Properties {
    _FresnelExponent ("_FresnelExponent", Float) = 2
    _Diffuse ("_Diffuse", 2D) = "gray" {}
}
SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    LOD 200

    Cull Off
    ZWrite Off
    Blend One One  // Additive: light shafts brighten what's behind them

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        sampler2D _Diffuse;
        float4    _Diffuse_ST;
        float     _FresnelExponent;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv     : TEXCOORD0;
        };

        struct v2f {
            float4 pos     : SV_POSITION;
            float2 uv      : TEXCOORD0;
            float  fresnel : TEXCOORD1;
        };

        v2f vert(appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv  = TRANSFORM_TEX(v.uv, _Diffuse);

            // Compute Fresnel in world space: strongest when viewed edge-on
            float3 worldNormal = UnityObjectToWorldNormal(v.normal);
            float3 worldPos    = mul(unity_ObjectToWorld, v.vertex).xyz;
            float3 viewDir     = normalize(_WorldSpaceCameraPos - worldPos);

            // abs() so both faces contribute equally (Cull Off)
            float NdotV    = saturate(abs(dot(viewDir, worldNormal)));
            o.fresnel      = pow(1.0 - NdotV, _FresnelExponent);
            return o;
        }

        fixed4 frag(v2f i) : SV_Target {
            fixed4 c = tex2D(_Diffuse, i.uv);
            // Multiply RGB by fresnel; additive blend adds this to the scene
            return fixed4(c.rgb * c.a * i.fresnel, 1.0);
        }
        ENDCG
    }
}
Fallback "Transparent/Additive"
}
