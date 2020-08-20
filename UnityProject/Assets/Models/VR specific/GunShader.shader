// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:32414,y:31856,varname:node_4013,prsc:2|diff-5277-OUT,spec-7930-OUT,gloss-962-OUT;n:type:ShaderForge.SFN_Code,id:7513,x:31255,y:32119,varname:node_7513,prsc:2,code:ZgBsAG8AYQB0ADMAIABsAG8AYwBhAGwATgBvAHIAbQBhAGwAIAA9ACAAcABvAHcAKABhAGIAcwAoAG0AdQBsACgAIABfAFcAbwByAGwAZAAyAE8AYgBqAGUAYwB0ACwAIABmAGwAbwBhAHQANAAoAE4AbwByAG0AYQBsAEQAaQByAGUAYwB0AGkAbwBuACwAMAApACAAKQAuAHgAeQB6AC4AcgBnAGIAKQAsAEQAZQB0AGEAaQBsACkAOwANAAoAbABvAGMAYQBsAE4AbwByAG0AYQBsACAAPQAgAGwAbwBjAGEAbABOAG8AcgBtAGEAbAAuAHIAZwBiACAALwAgACgAbABvAGMAYQBsAE4AbwByAG0AYQBsAC4AcgAgACsAIABsAG8AYwBhAGwATgBvAHIAbQBhAGwALgBnACAAKwAgAGwAbwBjAGEAbABOAG8AcgBtAGEAbAAuAGIAKQA7AA0ACgANAAoAZgBsAG8AYQB0ADMAIABsAG8AYwBhAGwAUABvAHMAaQB0AGkAbwBuACAAPQAgAG0AdQBsACgAIABfAFcAbwByAGwAZAAyAE8AYgBqAGUAYwB0ACwAIABmAGwAbwBhAHQANAAoACgAVwBvAHIAbABkAFAAbwBzAGkAdABpAG8AbgAuAHIAZwBiAC0ATwBiAGoAZQBjAHQAUABvAHMAaQB0AGkAbwBuAC4AcgBnAGIAKQAsADAAKQAgACkALgByAGcAYgAvAFMAYwBhAGwAZQA7AA0ACgBmAGwAbwBhAHQANAAgAHQAZQB4ADEAIAA9ACAAdABlAHgAMgBEACgAVABlAHgALABsAG8AYwBhAGwAUABvAHMAaQB0AGkAbwBuAC4AcgBnACkAOwANAAoAZgBsAG8AYQB0ADQAIAB0AGUAeAAyACAAPQAgAHQAZQB4ADIARAAoAFQAZQB4ACwAbABvAGMAYQBsAFAAbwBzAGkAdABpAG8AbgAuAHIAYgApADsADQAKAGYAbABvAGEAdAA0ACAAdABlAHgAMwAgAD0AIAB0AGUAeAAyAEQAKABUAGUAeAAsAGwAbwBjAGEAbABQAG8AcwBpAHQAaQBvAG4ALgBnAGIAKQA7AA0ACgANAAoAcgBlAHQAdQByAG4AIAAoAGwAbwBjAGEAbABOAG8AcgBtAGEAbAAuAGIAKgB0AGUAeAAxAC4AcgBnAGIAIAArACAAbABvAGMAYQBsAE4AbwByAG0AYQBsAC4AZwAqAHQAZQB4ADIALgByAGcAYgAgACsAIABsAG8AYwBhAGwATgBvAHIAbQBhAGwALgByACoAdABlAHgAMwAuAHIAZwBiACkAOwA=,output:2,fname:Object_TPM,width:768,height:182,input:12,input:2,input:2,input:2,input:0,input:0,input_1_label:Tex,input_2_label:WorldPosition,input_3_label:ObjectPosition,input_4_label:NormalDirection,input_5_label:Detail,input_6_label:Scale|A-6058-TEX,B-1613-XYZ,C-6775-XYZ,D-9927-OUT,E-6000-OUT,F-4238-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:6058,x:30957,y:31815,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_6058,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:6000,x:30987,y:32430,ptovrint:False,ptlb:Detail,ptin:_Detail,varname:node_6000,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:4238,x:30987,y:32527,ptovrint:False,ptlb:Scale,ptin:_Scale,varname:node_4238,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_FragmentPosition,id:1613,x:30957,y:31974,varname:node_1613,prsc:2;n:type:ShaderForge.SFN_ObjectPosition,id:6775,x:30957,y:32096,varname:node_6775,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9927,x:30987,y:32242,prsc:2,pt:False;n:type:ShaderForge.SFN_VertexColor,id:2257,x:31848,y:31752,varname:node_2257,prsc:2;n:type:ShaderForge.SFN_Add,id:8471,x:32220,y:31856,varname:node_8471,prsc:2|A-2257-RGB,B-7513-OUT;n:type:ShaderForge.SFN_Slider,id:7930,x:32031,y:32188,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:node_7930,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:962,x:32031,y:32294,ptovrint:False,ptlb:Smoothness,ptin:_Smoothness,varname:_Metallic_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Tex2d,id:9271,x:31997,y:31584,ptovrint:False,ptlb:_maintex,ptin:__maintex,varname:node_9271,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5277,x:32313,y:31713,varname:node_5277,prsc:2|A-9271-RGB,B-8471-OUT;proporder:6058-6000-4238-7930-962-9271;pass:END;sub:END;*/

Shader "Custom/GunShader" {
    Properties {
        _Texture ("Texture", 2D) = "white" {}
        _Detail ("Detail", Float ) = 1
        _Scale ("Scale", Float ) = 2
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        __maintex ("_maintex", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            float3 Object_TPM( sampler2D Tex , float3 WorldPosition , float3 ObjectPosition , float3 NormalDirection , float Detail , float Scale ){
            float3 localNormal = pow(abs(mul( unity_WorldToObject, float4(NormalDirection,0) ).xyz.rgb),Detail);
            localNormal = localNormal.rgb / (localNormal.r + localNormal.g + localNormal.b);
            
            float3 localPosition = mul( unity_WorldToObject, float4((WorldPosition.rgb-ObjectPosition.rgb),0) ).rgb/Scale;
            float4 tex1 = tex2D(Tex,localPosition.rg);
            float4 tex2 = tex2D(Tex,localPosition.rb);
            float4 tex3 = tex2D(Tex,localPosition.gb);
            
            return (localNormal.b*tex1.rgb + localNormal.g*tex2.rgb + localNormal.r*tex3.rgb);
            }
            
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform sampler2D __maintex; uniform float4 __maintex_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _Detail)
                UNITY_DEFINE_INSTANCED_PROP( float, _Scale)
                UNITY_DEFINE_INSTANCED_PROP( float, _Metallic)
                UNITY_DEFINE_INSTANCED_PROP( float, _Smoothness)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float _Smoothness_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Smoothness );
                float gloss = _Smoothness_var;
                float perceptualRoughness = 1.0 - _Smoothness_var;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float _Metallic_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Metallic );
                float3 specularColor = _Metallic_var;
                float specularMonochrome;
                float4 __maintex_var = tex2D(__maintex,TRANSFORM_TEX(i.uv0, __maintex));
                float _Detail_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Detail );
                float _Scale_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Scale );
                float3 diffuseColor = (__maintex_var.rgb*(i.vertexColor.rgb+Object_TPM( _Texture , i.posWorld.rgb , objPos.rgb , i.normalDir , _Detail_var , _Scale_var ))); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma target 3.0
            float3 Object_TPM( sampler2D Tex , float3 WorldPosition , float3 ObjectPosition , float3 NormalDirection , float Detail , float Scale ){
            float3 localNormal = pow(abs(mul( unity_WorldToObject, float4(NormalDirection,0) ).xyz.rgb),Detail);
            localNormal = localNormal.rgb / (localNormal.r + localNormal.g + localNormal.b);
            
            float3 localPosition = mul( unity_WorldToObject, float4((WorldPosition.rgb-ObjectPosition.rgb),0) ).rgb/Scale;
            float4 tex1 = tex2D(Tex,localPosition.rg);
            float4 tex2 = tex2D(Tex,localPosition.rb);
            float4 tex3 = tex2D(Tex,localPosition.gb);
            
            return (localNormal.b*tex1.rgb + localNormal.g*tex2.rgb + localNormal.r*tex3.rgb);
            }
            
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform sampler2D __maintex; uniform float4 __maintex_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float, _Detail)
                UNITY_DEFINE_INSTANCED_PROP( float, _Scale)
                UNITY_DEFINE_INSTANCED_PROP( float, _Metallic)
                UNITY_DEFINE_INSTANCED_PROP( float, _Smoothness)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float _Smoothness_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Smoothness );
                float gloss = _Smoothness_var;
                float perceptualRoughness = 1.0 - _Smoothness_var;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float _Metallic_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Metallic );
                float3 specularColor = _Metallic_var;
                float specularMonochrome;
                float4 __maintex_var = tex2D(__maintex,TRANSFORM_TEX(i.uv0, __maintex));
                float _Detail_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Detail );
                float _Scale_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Scale );
                float3 diffuseColor = (__maintex_var.rgb*(i.vertexColor.rgb+Object_TPM( _Texture , i.posWorld.rgb , objPos.rgb , i.normalDir , _Detail_var , _Scale_var ))); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
