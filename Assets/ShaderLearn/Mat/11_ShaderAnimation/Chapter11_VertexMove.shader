Shader "Unlit/Chapter11_VertexMove"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color",Color) = (1, 1, 1, 1)
        _Speed ("Speed",Float) = 1
        _Frequency("Move Frequency",Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="ForwardBase" "DisableBatching" = "True" }
            
            ZWrite Off
            Blend SrcAlpha One
            Cull Off
            
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
            float _Speed;
            float _Frequency;

            v2f vert (appdata v)
            {
                v2f o;
                float4 offset;
                offset.yzw = float3(0.0,0.0,0.0);
                offset.x = sin(_Frequency * _Time.y) * _Speed;
                offset.y = sin(_Frequency * _Time.y) * _Speed;
                offset.z = sin(_Frequency * _Time.y) * _Speed;
                offset.w = sin(_Frequency * _Time.y) * _Speed;
                o.vertex = UnityObjectToClipPos(v.vertex + offset);
                
                
                float2 uv = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv = uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * _Color;
            }
            ENDCG
        }
    }
}
