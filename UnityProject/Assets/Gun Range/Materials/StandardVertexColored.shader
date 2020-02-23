 Shader "Custom/Standard (Vertex colored)" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
         _Glossiness ("Smoothness", Range(0,1)) = 0.5
         _Metallic ("Metallic", Range(0,1)) = 0.0
     }
     SubShader {
         Tags { "RenderType"="Opaque" }
         LOD 200
         Cull Off
         CGPROGRAM
         #pragma surface surf Standard vertex:vert 
         #pragma target 3.0
         //fullforwardshadows
         
         struct Input {
             float2 uv2_MainTex;
             float3 vertexColor;
         };
         
         struct v2f {
           float4 pos : SV_POSITION;
           fixed4 color : COLOR;
           half2 texcoord : TEXCOORD0;           
         };
 
         void vert (inout appdata_full v, out Input o)
         {
             UNITY_INITIALIZE_OUTPUT(Input,o);
             o.vertexColor = v.color;

         } 
 
         sampler2D _MainTex;
 
         half _Glossiness;
         half _Metallic;
         fixed4 _Color;
 
         void surf (Input IN, inout SurfaceOutputStandard o) 
         {
             fixed4 c = tex2D (_MainTex, IN.uv2_MainTex) * _Color;
			 o.Albedo = IN.vertexColor + (c.rgb * c.a);
             o.Metallic = _Metallic;
             o.Smoothness = _Glossiness;
         }
         
         ENDCG
     } 
     FallBack "Diffuse"
 }