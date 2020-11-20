// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
//https://zhuanlan.zhihu.com/p/31450857 
//https://blog.csdn.net/lzhq1982/article/details/75212518
//世界空间下的光照模型计算
Shader "UnityShader/Chapter7_WorldNormal"
{
	Properties
	{
		_Color("Color",Color) = (1,1,1,1)
		_MainTex("MainTex",2D) = "white" {}
		_BumpTex("Noraml Tex",2D) = "bump"{} //bump为Unity自带的法线纹理，当没有提供任何法线时，"bump"就对应模型自身的法线信息
		_BumpScale("BumpScal",Float) = 1.0  //BumpScale代表凹凸程度，值为0时，表示该法线纹理不会对光照产生任何影响
		_Specular("Specular",Color) = (1,1,1,1)
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

			#inlcude "Lighting.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpTex;
			float4 _BumpTex_ST;
			float _BumpScale;
			fixed4 _Specular;
			float _Gloss;
			

	 
			//b、在appdata的顶点输入结构体中，我们传入了normal顶点法线和tangent顶点切线

			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;
				float4 tangent:TANGENT;   //tangent存储顶点的切线方向，float4类型，通过tangent.w分量决定副切线的方向性
				float4 texcoord:TEXCOORD0;
			};
 
			//模型的每一个 顶点有自己对应的切线空间，
			//z轴为顶点的法线方向，
			//x轴为顶点的切线方向，
			//y轴为顶点的副切线方向，由法线和切线的叉积得到
			//这种纹理被称为切线空间的法线纹理。

			struct v2f_old {
				float4 pos:SV_POSITION;
				float4 uv:TEXCOORD0;
				float3 lightDir:TEXCOORD1;
				float3 viewDir:TEXCOORD2;
			};


			struct v2f {
				float4 pos:SV_POSITION;
				float4 uv:TEXCOORD0;
				//定义用于存储变换矩阵的变量，并拆分成行存储在对应行的变量中，
				//对于矢量的变换矩阵只需要3X3即可,float4的最后一个值可以用来存储世界空间下顶点的位置
				float4 T2W0:TEXCOORD1;
				float4 T2W1:TEXCOORD2;
				float4 T2W2:TEXCOORD3;
			};

 

			v2f vert_old(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.texcoord.xy*_MainTex_ST.xy + _MainTex_ST.zw;//(uv.xy存储主纹理坐标变换后的uv坐标)
				o.uv.zw = v.texcoord.xy*_BumpTex_ST.xy + _BumpTex_ST.zw;//(uv.zw存储法线纹理坐标变换后的uv坐标)
				//_MainTex和_BumpTex通常会使用同一组纹理坐标（法线纹理贴图由对应纹理贴图生成） 

				float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz))*v.tangent.w;
				float3x3 rotation = float3x3(v.tangent.xyz, binormal, v.normal);
				//(按行填充，得到的矩阵实际上是模型到切线的逆矩阵的转置矩阵，也就是模型到切线的转换矩阵(正交矩阵))
				//也可以使用内建宏TANGENT_SPACE_ROTATION得到变换矩阵 

				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;

				return o;
			}
			 

			//还记得我在原理中说过，我们要在顶点着色器中得到从切线空间变换到世界空间的转换矩阵吗，
			//所以这三个向量就是变换矩阵的组成部分，我们要传给片元处理。

			//c、原理中还说过，变换矩阵的计算可以由顶点的切线，副切线，法线在世界空间下的表示来得到，
			//模型的切线和法线我们有了，直接转换到世界空间就可，副切线我们需要叉乘一下。

			//fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
			//fixed3 worldTangent = UnityObjectToWorldDir(v.tangent);
			//fixed3 binormal = cross(worldNormal, worldTangent) * v.tangent.w;

			//眼尖的读者会看到计算副切线时我们乘了个v.tangent.w，
			//这是因为和切线，法线都垂直的方向有两个，而w决定了我们选择哪一个方向。
			//最后我们把它们赋给v2f结构体传出去即可。

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.texcoord.xy*_MainTex_ST.xy + _MainTex_ST.zw;//(uv.xy存储主纹理坐标变换后的uv坐标)
				o.uv.zw = v.texcoord.xy*_BumpTex_ST.xy + _BumpTex_ST.zw;//(uv.zw存储法线纹理坐标变换后的uv坐标)
				//_MainTex和_BumpTex通常会使用同一组纹理坐标（法线纹理贴图由对应纹理贴图生成） 

				float3 worldPos = mul(unity_ObjectToWorld, v.vertext).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

				o.T2W0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.T2W1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.T2W2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
 
				return o;
			}

			fixed4 frag_old(v2f i) :SV_TARGET{
				fixed3 tangentLightDir = normalize(i.lightDir);
				fixed3 tangentViewDir = normalize(i.viewDir);

				fixed4 packedNormal = tex2D(_BumpTex,i.uv.zw);  //对法线纹理进行采样
				fixed3 tangentNormal;
				//若法线纹理类型没有被设置为bump类型，则进行手动反映射
				//tangentNormal=(packedNormal.xyz*2-1);
				//若已经设置为bump类型，可以使用内建函数
				tangentNormal = UnpackNormal(packedNormal);
				tangentNormal.xy *= _BumpScale;
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy,tangentNormal.xy)));

				fixed3 albedo = _Color.rgb*tex2D(_MainTex,i.uv.xy);
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;
				fixed3 diffuse = _LightColor0.rgb*albedo*max(0,dot(tangentNormal,tangentLightDir));
				fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
				fixed3 specular = _LightColor0.rgb*_Specular.rgb*pow(max(0,dot(tangentNormal,halfDir)),_Gloss);

				return fixed4(ambient + diffuse + specular,1.0);
			}
		
//cross 返回两个3D向量的叉积。
//mul 返回输入矩阵相乘的积




			fixed4 frag(v2f i) :SV_Target{

				float3 worldPos = float3(i.T2W0.w,i.T2W1.w,i.T2W2.w);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				//法线纹理采样
				fixed3 tangentNormal = UnpackNormal(tex2D(_BumpTex, i.uv.zw));
				tangentNormal.xy *= _BumpScale;
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy,tangentNormal.xy)));

				fixed3 worldNormal = normalize(half3(dot(i.T2W0.xyz,tangentNormal),dot(i.T2W1.xyz,tangentNormal),dot(i.T2W2.xyz,tangentNormal)));

				fixed3 albedo = _Color.rgb*tex2D(_MainTex,i.uv).rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;
				fixed3 diffuse = _LightColor0.rgb*albedo*max(0,dot(worldLightDir,worldNormal));
				fixed3 halfDir = normalize(worldLightDir + worldViewDir);
				fixed3 specular = _LightColor0.rgb*_Specular.rgb*pow(max(0,dot(halfDir,worldNormal)),_Gloss);

				return fixed4(ambient + diffuse + specular,1.0);
			}

		ENDCG

		}
    }
    FallBack "Diffuse"
}
