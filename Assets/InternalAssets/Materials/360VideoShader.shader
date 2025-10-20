Shader "Custom/360VideoInside"
{
    Properties
    {
        _MainTex ("Equirectangular Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0.0, 2.0)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        // Cull Front to see the inside of the sphere
        Cull Front
        
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
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Get direction from center to fragment
                float3 dir = normalize(i.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz);
                
                // Convert direction to spherical coordinates
                float theta = atan2(dir.z, dir.x);
                float phi = acos(dir.y);
                
                // Convert to UV coordinates for equirectangular mapping
                float u = (theta + 3.14159) / (2.0 * 3.14159);
                float v = phi / 3.14159;
                
                // Sample texture
                fixed4 col = tex2D(_MainTex, float2(u, v));
                col.rgb *= _Brightness;
                
                return col;
            }
            ENDCG
        }
    }
}