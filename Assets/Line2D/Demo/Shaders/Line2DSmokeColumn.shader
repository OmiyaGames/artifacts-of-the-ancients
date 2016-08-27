// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Line2D/AlphaBlendSmokeColumn" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Speed ("Speed Multiplier", Range (0,1)) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .01
	Cull Off Lighting Off ZWrite Off
	ColorMask RGB // Don't write on Alpha Channel
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			float _Speed;
			
			fixed4 frag (v2f i) : COLOR
			{
				
				// Color is handle by vertex color
				float4 col = i.color;
				col.a = 0;

				// Use RGBA channels additionally to create alpha
				float2 uvA = i.texcoord + float2(0.5,0);
				float2 uvR = i.texcoord + float2(0.25,0);
				float2 uvG = i.texcoord + float2(1.0,0);
				float2 uvB = i.texcoord + float2(0.75,0);
				
				uvA.x -= _Time * 0.5 * _Speed;
				uvR.x -= _Time * 2 * _Speed;
				uvG.x -= _Time * 4 * _Speed;
				uvB.x -= _Time * _Speed;
				
				float channelA = tex2D(_MainTex, uvA).a; // use for distortion
				float channelR = tex2D(_MainTex, uvR + channelA).r;
				float channelG = tex2D(_MainTex, uvG - channelA).g;
				float channelB = tex2D(_MainTex, uvB).b;
				
				
				col.a = channelR + channelG + channelB;
				col.a *= i.color.a;
				
				
				return col;
			}
			ENDCG 
		}
	}	
}
}
