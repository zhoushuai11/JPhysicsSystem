Shader "Unlit/Chapter11_Texture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color",Color) = (1, 1, 1, 1)
        _Offset ("Offset",Float) = 1
        _Speed ("Speed",Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            Blend SrcAlpha One
            
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

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Offset;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 uv = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv = uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half2 uv = i.uv;
                uv.x /= _Offset;
                uv.y /= _Offset;
				float time = floor(_Time.y * _Speed);  
				uv.x += time / _Offset;
				uv.y -= time / _Offset;
            
                // sample the texture
                fixed4 col = tex2D(_MainTex, uv);
                return col * _Color;
            }
            ENDCG
        }
    }
}
