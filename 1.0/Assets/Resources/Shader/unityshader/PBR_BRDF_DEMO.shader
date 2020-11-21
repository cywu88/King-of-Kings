Shader "UnityShader/PBR_BRDF_DEMO"
{
	Properties
	{
		_BaseColor("本来颜色", 2D) = "white" {}
		_SpecularColor("高光颜色", Color) = (1,1,1,1)
		_Glossiness("光滑度", Range(0, 1)) = 0.5
		_Metallic("金属度", Range(0, 1)) = 0.5
		_Anisotropic("各向异性",  Range(-20,1)) = 0
		[Toggle]_USE_NDF("使用表面法线分布？", Float) = 0
		[Toggle]_USE_TOTAL("是否组装？", Float) = 0
		[Toggle]_USE_GEOMETRYSHELTER("使用几何遮蔽？", Float) = 0
		[Toggle]_USE_FRESNEL("使用菲涅尔？", Float) = 0
		[Toggle]_USE_DIFFUSE("使用漫反射？", Float) = 0
		[Toggle]_USE_SPECULAR("使用高光？", Float) = 0
		[KeywordEnum(Blinn_Phong_ndf, Phong_ndf, Beckman_ndf, Gaussian_ndf, GGX_ndf, Ward_Anisotropic_ndf)]_Specular("表面法线模型", Float) = 0
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

			#inlcude "Lighting.cginc"

			fixed4 _Diffuse;
			
			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;

			};

			struct v2f {
				float pos : SV_POSITION;
				fixed3 color : COLOR;
			};


			float PhongNormalDistribution(float RdotV, float SpecularPower, float SpecularGloss) {
				float Distribution = pow(RdotV, SpecularGloss) * SpecularPower;
				Distribution *= (2 + SpecularPower) / _2PI;
				return Distribution;
			}

			float BlinnPhongNormalDistribution(float NdotH, float SpecularPower, float SpecularGloss)
			{
				float d = pow(NdotH, SpecularGloss) * SpecularPower;
				d *= (2 + SpecularPower) / _2PI;
				return d;
			}

			float BeckmannNormalDistribution(float roughness, float NdotH)
			{
				float roughnessSqr = roughness * roughness;
				float NdotHSqr = NdotH * NdotH;
				return max(0, (1.0 / (_PI * roughnessSqr * NdotHSqr * NdotHSqr)) * exp((NdotHSqr - 1) / (roughnessSqr * NdotHSqr)));
			}

			float GaussianNormalDistribution(float NdotH, float roughness)
			{
				float t = acos(NdotH) / roughness;
				return exp(-t * t);
			}

			float GGXNormalDistribution(float NdotH, float roughness, float SpecularPower, float c)
			{
				float roughnessSqr = roughness * roughness;
				//产生更线性的变化
				//roughnessSqr = roughnessSqr * roughnessSqr;
				float NdotHSqr = NdotH * NdotH;
				return c / pow((roughnessSqr * NdotHSqr + 1 - NdotHSqr), SpecularPower);
			}


			v2f vert(a2v v){

				D = max(GGXNormalDistribution(NdotH, roughness, 1, 0.1), GGXNormalDistribution(NdotH, roughness, 2, 0.04));

				float3 brdf = diffuseColor + _SpecularColor * Rf * G * D / (4 * NdotV * NdotL);
				//显而易见，BRDF模型由漫反射、受菲涅尔定律、几何遮蔽、表面法线分布的镜面反射组成。
			
				//法线分布函数（Normal Distribution Function）是BRDF的核心要素之一，
				//从统计学上描述了微面元的法线分布情况，而开发者们有诸多NDF算法，
				//其中最为著名的可能就要数GGX NDF了。
				//下面谈及的NDF代码从【3】上找到，
				//【3】【翻译】通过Unity学习PBR算法 ①https://zhuanlan.zhihu.com/p/36705264
				//其计算方法可从
				//【1】《全局光照技术》 
				//【5】Specular highlight https://en.wikipedia.org/wiki/Specular_highlight
				//【6】Unity3d 基于物理渲染Physically - Based Rendering之specular BRDF【6】上找到。
				// https://blog.csdn.net/wolf96/article/details/44172243
			
			}

			fixed4 frag(v2f i) :SV_Target{

			}

		ENDCG

		}
    }
    FallBack "Diffuse"
}
