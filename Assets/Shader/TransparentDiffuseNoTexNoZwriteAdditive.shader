Shader "Sonic Dash/TransparentDiffuseNoTexNoZWriteAdditive" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _Intensity ("Glow", Range(0,4)) = 1
}
SubShader {
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    Cull Off
    ZWrite Off
    Blend One OneMinusSrcAlpha
    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0

        struct appdata {
            float4 vertex : POSITION;
            float4 color : COLOR;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
        };

        fixed4 _Color;
        float _Intensity;

        v2f vert(appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.color = v.color;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target {
            fixed4 col = i.color;
            col.rgb *= _Color.rgb * _Intensity;
            col.a *= saturate(_Color.a);
            return col;
        }
        ENDCG
    }
}
Fallback "Unlit/Transparent"
}
