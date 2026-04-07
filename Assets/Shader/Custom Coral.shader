Shader "Custom/Coral" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Gloss ("Gloss", Range(0,1)) = 0.3
}
SubShader {
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    LOD 200
    Blend SrcAlpha OneMinusSrcAlpha
    CGPROGRAM
    #pragma surface surf Standard fullforwardshadows alpha:fade
    #pragma target 3.0

    sampler2D _MainTex;
    fixed4 _Color;
    float _Gloss;

    struct Input {
        float2 uv_MainTex;
    };

    void surf(Input IN, inout SurfaceOutputStandard o) {
        fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        o.Albedo = col.rgb;
        o.Metallic = 0;
        o.Smoothness = _Gloss;
        o.Alpha = col.a;
    }
    ENDCG
}
Fallback "Diffuse"
}
