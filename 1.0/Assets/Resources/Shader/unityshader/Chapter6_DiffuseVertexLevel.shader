// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//逐顶点漫反射实现
//Cdiffuse = (Clight*Mdiffuse)max(0,n*I)
//Cspecular = (Clight*mSpecular)max(0,v*r) Mglass
Shader "UnityShader/Chapter6_DiffuseVertexLevel"
{
	Properties
	{
		_Diffuse("DiffuseColor",Color) = (1.0,1.0,1.0,1.0)
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
			
			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;
			};

			struct v2f {
				float4 pos:SV_POSITION;
				fixed3 color : COLOR;
			};

			v2f vert(a2v v)
			{
				v2f o;

				//mul(UNITY_MATRIX_MVP, v.vertex) ==> UnityObjectToClipPos(v.vertex); 

				o.pos = UnityObjectToClipPos(v.vertex);

				//fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				////通过模型到世界的转置逆矩阵计算得到世界空间内的顶点法向方向
				////（v.normal存储的是模型空间内的顶点法线方向）
				//fixed3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject));
				////得到世界空间内的光线方向
				//fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);


				////根据Lambert定律计算漫反射 
				//saturate函数将所得矢量或标量的值限定在[0,1]之间
				//fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLight))
					 
				//只有顶点着色器才能接受来自模型空间的数据，因此需要先计算再传递给片元着色器
				o.worldNormal = mul(v.normal, unity_WorldToObject);;

				return o;
			}

			fixed4 frag(v2f i) :SV_Target{

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLight = normalize(_WorldSpaceLightPos0);
				 
				//Cdiffuse = (Clight*Mdiffuse)max(0,n*I)
				////根据Lambert定律计算漫反射 
				//saturate函数将所得矢量或标量的值限定在[0,1]之间
				fixed3 diffuse = _LightColor0.rgb*_Diffuse.rgb*saturate(dot(worldNormal, worldLight));
				fixed3 color = diffuse + ambient;
				return fixed4(color, 1.0);
			}

		ENDCG

		}
    }
    FallBack "Diffuse"
}

//左侧为逐顶点实现效果，右侧为逐像素实现效果。可以看到，
//在逐顶点效果中，模型下部背光面与向光面的交界处会出现不平滑的锯齿过渡。
//而逐像素则交界处相对平滑。但以上两种均存在在光线无法到达的区域，模型外观通常全黑，没有明暗变化，
//使背光区域看起来像平面，为此提出一项改进技术——Half Lambert光照模型。


//https://zhuanlan.zhihu.com/p/31229044