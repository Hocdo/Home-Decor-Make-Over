Shader "Unlit/LightingGem"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0, 0, 1)
        _SpecularColor("Specular Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float3 fragPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _SpecularColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.fragPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                i.normal = normalize(i.normal);

                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 diffuse = max(0.5, dot(lightDir, i.normal)) * _Color;

                float3 viewDir = normalize(float3(0, 0, -10) - i.fragPos);
                float3 reflectDir = reflect(-lightDir, i.normal);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
                float3 specular = 2 * spec * _SpecularColor;

                float3 light = diffuse + specular;

                fixed4 col = fixed4(light, 1);

                return col;
            }
            ENDCG
        }
    }
}
