Shader "Unlit/Highlight"
{
    Properties
    {
        _MainTex("MainTex",2D)= "white"{}
       _Color("Color",Color)= (1,1,1,1)
        _OutlineColor("OutlineColor",Color) =(1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" 
             "Queue" = "Transparent"
        }
        
        LOD 100

        Pass
        {
            ZTest Always
            ZWrite on
            
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO 
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); 
                UNITY_INITIALIZE_OUTPUT(v2f, o); 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); 
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv= TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col= tex2D(_MainTex,i.uv);
                float4 finalColor=lerp(_Color,_OutlineColor,col.r);
                return col.w* finalColor;
            }
            ENDCG
        }
    }
}
