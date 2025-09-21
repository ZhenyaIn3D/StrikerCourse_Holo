Shader "Unlit/MovingTex"
{
    Properties
    {
        _MainTex("MainTex",2D)= "white"{}
       _FirstColor("FirstColor",Color)= (1,1,1,1)
       _SecondColor("SecondColor",Color)= (1,1,1,1)
        _Speed("Speed",Float)= 0
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
            float4 _FirstColor;
            float4 _SecondColor;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); 
                UNITY_INITIALIZE_OUTPUT(v2f, o); 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); 
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv=v.uv+ _Speed.xx*_Time.y;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col= tex2D(_MainTex,i.uv);
                float4 c= lerp(_FirstColor,_SecondColor,col.r);
                return c;
            }
            ENDCG
        }
    }
}
