// From https://bitbucket.org/UnityUIExtensions/unity-ui-extensions
// BSD license https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/License

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UIShader"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref 0
				Comp always
				Pass keep
				ReadMask 255
				WriteMask 255
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Always
			Fog { Mode Off }
			BlendOp Add
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGBA

			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					half2 texcoord  : TEXCOORD0;
				};

				fixed4 _Color;


				float sRGB2linear(float v) {
					if (v <= 0.04045) {
						return v / 12.92;
					} else {
						return pow((v + 0.055) / 1.055, 2.4);
					}
				}


				float3 sRGB2linear(float3 v) {
					return float3(sRGB2linear(v.x), sRGB2linear(v.y), sRGB2linear(v.z));
				}


				float4 sRGB2linear(float4 v) {
					return float4(sRGB2linear(v.xyz), v.a);
				}


				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord.x = IN.texcoord.x;
					OUT.texcoord.y = 1.0 - IN.texcoord.y;
	#ifdef UNITY_HALF_TEXEL_OFFSET
					OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
	#endif
					// Unity doesn't convert vertex input color to linear automatically, we need to do it ourself
					// Remove this if you still use gamma workflow
					OUT.color = sRGB2linear(IN.color);
					return OUT;
				}

				sampler2D _MainTex;

				fixed4 frag(v2f IN) : SV_Target
				{
					half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
					clip(color.a - 0.01);
					return color;
				}
			ENDCG
			}
		}
}
