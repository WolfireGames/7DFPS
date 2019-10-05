Shader "Self-Illumin/Diffuse" {
	Properties {
		_Color ("Main Color", Color) = (.15,.15,.15,.15)
		_Illum ("Illumin (A)", Color) = (1,1,1,1)
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _Illum;
		fixed4 _Color;

		struct Input {
			float2 uv;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;
			o.Emission = _Illum;
			o.Alpha = _Color.a;
		}
		ENDCG
	} 
	FallBack "Self-Illumin/VertexLit"
}