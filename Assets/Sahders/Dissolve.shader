Shader "Custom/Dissolve"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [HDR]_EmissionColor ("EmissionColor", Color) = (1,1,1,1)
        _Dissolve ("Dissolve", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Progression("Progreesion",Range(0,1)) =0
        _Alpha("Alpha",Range(0,1)) =0
        _Grid("Grid",Float) =100
        _Pow("Pow",Float)=1
        _Offset("Offset",Float)=1
        [Toggle]_zDirection("ZDirection",Float)=0
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        CGPROGRAM
        //#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff
        #pragma multi_compile_instancing
        #pragma surface surf Standard vertex:vert alphatest:_Cutoff
        #pragma target 3.5
        sampler2D _Dissolve;

        struct Input
        {
            float2 uv_MainTex;
            float3 objPos;
            float3 wPos;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO // Stereo rendering output
            
        };
        
        

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;
        float _Progression;
        float _Alpha;
        float _Movement;
        float _Grid;
        float _Pow;
        float _Offset;
        float _zDirection;

        
        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        void vert (inout appdata_full v, out Input o)
        {
            UNITY_SETUP_INSTANCE_ID(v);                
            UNITY_INITIALIZE_OUTPUT(Input, o);           
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);  
            o.objPos = v.vertex;
            o.wPos=mul(unity_ObjectToWorld,v.vertex);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 cutOff= tex2D (_Dissolve, IN.uv_MainTex);
            float3 pos= IN.objPos;
            float stripesY;
             float stripesX=step(0.9,frac(IN.wPos.x*_Grid+_Movement));
            if(_zDirection==1)
            {
                cutOff-=pow(_Progression+_Offset,_Pow) *pos.z;
                stripesY= step(0.9,frac(IN.wPos.z*_Grid+_Movement));
            }
            else
            {
                cutOff-=pow(_Progression+_Offset,_Pow) *pos.y;
                stripesY= step(0.9,frac(IN.wPos.y*_Grid+_Movement));
            }
            //_Movement+=_Time.y;
            o.Albedo = _Color;;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = cutOff.r*_Alpha*_Color.w;
            o.Emission=_EmissionColor*stripesY+_EmissionColor*stripesX;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
