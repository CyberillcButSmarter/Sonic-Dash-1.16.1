Shader "Sonic Dash/Waves_Fixed" {
Properties {
	_Diffuse1          ("_Diffuse1",          2D)    = "gray" {}
	_Color             ("_Color",             Color)  = (1,1,1,1)
	_Texture1PanSpeedX ("_Texture1PanSpeedX", Float)  = 7
	_Texture1PanSpeedY ("_Texture1PanSpeedY", Float)  = 30
	_Texture2PanSpeed  ("_Texture2PanSpeed",  Float)  = 5
}

// -----------------------------------------------------------
// SubShader 1: Two-layer animated foam/wave — alpha blended
// Layer 1 pans along (X, Y) using _Texture1PanSpeedX/Y.
// Layer 2 pans diagonally in the counter direction using
// _Texture2PanSpeed, breaking up tiling symmetry.
// The two layers are multiplied together, then tinted by
// _Color. The combined alpha drives transparency so the
// water shader underneath shows through.
// -----------------------------------------------------------
SubShader {
	Tags {
		"Queue"           = "Transparent"
		"RenderType"      = "Transparent"
		"IgnoreProjector" = "True"
	}

	// Additive blend: black pixels are invisible, bright foam adds on top.
	// This is correct for shoreline/wave foam textures (dark BG, bright highlights).
	Blend One One
	ZWrite Off
	Cull Off
	Offset -1, -1

	Pass {
		CGPROGRAM
		#pragma vertex   vert
		#pragma fragment frag
		#pragma multi_compile_fog
		#include "UnityCG.cginc"

		sampler2D _Diffuse1;
		float4    _Diffuse1_ST;
		half4     _Color;
		float     _Texture1PanSpeedX;
		float     _Texture1PanSpeedY;
		float     _Texture2PanSpeed;

		struct appdata {
			float4 vertex : POSITION;
			float2 uv     : TEXCOORD0;
		};

		struct v2f {
			float4 pos  : SV_POSITION;
			float2 uv1  : TEXCOORD0;   // layer 1 animated UV
			float2 uv2  : TEXCOORD1;   // layer 2 animated UV
			UNITY_FOG_COORDS(2)
		};

		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			// Base UV with tiling/offset from the material
			float2 uv = TRANSFORM_TEX(v.uv, _Diffuse1);

			// Layer 1: scroll along (X, Y) axes independently
			o.uv1 = uv + float2(
				_Texture1PanSpeedX * _Time.x,
				_Texture1PanSpeedY * _Time.x
			);

			// Layer 2: scroll counter-diagonally to break symmetry.
			// Negative X creates the cross-hatch look of rolling foam.
			o.uv2 = uv + float2(
				-_Texture2PanSpeed * _Time.x,
				 _Texture2PanSpeed * _Time.x
			);

			UNITY_TRANSFER_FOG(o, o.pos);
			return o;
		}

		half4 frag(v2f i) : SV_Target {
			half4 layer1 = tex2D(_Diffuse1, i.uv1);
			half4 layer2 = tex2D(_Diffuse1, i.uv2);

			// ADD the two layers (not multiply) — multiplying sub-1 values dims
			// the result. Adding accumulates foam brightness so crests are fully
			// white rather than faint. Saturate clamps to [0,1].
			half4 combined = saturate(layer1 + layer2);

			// Texture stores waves in R and G channels. Sum them for maximum
			// white coverage — saturate keeps it in range.
			half brightness = saturate(combined.r + combined.g);

			// _Color.rgb = additive tint (black = no shift, white foam shows as-is)
			// _Color.a   = overall intensity scale (1 = full, 0 = off)
			half4 color;
			color.rgb = brightness.xxx + _Color.rgb;
			color.rgb *= _Color.a > 0.0 ? _Color.a : 1.0;
			color.a    = brightness;

			UNITY_APPLY_FOG(i.fogCoord, color);
			return color;
		}
		ENDCG
	}
}

// -----------------------------------------------------------
// SubShader 2: Single-layer minimal fallback
// For very old GLES 2.0 devices with limited interpolators.
// -----------------------------------------------------------
SubShader {
	Tags {
		"Queue"           = "Transparent"
		"RenderType"      = "Transparent"
		"IgnoreProjector" = "True"
	}
	Blend One One
	ZWrite Off
	Cull Off
	Offset -1, -1

	Pass {
		CGPROGRAM
		#pragma vertex   vert
		#pragma fragment frag
		#pragma multi_compile_fog
		#include "UnityCG.cginc"

		sampler2D _Diffuse1;
		float4    _Diffuse1_ST;
		half4     _Color;
		float     _Texture1PanSpeedX;
		float     _Texture1PanSpeedY;

		struct appdata {
			float4 vertex : POSITION;
			float2 uv     : TEXCOORD0;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv  : TEXCOORD0;
			UNITY_FOG_COORDS(1)
		};

		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			float2 uv = TRANSFORM_TEX(v.uv, _Diffuse1);
			o.uv = uv + float2(
				_Texture1PanSpeedX * _Time.x,
				_Texture1PanSpeedY * _Time.x
			);
			UNITY_TRANSFER_FOG(o, o.pos);
			return o;
		}

		half4 frag(v2f i) : SV_Target {
			half4 tex  = tex2D(_Diffuse1, i.uv);
			half  brightness = saturate(tex.r + tex.g);
			half4 col;
			col.rgb = brightness.xxx + _Color.rgb;
			col.rgb *= _Color.a > 0.0 ? _Color.a : 1.0;
			col.a   = brightness;
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
		}
		ENDCG
	}
}

}
