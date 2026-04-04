Shader "FX/Water" {
Properties {
	_WaveScale ("Wave scale", Range(0.02,0.15)) = 0.063
	_ReflDistort ("Reflection distort", Range(0,1.5)) = 0.44
	_RefrDistort ("Refraction distort", Range(0,1.5)) = 0.4
	_RefrColor ("Refraction color", Color) = (0.34,0.85,0.92,1)
	_Fresnel ("Fresnel (A) ", 2D) = "gray" {}
	_BumpMap ("Normalmap ", 2D) = "bump" {}
	WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
	_ReflectiveColor ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
	_ReflectiveColorCube ("Reflective color cube (RGB) fresnel (A)", CUBE) = "" {}
	_HorizonColor ("Simple water horizon color", Color) = (0.172,0.463,0.435,1)
	_MainTex ("Fallback texture", 2D) = "" {}
	_ReflectionTex ("Internal Reflection", 2D) = "" {}
	_RefractionTex ("Internal Refraction", 2D) = "" {}
}

// -----------------------------------------------------------
// SubShader 1: Pixel-lit with reflection AND refraction
// Used when WaterMode = "Refractive" (highest quality)
// -----------------------------------------------------------
SubShader {
	Tags { "WaterMode"="Refractive" "RenderType"="Opaque" }
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog
		#pragma target 3.0

		#include "UnityCG.cginc"

		// Set from script (WaterBase.cs) each frame
		uniform float4 _WaveScale4;
		uniform float4 _WaveOffset;

		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f {
			float4 pos      : SV_POSITION;
			float4 ref      : TEXCOORD0;  // screen-space for reflection/refraction
			float2 bumpuv0  : TEXCOORD1;  // first bump map UV
			float2 bumpuv1  : TEXCOORD2;  // second bump map UV (offset for variety)
			float3 viewDir  : TEXCOORD3;  // world-space view direction
			UNITY_FOG_COORDS(4)
		};

		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			// Animate two independent wave UV sets using world XZ position
			float4 wpos = mul(unity_ObjectToWorld, v.vertex);
			float4 temp = wpos.xzxz * _WaveScale4 + _WaveOffset;
			o.bumpuv0 = temp.xy;
			o.bumpuv1 = temp.wz;

			// View direction in world space (xzy swizzle matches Unity's convention)
			o.viewDir.xzy = WorldSpaceViewDir(v.vertex);

			// Screen-space position for projective texture lookup
			o.ref = ComputeNonStereoScreenPos(o.pos);

			UNITY_TRANSFER_FOG(o, o.pos);
			return o;
		}

		sampler2D _ReflectionTex;
		sampler2D _RefractionTex;
		sampler2D _Fresnel;
		sampler2D _BumpMap;
		float _ReflDistort;
		float _RefrDistort;

		half4 frag(v2f i) : SV_Target {
			i.viewDir = normalize(i.viewDir);

			// Sample and average two bump map layers for animated surface detail
			half3 bump1 = UnpackNormal(tex2D(_BumpMap, i.bumpuv0)).rgb;
			half3 bump2 = UnpackNormal(tex2D(_BumpMap, i.bumpuv1)).rgb;
			half3 bump  = (bump1 + bump2) * 0.5;

			// Fresnel factor: look up based on view-normal dot product
			half fresnel = tex2D(_Fresnel, float2(dot(i.viewDir, bump), 0.0)).a;

			// Reflection: distort screen UV by bump normal
			float4 uv1 = i.ref;
			uv1.xy += bump.xy * _ReflDistort;
			half4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(uv1));

			// Refraction: distort screen UV in opposite direction
			float4 uv2 = i.ref;
			uv2.xy -= bump.xy * _RefrDistort;
			half4 refr = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(uv2));

			// Blend reflection and refraction by fresnel
			half4 color = lerp(refr, refl, fresnel);

			UNITY_APPLY_FOG(i.fogCoord, color);
			return color;
		}
		ENDCG
	}
}

// -----------------------------------------------------------
// SubShader 2: Pixel-lit with reflection ONLY (no refraction)
// Used when WaterMode = "Simple"
// -----------------------------------------------------------
SubShader {
	Tags { "WaterMode"="Simple" "RenderType"="Opaque" }
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog

		#include "UnityCG.cginc"

		uniform float4 _WaveScale4;
		uniform float4 _WaveOffset;

		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f {
			float4 pos      : SV_POSITION;
			float4 ref      : TEXCOORD0;
			float2 bumpuv0  : TEXCOORD1;
			float2 bumpuv1  : TEXCOORD2;
			float3 viewDir  : TEXCOORD3;
			UNITY_FOG_COORDS(4)
		};

		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			float4 wpos = mul(unity_ObjectToWorld, v.vertex);
			float4 temp = wpos.xzxz * _WaveScale4 + _WaveOffset;
			o.bumpuv0 = temp.xy;
			o.bumpuv1 = temp.wz;

			o.viewDir.xzy = WorldSpaceViewDir(v.vertex);
			o.ref = ComputeNonStereoScreenPos(o.pos);

			UNITY_TRANSFER_FOG(o, o.pos);
			return o;
		}

		sampler2D _ReflectionTex;
		sampler2D _ReflectiveColor;
		sampler2D _Fresnel;
		sampler2D _BumpMap;
		float     _ReflDistort;
		half4     _RefrColor;
		half4     _HorizonColor;

		half4 frag(v2f i) : SV_Target {
			i.viewDir = normalize(i.viewDir);

			half3 bump1 = UnpackNormal(tex2D(_BumpMap, i.bumpuv0)).rgb;
			half3 bump2 = UnpackNormal(tex2D(_BumpMap, i.bumpuv1)).rgb;
			half3 bump  = (bump1 + bump2) * 0.5;

			// Distorted reflection lookup
			float4 uv1 = i.ref;
			uv1.xy += bump.xy * _ReflDistort;
			half4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(uv1));

			// Try _ReflectiveColor first (RGB=water color, A=fresnel).
			// If it is unassigned Unity returns a grey texel (all 0.5).
			// In that case fall back: use _RefrColor/_HorizonColor for the
			// water body and compute fresnel from the dedicated _Fresnel LUT.
			half  NdotV   = saturate(dot(i.viewDir, bump));
			half4 rcol    = tex2D(_ReflectiveColor, float2(NdotV, 0.0));
			half  fresnel = tex2D(_Fresnel, float2(NdotV, 0.0)).a;

			// Determine whether _ReflectiveColor is actually assigned:
			// an unassigned 2D defaults to "gray" (0.5,0.5,0.5,0.5 in linear).
			// Use the fresnel LUT result when rcol looks like that default.
			half  rcAssigned = saturate((abs(rcol.r - 0.5) + abs(rcol.g - 0.5) + abs(rcol.b - 0.5)) * 10.0);
			half4 waterColor = lerp(lerp(_HorizonColor, _RefrColor, NdotV), rcol, rcAssigned);
			half  fresnelW   = lerp(fresnel, rcol.a, rcAssigned);

			half4 color = lerp(waterColor, refl, fresnelW);

			UNITY_APPLY_FOG(i.fogCoord, color);
			return color;
		}
		ENDCG
	}
}

// -----------------------------------------------------------
// SubShader 3: Vertex-lit, cubemap reflection
// Used on hardware that can't do projective textures
// -----------------------------------------------------------
SubShader {
	Tags { "RenderType"="Opaque" }
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog

		#include "UnityCG.cginc"

		uniform float4 _WaveScale4;
		uniform float4 _WaveOffset;

		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f {
			float4 pos      : SV_POSITION;
			float2 bumpuv0  : TEXCOORD0;
			float2 bumpuv1  : TEXCOORD1;
			float3 viewDir  : TEXCOORD2;
			UNITY_FOG_COORDS(3)
		};

		v2f vert(appdata v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);

			float4 wpos = mul(unity_ObjectToWorld, v.vertex);
			float4 temp = wpos.xzxz * _WaveScale4 + _WaveOffset;
			o.bumpuv0 = temp.xy;
			o.bumpuv1 = temp.wz;

			o.viewDir.xzy = WorldSpaceViewDir(v.vertex);

			UNITY_TRANSFER_FOG(o, o.pos);
			return o;
		}

		samplerCUBE _ReflectiveColorCube;
		sampler2D   _BumpMap;
		half4       _RefrColor;

		half4 frag(v2f i) : SV_Target {
			i.viewDir = normalize(i.viewDir);

			half3 bump1 = UnpackNormal(tex2D(_BumpMap, i.bumpuv0)).rgb;
			half3 bump2 = UnpackNormal(tex2D(_BumpMap, i.bumpuv1)).rgb;
			half3 bump  = (bump1 + bump2) * 0.5;

			// Reflect view direction around the perturbed normal for cubemap lookup
			half3 reflDir = reflect(i.viewDir, bump);
			half4 water   = texCUBE(_ReflectiveColorCube, reflDir);

			half4 color = lerp(_RefrColor, water, water.a);

			UNITY_APPLY_FOG(i.fogCoord, color);
			return color;
		}
		ENDCG
	}
}

// -----------------------------------------------------------
// SubShader 4: Minimal CGPROGRAM fallback
// Samples _MainTex tinted toward _HorizonColor.
// No render textures needed — safe on all Unity 2017.4 targets
// (Metal, GL Core, GLES2/3, WebGL) which have no fixed-function pipeline.
// -----------------------------------------------------------
SubShader {
	Tags { "RenderType"="Opaque" }
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float4    _MainTex_ST;
		half4     _HorizonColor;
		half4     _RefrColor;

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
			o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
			UNITY_TRANSFER_FOG(o, o.pos);
			return o;
		}

		half4 frag(v2f i) : SV_Target {
			half4 tex   = tex2D(_MainTex, i.uv);
			// Blend sampled texture toward the refraction/horizon color
			half4 color = lerp(_HorizonColor, tex * _RefrColor, tex.a);
			UNITY_APPLY_FOG(i.fogCoord, color);
			return color;
		}
		ENDCG
	}
}

}
