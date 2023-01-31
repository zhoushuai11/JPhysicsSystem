Shader "Unlit/GpuAnimMapTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AnimMap ("AnimMap", 2D) = "white" {}
		_AnimLen("Anim Length", Float) = 0
		_AnimFrame("Anim Frame", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            // 第一步， Shader 增加变体 支持Instance
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
                // 第二步， instancID 加入顶点着色器输入结构
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                // 第三步，instancID 加入顶点着色器输出结构
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AnimMap;
            float4 _AnimMap_TexelSize;
            float _AnimLen;
            float _AnimFrame;

            v2f vert (appdata v,uint vid : SV_VertexID)
            {
                // 第四步，instanceId 在顶点的位置
                UNITY_SETUP_INSTANCE_ID(v); 
                //第五步：传递 instanceid 顶点到片元
                //UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                // 跟随时间获取当前索引 Index
                float defaultFrame = _AnimMap_TexelSize.w;
                float nowTime = frac(_Time.y / _AnimLen) * _AnimLen;
                float index = nowTime * _AnimFrame;
                
                float animX = (vid+0.5) * _AnimMap_TexelSize.x;
                half animY = index;
                // 获取并传递定点数据
                float4 pos = tex2Dlod(_AnimMap,float4(animX,animY,0,0));
                
                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //第六步：instanceid在片元的相关设置
                UNITY_SETUP_INSTANCE_ID(i);
                
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
