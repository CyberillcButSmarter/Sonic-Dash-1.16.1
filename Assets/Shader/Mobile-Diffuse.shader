Shader "Mobile/Diffuse" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
    Tags { "Queue"="Geometry" "RenderType"="Opaque" }
    LOD 200
    Cull Back
    CGPROGRAM
    #pragma surface surf Lambert
    #pragma target 3.0

    sampler2D _MainTex;

    struct Input {
        float2 uv_MainTex;
    };

    void surf(Input IN, inout SurfaceOutput o) {
        float4 c = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = c.rgb;
        o.Alpha = c.a;
    }
    ENDCG
}
Fallback "Diffuse"
}
