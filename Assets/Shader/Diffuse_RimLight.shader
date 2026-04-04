Shader "Sonic Dash/Diffuse_Lit_Rim" {
Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
    _RimPower ("Rim Power", Range(0.5,8)) = 3
}
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0
        sampler2D _MainTex;
        float4 _RimColor;
        float _RimPower;
        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
        };
        void surf(Input IN, inout SurfaceOutput o) {
            float4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow(rim, _RimPower);
        }
        ENDCG
    }
    Fallback "Diffuse"
}
