Shader "Unlit/EdgeFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_TexelSize ("Texel Size", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
			half4 _TexelSize;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = 0;

			color += tex2D(_MainTex, i.uv + half2(-_TexelSize.x, -_TexelSize.y)) * -1.0;
			color += tex2D(_MainTex, i.uv + half2(			  0, -_TexelSize.y)) * -1.0;
			color += tex2D(_MainTex, i.uv + half2(+_TexelSize.x, -_TexelSize.y)) * -1.0;

			color += tex2D(_MainTex, i.uv + half2(-_TexelSize.x, _TexelSize.y)) * -1.0;
			color += tex2D(_MainTex, i.uv + half2(0, _TexelSize.y)) * -1.0;
			color += tex2D(_MainTex, i.uv + half2(+_TexelSize.x, _TexelSize.y)) * -1.0;

			color += tex2D(_MainTex, i.uv + half2(-_TexelSize.x, 0)) * -1.0;
			color += tex2D(_MainTex, i.uv) * 8.0;
			color += tex2D(_MainTex, i.uv + half2(+_TexelSize.x, 0)) * -1.0;

			float a = (color.r + color.g + color.b);
			color.rgb = a * a * a;
			color.a = 1;

			return color;
            }
            ENDCG
        }
    }
}
