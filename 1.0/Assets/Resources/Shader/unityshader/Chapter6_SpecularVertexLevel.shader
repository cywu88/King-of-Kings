// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//逐顶点高光反射实现
//Cdiffuse = (Clight*Mdiffuse)max(0,n*I)
//Cspecular = (Clight*mSpecular)max(0,v*r) Mglass
Shader "UnityShader/Chapter6_SpecularVertexLevel"
{
	Properties
	{
		_Diffuse("DiffuseColor",Color) = (1.0,1.0,1.0,1.0)
		_Specular("SpecualrColor",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(8.0,256)) = 20
	}
	
	SubShader
	{
		Pass
		{
		//定义光照模式，只有正确定光照模式，才能得到一些Unity内置光照变量
		Tags{"LightMode" = "ForwardBase"}

		CGPROGRAM
			#pragma vertext vert 
			#prgame fragment frag

		//使用Unity的内置包含文件，使用其内置变量
			#inlcude "Lighting.cginc"

			fixed4 _Diffuse;
			fixed4 _Specular;
			float _Gloss;

			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;
			};

			struct v2f {
				float4  pos : SV_POSITION;
				fixed3 color : COLOR;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				//通过模型到世界的转置逆矩阵计算得到世界空间内的顶点法向方向
				//（v.normal存储的是模型空间内的顶点法线方向）
				fixed3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject));
				//得到世界空间内的光线方向
				//fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

				//根据Lambert定律计算漫反射 
				//saturate函数将所得矢量或标量的值限定在[0,1]之间
				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLightDir))
					 
				//通过CG提供的reflect(i,n)提供的函数计算反射方向
				fixed3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));
				//计算世界空间下的观察方向
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
				//计算高光反射部分
				fixed3 specular = _LightColor0.rgb*_Specular.rgb*pow(saturate(dot(viewDir, reflectDir)), _Gloss);

				//o.color = diffuse + ambient;
				o.color = ambient + diffuse + specular;
				return o;
			}

			fixed4 frag(v2f i) :SV_Target{
				return fixed4(i.color,1.0);
			}

		ENDCG

		}
    }
    FallBack "Diffuse"
}

//可以看出，逐顶点的高光反射在高光部分出现非常明显的锯齿不连续现象，
//由于高光计算部分pow（）是非线性，因此顶点插值后的结果不理想


//https://zhuanlan.zhihu.com/p/31229044