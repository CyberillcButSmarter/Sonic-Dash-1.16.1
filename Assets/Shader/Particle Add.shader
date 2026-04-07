Shader "Particles/Additive" {
Properties {
    _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
    _MainTex ("Particle Texture", 2D) = "white" {}
    _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
}
SubShader {
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    ZWrite Off
    Blend One One
    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0

        struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        sampler2D _MainTex;
        fixed4 _TintColor;

        v2f vert(appdata v) {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target {
            fixed4 tex = tex2D(_MainTex, i.uv) * _TintColor;
            return tex;
        }
        ENDCG
    }
}
Fallback "Unlit/Transparent"
}
