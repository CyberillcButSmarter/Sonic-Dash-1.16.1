Shader "Hidden/Unlit/Premultiplied Colored 1" {
Properties {
    _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
    _Color ("Tint", Color) = (1,1,1,1)
    _StencilComp ("Stencil Comparison", Float) = 8
    _Stencil ("Stencil ID", Float) = 1
    _StencilOp ("Stencil Operation", Float) = 0
    _StencilWriteMask ("Stencil Write Mask", Float) = 255
    _StencilReadMask ("Stencil Read Mask", Float) = 255
}

SubShader {
    Tags {
        "Queue"           = "Transparent"
        "IgnoreProjector" = "True"
        "RenderType"      = "Transparent"
    }
    LOD 100

    Stencil {
        Ref   1
        Comp  Equal
        Pass  Keep
        ReadMask  1
        WriteMask 0
    }

    ZWrite Off
    Cull Off
    Blend One OneMinusSrcAlpha

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
            fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
            return col;
        }
        ENDCG
    }
}

FallBack "Unlit/Transparent"
}
