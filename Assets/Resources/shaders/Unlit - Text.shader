Shader "Unlit/Text" {
Properties {
    _MainTex ("Alpha (A)", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)
}

SubShader {
    Tags {
        "Queue"           = "Transparent"
        "IgnoreProjector" = "True"
        "RenderType"      = "Transparent"
    }
    LOD 100

    ZWrite Off
    Cull Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4    _MainTex_ST;
        fixed4    _Color;

        struct appdata_t {
            float4 vertex   : POSITION;
            float2 texcoord : TEXCOORD0;
            fixed4 color    : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f {
            float4 vertex   : SV_POSITION;
            float2 texcoord : TEXCOORD0;
            fixed4 color    : COLOR;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert (appdata_t v) {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.vertex   = UnityObjectToClipPos(v.vertex);
            o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
            o.color    = v.color * _Color;
            return o;
        }

        fixed4 frag (v2f i) : SV_Target {
            // Texture is alpha-only; vertex color supplies the RGB tint
            fixed4 col = i.color;
            col.a *= tex2D(_MainTex, i.texcoord).a;
            return col;
        }
        ENDCG
    }
}

FallBack "Unlit/Transparent"
}
