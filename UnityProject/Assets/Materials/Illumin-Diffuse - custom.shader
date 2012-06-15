Shader "Self-Illumin/Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Illum ("Illumin (A)", Color) = (1,1,1,1)
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200
	
CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Illum;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Emission = _Illum;
	o.Alpha = c.a;
}
ENDCG
} 
FallBack "Self-Illumin/VertexLit"
}
