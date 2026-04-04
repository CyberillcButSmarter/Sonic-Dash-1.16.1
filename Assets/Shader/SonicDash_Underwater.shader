Shader "Sonic Dash/Underwater" {
Properties {
    _Diffuse ("_Diffuse", 2D) = "gray" {}
    _Color1 ("_Color1", Color) = (0,0.309804,0.678431,1)
    _EmissionStrength ("_EmissionStrength", Range(0,1)) = 1.0
}
SubShader {
    Tags { "RenderType" = "Opaque" }
    LOD 200

    CGPROGRAM
    #pragma surface surf Lambert
    #pragma target 3.0

    sampler2D _Diffuse;
    float4    _Color1;
    float     _EmissionStrength;

    struct Input {
        float2 uv_Diffuse;
    };

    void surf(Input IN, inout SurfaceOutput o) {
        float4 c   = tex2D(_Diffuse, IN.uv_Diffuse);
        o.Albedo   = c.rgb;
        // Underwater ambient tint as emission - set _EmissionStrength to 0
        // on tunnel floor materials to prevent them turning blue
        o.Emission = _Color1.rgb * _Color1.a * _EmissionStrength;
        o.Alpha    = c.a;
    }
    ENDCG
}
Fallback "Diffuse"
}
