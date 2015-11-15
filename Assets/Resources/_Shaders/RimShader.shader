Shader "Rim Light" {

	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader{

		Pass{
		Tags{ "LightMode" = "ForwardBase" }

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		#pragma multi_compile_fwdbase
		#include "AutoLight.cginc"
		struct appdata {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 color : COLOR;
			LIGHTING_COORDS(1, 2)
		};

		uniform float4 _MainTex_ST;
		uniform float4 _RimColor;

		v2f vert(appdata_base v) {	
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

			float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
			float dotProduct = 1 - dot(v.normal, viewDir);
			float rimWidth = 0.7;
			o.color = smoothstep(1 - rimWidth, 1.0, dotProduct);

			o.color *= _RimColor;

			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

			TRANSFER_VERTEX_TO_FRAGMENT(o);

			return o;
		}

		uniform sampler2D _MainTex;
		uniform float4 _Color;

		float4 frag(v2f i) : COLOR{
			float atten = LIGHT_ATTENUATION(i);
			float4 texcol = tex2D(_MainTex, i.uv);
			texcol *= _Color * (atten*2);
			texcol.rgb += i.color;
			return texcol;
		}

		ENDCG
		}
	}
	Fallback "Diffuse"
}