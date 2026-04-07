Shader "Projector/Multiply" {
Properties {
    _ShadowTex ("Cookie", 2D) = "gray" {}
    _FalloffTex ("FallOff", 2D) = "white" {}
}

SubShader {
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    LOD 100
    Cull Off
    ZWrite Off
    Blend DstColor Zero

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0

        sampler2D _ShadowTex;
        sampler2D _FalloffTex;

        float4x4 unity_Projector;
        float4x4 unity_ProjectorClip;

        struct appdata {
            float4 vertex : POSITION;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            float4 uvShadow : TEXCOORD0;
            float4 uvFalloff : TEXCOORD1;
        };

        v2f vert(appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uvShadow = mul(unity_Projector, v.vertex);
            o.uvFalloff = mul(unity_ProjectorClip, v.vertex);
            return o;
        }

        fixed4 frag(v2f i) : SV_Target {
            fixed2 uvS = i.uvShadow.xy / i.uvShadow.w;
            fixed2 uvF = i.uvFalloff.xy / i.uvFalloff.w;

            fixed4 shadow = tex2D(_ShadowTex, uvS);
            fixed4 falloff = tex2D(_FalloffTex, uvF);

            fixed alpha = shadow.a * falloff.a;
            fixed3 color = shadow.rgb * falloff.rgb;

            return fixed4(color, alpha);
        }
        ENDCG
    }
}
Fallback Off
}
