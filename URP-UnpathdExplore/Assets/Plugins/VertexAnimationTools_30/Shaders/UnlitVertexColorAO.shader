Shader "Custom/VertexAnimationTools/UnlitVertexColorAO"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_AOStrength ("AO Strength", Float) = 1 
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#define UNITY_SHADER_NO_UPGRADE 1 
			#include "UnityCG.cginc"

			fixed _AOStrength;
			fixed4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
 
			};

			struct v2f
			{
 
				float4 color : COLOR;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};
 
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				_Color.a = i.color.a * _AOStrength;
				float4 col =  _Color * _Color.a  ;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
