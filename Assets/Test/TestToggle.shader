Shader "Unlit/TestToggle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle(FILL_WITH_RED)] _FillWithRed ("Fill With Red", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                #ifndef FILL_WITH_RED
                col = tex2D(_MainTex, i.uv);
                #else
                col = 1;
                #endif

                return col;
            }
            ENDCG
        }
    }
}
