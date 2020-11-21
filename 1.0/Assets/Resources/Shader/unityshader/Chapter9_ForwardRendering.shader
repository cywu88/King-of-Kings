// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "UnityShader/Chapter9_ForwardRendering"
{
	Properties
	{
		_Color("Color",Color) = (1,1,1,1)
		_SpecularColor("SpecularColor",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(8.0,256)) = 20
	}
	
		SubShader
	{
		Pass
		{
		//定义光照模式，只有正确定光照模式，才能得到一些Unity内置光照变量
		Tags{"LightMode" = "ForwardBase"}

		CGPROGRAM
		#pragma multi_compile_fwdbase
		//#pragma multi_compile_fwdbase指令保证Shader中使用光照衰减变量可以被正确赋值
 
			#pragma vertext vert 
			#prgame fragment frag

			#inlcude "Lighting.cginc"

			fixed4 _Diffuse;
			
			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;

			};

			struct v2f {
				float pos : SV_POSITION;
				float3 worldPos:TEXCOORD0;
				float3 worldNormal:TEXCOORD1;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);

				return o;

			}

			fixed4 frag(v2f i) :SV_Target{
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed3 diffuse = _LightColor0.rgb*_Color.rgb*max(0,dot(worldNormal,worldLightDir));
				fixed3 halfDir = normalize(worldViewDir + worldLightDir);
				fixed3 specularColor = _LightColor0.rgb*_SpecularColor.rgb*pow(saturate(dot(worldNormal,halfDir)),_Gloss);
			
				
				fixed atten = 1.0;
				//Unity选择最亮的平行光在BasePass中处理，平行光的衰减值设为1，认为没有衰减
				return fixed4(ambient + (diffuse + specularColor)*atten, 1.0);
			
			
			}

		ENDCG

		}


		Pass{
		Tags{"LightMode" = "ForwardAdd"}
		Blend One One
		CGPROGRAM
				#pragma multi_compile_fwdadd
				//#pragma multi_compile_fwdadd指令保证Shader中访问正确的光照变量
				#pragma vertex vert
				#pragma fragment frag 

				#include "Lighting.cginc"
				//需要引入该文件，才能正确的访问到_LightMatrix0光照矩阵，对坐标进行转换
				//以便对光照衰减纹理进行采样
				#include "AutoLight.cginc"

				fixed4 _Color;
				fixed4 _SpecularColor;
				float   _Gloss;

				struct a2v {
					float4 vertex:POSITION;
					float3 normal:NORMAL;
				};
				struct v2f {
					float4 pos:SV_POSITION;
					float3 worldPos:TEXCOORD0;
					float3 worldNormal:TEXCOORD1;
				};

				v2f vert(a2v v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
					o.worldNormal = UnityObjectToWorldNormal(v.normal);

					return o;
				}

				fixed4 frag(v2f i) :SV_Target{
					//Additional中去掉BasePass中的环境光、自发光、逐顶点光照和SH光照部分
					//AddPass中处理的光源可能是非最亮平行光、点光源、聚光灯，因此计算光源的
					//位置、方向、颜色、强度和衰减时，需要根据光源类型分别进行计算
					#ifdef USING_DIRECTIONAL_LIGHT
						fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
					#else
						fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
					#endif
					//上述过程先判断处理的光源是否为平行光，如果是平行光，渲染引擎会定义USING_DIRECTIONAL_LIGHT
					//若没有定义则说明不是平行光，光源位置通过运算得到
					fixed3 worldNormal = normalize(i.worldNormal);
					fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

					fixed3 diffuse = _LightColor0.rgb*_Color.rgb*max(0,dot(worldNormal,worldLightDir));
					fixed3 halfDir = normalize(worldViewDir + worldLightDir);
					fixed3 specularColor = _LightColor0.rgb*_SpecularColor.rgb*pow(saturate(dot(worldNormal,halfDir)),_Gloss);

					//处理衰减过程
					//针对其他光源类型，Unity使用一张纹理作为查找表，来得到片元着色器中的光照衰减值
					//先得到光源空间下的坐标，使用该坐标进行采样得到衰减值
					#ifdef USING_DIRECTIONAL_LIGHT
						fixed  atten = 1.0;
					#else 
						float3 lightCoord = mul(unity_WorldToLight,float4(i.worldPos,1)).xyz;
						fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
					#endif		
					return fixed4((diffuse + specularColor)*atten,1.0);
				}
		ENDCG
	}

    }
    FallBack "Diffuse"
}
