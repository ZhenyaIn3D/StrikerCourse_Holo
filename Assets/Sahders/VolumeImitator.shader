Shader "Unlit/VolumeImitator"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Test("Test",Float)=0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

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
                float4 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Test;

            float random (float2 st) {
             return frac(sin(dot(st.xy,
                         float2(12.9898,78.233)))*43758.5453123);
}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                 fixed4 col = float4(0.925, 0.878, 0.823, 1.);
                float waveFormHeight = _WaveForm[(int)(i.uv.x * 128)];


                 col = lerp(col, float4(0.305, 0.223, 0.223, 1. ), 1. - smoothstep(0.01, 0.015, abs(i.uv.y - 0.7 - waveFormHeight) ));
                fixed4 col = tex2D(_MainTex, i.uv);
                float newPos= sin(random(_Time.y).x)*i.localPos.z;
                float4 color= lerp(float4(1,0,0,1),float4(1,1,1,1),newPos<-_Test);
                color= lerp(float4(1,1,1,1), float4(1,0,0,1), (1. - step(0.3, i.uv.y)) * step(0.0, i.uv.y));
                return col*color;
            }
            ENDCG
        }
    }
}
