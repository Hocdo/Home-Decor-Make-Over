// Upgrade NOTE: upgraded instancing buffer 'PerDrawSprite' to new syntax.

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Shining" {
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_ShininessColor ("Shininess Color",Color) = (1,1,1,1)
		_Width("Width",Float) = 0.1
		_TimeController ("Time",Range(0,1)) = 0
		_Intensity ("Intensity",Range(0,1)) = 0
	

		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

	}
	SubShader 
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"

		   #ifdef UNITY_INSTANCING_ENABLED

 		   UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
		        // SpriteRenderer.Color while Non-Batched/Instanced.
		        fixed4 unity_SpriteRendererColorArray[UNITY_INSTANCED_ARRAY_SIZE];
		        // this could be smaller but that's how bit each entry is regardless of type
		        float4 unity_SpriteFlipArray[UNITY_INSTANCED_ARRAY_SIZE];
   	   	   UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

   		   #define _RendererColor unity_SpriteRendererColorArray[unity_InstanceID]
  		   #define _Flip unity_SpriteFlipArray[unity_InstanceID]

		   #endif // instancing

		   CBUFFER_START(UnityPerDrawSprite)
           #ifndef UNITY_INSTANCING_ENABLED
               fixed4 _RendererColor;
               float4 _Flip;
           #endif
   		   	   float _EnableExternalAlpha;
		   CBUFFER_END

			sampler2D _MainTex;
			sampler2D _AlphaTex;


			fixed4 _Color;
			float4 _ShininessColor;
			float _Width;
			float _TimeController;
			float _Intensity;

			struct Interpolater {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float4 color    : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color    : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert (Interpolater i)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID (i);
   				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

   				#ifdef UNITY_INSTANCING_ENABLED
   					 i.pos.xy *= _Flip.xy;
				#endif

				o.pos = UnityObjectToClipPos (i.pos);
				o.uv = i.uv;
				o.color = i.color * _Color * _RendererColor;

				#ifdef PIXELSNAP_ON
    				o.pos = UnityPixelSnap (i.pos);
    			#endif

				return o;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
  				fixed4 color = tex2D (_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
   					fixed4 alpha = tex2D (_AlphaTex, uv);
    				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
				#endif
    			return color;
			}

			float4 frag (v2f i) : COLOR
			{
				fixed4 c = SampleSpriteTexture (i.uv) * i.color;
   				c.rgb *= c.a;
				
				float value = (i.uv.x  + i.uv.y );


				if(value < (2 * _TimeController +_Width ) && value > (2 * _TimeController  - _Width) )
				{
					float blur = (1 - abs( (2 * _TimeController) - value)/ _Width) * _Intensity;
					return lerp(c,_ShininessColor * c.a,blur) ;
				}

				return c;
			}

			
			ENDCG
		}
	}
	FallBack "Diffuse"
}