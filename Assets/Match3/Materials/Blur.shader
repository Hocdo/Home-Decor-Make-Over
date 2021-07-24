Shader "Unlit/Blur"
{
    Properties
    {
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
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
			fixed4 _Tint;

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
	            //col += tex2D(_MainTex, i.uv + texelSize * 5) * 0.0093;
	            //col += tex2D(_MainTex, i.uv + texelSize * 4) * 0.028002;
	            col += tex2D(_MainTex, i.uv + texelSize * 3) * 0.065984;
	            col += tex2D(_MainTex, i.uv + texelSize * 2) * 0.121703;
	            col += tex2D(_MainTex, i.uv + texelSize * 1) * 0.175713;
	            col += tex2D(_MainTex, i.uv + texelSize * 0) * 0.198596;
	            col += tex2D(_MainTex, i.uv - texelSize * 1) * 0.175713;
	            col += tex2D(_MainTex, i.uv - texelSize * 2) * 0.121703;
	            col += tex2D(_MainTex, i.uv - texelSize * 3) * 0.065984;
	            //col += tex2D(_MainTex, i.uv - texelSize * 4) * 0.028002;
	            //col += tex2D(_MainTex, i.uv - texelSize * 5) * 0.0093;

				//col *= _Tint;

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
	            //col += tex2D(_MainTex, i.uv + texelSize * 5) * 0.0093;
	            //col += tex2D(_MainTex, i.uv + texelSize * 4) * 0.028002;
	            col += tex2D(_MainTex, i.uv + texelSize * 3) * 0.065984;
	            col += tex2D(_MainTex, i.uv + texelSize * 2) * 0.121703;
	            col += tex2D(_MainTex, i.uv + texelSize * 1) * 0.175713;
	            col += tex2D(_MainTex, i.uv + texelSize * 0) * 0.198596;
	            col += tex2D(_MainTex, i.uv - texelSize * 1) * 0.175713;
	            col += tex2D(_MainTex, i.uv - texelSize * 2) * 0.121703;
	            col += tex2D(_MainTex, i.uv - texelSize * 3) * 0.065984;
	            //col += tex2D(_MainTex, i.uv - texelSize * 4) * 0.028002;
	            //col += tex2D(_MainTex, i.uv - texelSize * 5) * 0.0093;

                return col;
            }
            ENDCG
        }
    }
}
