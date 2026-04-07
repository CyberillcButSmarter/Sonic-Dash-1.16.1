Shader "Mobile/VertexLit" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
#pragma surface surf Lambert noforwardadd
#pragma target 2.0
        sampler2D _MainTex;
        struct Input {
            float2 uv_MainTex;
            float4 color : COLOR;
        };
        void surf(Input IN, inout SurfaceOutput o) {
            float4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * IN.color.rgb;
            o.Alpha  = c.a  * IN.color.a;
        }
        ENDCG
    }
    // Avoid self-referential fallback that triggers warnings on import
    Fallback "VertexLit"
}
