Shader "Unlit/TextureColorTransparent"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Range("Range", Range(0, 10)) = 0.5
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "Queue" = "Transparent"}

			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Color;
				float _Range;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					return o;
				}

				float2 Distort(float2 p)
				{
					float theta = atan2(p.y, p.x);
					float radius = length(p);
					radius = pow(radius, _Range);
					p.x = radius * cos(theta);
					p.y = radius * sin(theta);
					return 0.5 * (p + 1.0);
				}

				fixed4 frag(v2f i) : SV_Target
				{
					/*float2 xy = 2.0 * i.uv - 1.0;

					float2 uv;
					float d = length(xy);

					  if (d < 1)
					  {
						uv = Distort(xy);
					  }
					  else
					  {
						uv = i.uv;
					  }*/

					fixed4 col = tex2D(_MainTex, i.uv) * _Color;

					return col;
				}
				ENDCG
			}
		}
}
