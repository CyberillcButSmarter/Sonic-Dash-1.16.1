Shader "Sonic Dash/VertexAlphaColor" {
    Properties {
        _MainColor ("_MainColor", Color) = (0.0823529, 0.52549, 0.827451, 1)
    }
    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert alpha:fade
        #pragma target 3.0

        fixed4 _MainColor;

        struct Input {
            float4 vertexColor;
        };

        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexColor = v.color;
        }

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 c = _MainColor * IN.vertexColor;
            o.Albedo = c.rgb;
            o.Alpha = c.a * IN.vertexColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}