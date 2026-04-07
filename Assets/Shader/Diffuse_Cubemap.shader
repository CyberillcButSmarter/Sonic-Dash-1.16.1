Shader "Sonic Dash/Diffuse_Lit_Cubemap" {
Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Cube ("Cubemap", CUBE) = "" {}
    _CubeIntensity ("Reflection", Range(0,1)) = 0.5
}
SubShader {
    Tags { "RenderType" = "Opaque" }
    LOD 200
    CGPROGRAM
    #pragma surface surf Lambert
    #pragma target 3.0

    sampler2D _MainTex;
    samplerCUBE _Cube;
    float _CubeIntensity;

    struct Input {
        float2 uv_MainTex;
        float3 worldRefl;
    };

    void surf(Input IN, inout SurfaceOutput o) {
        fixed4 baseCol = tex2D(_MainTex, IN.uv_MainTex);
        fixed3 reflection = texCUBE(_Cube, IN.worldRefl).rgb;
        o.Albedo = baseCol.rgb;
        o.Emission = reflection * _CubeIntensity;
        o.Alpha = baseCol.a;
    }
    ENDCG
}
Fallback "Diffuse"
}
