Shader "Unlit/Chapter11_Distortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("NoiseTexture",2D) = "white" {}
        _Color ("Color",Color) = (1, 1, 1, 1)
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
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float _Frequency;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed2 disp = tex2D(_NoiseTex,i.uv).xy;
                disp = ((disp * 2) - 1) * _Frequency;
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * _Color;
            }
            ENDCG
        }
    }
}
