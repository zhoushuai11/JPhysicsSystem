
Shader "Custom/DiffusePixel"
{
    Properties{
        _Diffuse("Diffuse",Color) = (1,1,1,1)
        _Range("Range",Range(-1,1)) = -1
    }
    
    SubShader{
        Pass{
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            
            fixed4 _Diffuse;
            float _Range;
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                fixed3 worldNormal : TEXCOORD0;
            };
            
            v2f vert(a2v v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject); 
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target{
                //Get ambient term
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                //Transform the normal fram object space to world space
                fixed3 worldNormal = normalize(i.worldNormal); 
                //Get the light direction in world space
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                //Compute diffuse term
                fixed3 halfLambert = dot(worldNormal,worldLightDir) * 0.5 + 0.5;
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * halfLambert;
                fixed3 color = ambient + diffuse;
                return fixed4(color,1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
