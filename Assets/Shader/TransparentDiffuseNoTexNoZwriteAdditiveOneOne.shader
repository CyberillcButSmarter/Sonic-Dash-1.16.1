Shader "Sonic Dash/TransparentDiffuseNoTexNoZWriteAdditiveOneOne" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _Exposure ("Exposure", Range(0,2)) = 1
}
SubShader {
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    Cull Off
    ZWrite Off
    Blend One One
    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0

        struct appdata {
            float4 vertex : POSITION;
        };

        struct v2f {
            float4 pos : SV_POSITION;
        };

        fixed4 _Color;
        float _Exposure;

        v2f vert(appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            return o;
        }

        fixed4 frag(v2f i) : SV_Target {
            fixed4 col = _Color * _Exposure;
            col.a = saturate(_Color.a);
            return col;
        }
        ENDCG
    }
}
Fallback "Unlit/Transparent"
}
