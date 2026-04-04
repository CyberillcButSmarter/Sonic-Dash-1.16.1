Shader "Sonic Dash/Glass" {
Properties {
    _Cubemap ("_Cubemap", CUBE) = "black" {}
    _ReflectionMultiplier ("_ReflectionMultiplier", Float) = 0.5
}
SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    LOD 200

    CGPROGRAM
    #pragma surface surf Lambert alpha:fade
    #pragma target 3.0

    samplerCUBE _Cubemap;
    float       _ReflectionMultiplier;

    struct Input {
        float3 worldRefl;
    };

    void surf(Input IN, inout SurfaceOutput o) {
        float4 reflection = texCUBE(_Cubemap, IN.worldRefl);
        o.Albedo = reflection.rgb * _ReflectionMultiplier;
        o.Alpha  = _ReflectionMultiplier;
    }
    ENDCG
}
Fallback "Transparent/Diffuse"
}
