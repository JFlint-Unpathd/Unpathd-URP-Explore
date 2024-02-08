Shader "Custom/VertexAnimationTools/UnlitVertexColorAOTransparent"
{
	Properties
	{
		 _ShadowStrength ("Shadow Strength", Range (0, 5)) = 1
	}
	SubShader
	{
		Tags {  "Queue"="Transparent"   "RenderType"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		
		ZWrite Off
		//Blend One OneMinusSrcAlpha 
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#define UNITY_SHADER_NO_UPGRADE 1 
			#include "UnityCG.cginc"
			fixed _ShadowStrength;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				//float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				//float2 uv : TEXCOORD0;
				float4 color : COLOR;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex =  mul(UNITY_MATRIX_MVP, v.vertex);
 
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = float4(0,0,0, 1-(i.color.a*_ShadowStrength) );
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
