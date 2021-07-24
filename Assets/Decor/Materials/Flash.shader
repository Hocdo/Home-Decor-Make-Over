Shader "Unlit/HorizontalBlend"
{
    Properties
    {
        [NoScaleOffset]_SourceTex ("Source Tex", 2D) = "white" {}
		[NoScaleOffset]_MainTex ("Main Tex", 2D) = "white" {}
		_Progress("Progress", Range(0, 1)) = 0.5
		_Brightness("Brightness", Range(1, 1.25)) = 1
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 2.0

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

            sampler2D _SourceTex;
			sampler2D _MainTex;
			float _Progress;
			float _Brightness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float x = smoothstep(i.uv.x - 0.05, i.uv.x + 0.05, _Progress);
                fixed4 col = tex2D(_MainTex, i.uv) * x + tex2D(_SourceTex, i.uv) * (1-x);

				col.rgb += (_Brightness - 1) * x;
				col.rgb *= col.a;

                return col;
            }
            ENDCG
        }
    }
}
