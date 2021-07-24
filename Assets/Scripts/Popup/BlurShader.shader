Shader "Unlit/BlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float4 _TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texelSize = float2(_TexelSize.x, 0);
                
                fixed4 col = fixed4(0, 0, 0, 0);
	            col += tex2D(_MainTex, i.uv + texelSize * 3) * 0.0713;
	            col += tex2D(_MainTex, i.uv + texelSize * 2) * 0.1315;
	            col += tex2D(_MainTex, i.uv + texelSize * 1) * 0.1899;
	            col += tex2D(_MainTex, i.uv + texelSize * 0) * 0.2146;
	            col += tex2D(_MainTex, i.uv - texelSize * 1) * 0.1899;
	            col += tex2D(_MainTex, i.uv - texelSize * 2) * 0.1315;
	            col += tex2D(_MainTex, i.uv - texelSize * 3) * 0.0713;

                return col;
            }
            ENDCG
        }

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
            float4 _TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texelSize = float2(0, _TexelSize.y);
                
                fixed4 col = fixed4(0, 0, 0, 0);
				col += tex2D(_MainTex, i.uv + texelSize * 3) * 0.0713;
				col += tex2D(_MainTex, i.uv + texelSize * 2) * 0.1315;
				col += tex2D(_MainTex, i.uv + texelSize * 1) * 0.1899;
				col += tex2D(_MainTex, i.uv + texelSize * 0) * 0.2146;
				col += tex2D(_MainTex, i.uv - texelSize * 1) * 0.1899;
				col += tex2D(_MainTex, i.uv - texelSize * 2) * 0.1315;
				col += tex2D(_MainTex, i.uv - texelSize * 3) * 0.0713;

                return col;
            }
            ENDCG
        }
    }
}
