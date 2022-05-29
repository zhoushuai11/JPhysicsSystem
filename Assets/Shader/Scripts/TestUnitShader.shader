Shader "Custom/TestUnitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            // 定义 顶点函数和片元函数的名称
            #pragma vertex vert
            #pragma fragment frag
            
            struct a2v // 模型空间到顶点函数的数据
            {
                float4 vertex : POSITION; // POSITION 语义，用模型空间的顶点坐标填充 vertex 变量
                float3 normal : NORMAL; // NORMAL 语义，用模型空间的法线方向填充 normal 变量
                float4 texcoord : TEXCOORD0; // TEXCOORD0 语义，用模型的第一套纹理坐标填充 texcoord 变量
            };

            struct v2f // 顶点函数到片元函数的数据
            {
                float4 position : SV_POSITION; // SV_POSITION 语义，表示 position 包含了顶点在裁剪空间中的位置信息
                float3 temp : COLOR0; // 保存模型空间的法线信息到 COLOR0 中 
            };
            
            v2f vert(a2v v){
                v2f f; // 声明输出结构 
                f.position = UnityObjectToClipPos(v.vertex);
                f.temp = v.normal;
                return f;
            }
            
            fixed4 frag(v2f f) : SV_Target{
                return fixed4(f.temp,0); 
            }

            ENDCG
        }
    }
}
