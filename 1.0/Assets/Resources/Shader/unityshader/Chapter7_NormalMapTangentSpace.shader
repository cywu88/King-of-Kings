// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//切线空间下的光照模型计算
Shader "UnityShader/Chapter7_NormalMapTangentSpace"
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
			

			struct a2v_old {
				float4 vertex:POSITION;
				float3 normal:NORMAL;

				//使用texcoord变量存储模型的第一组纹理坐标
				float4 texcoord:TEXCOORD0;
			};

			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;
				float4 tangent:TANGENT;   //tangent存储顶点的切线方向，float4类型，通过tangent.w分量决定副切线的方向性
				float4 texcoord:TEXCOORD0;
			};

			struct v2f_old {
				float4 pos:SV_POSITION;
				float3 worldNormal:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float2 uv:TEXCOORD2;
			};

			struct v2f {
				float4 pos:SV_POSITION;
				float4 uv:TEXCOORD0;
				float3 lightDir:TEXCOORD1;
				float3 viewDir:TEXCOORD2;
			};
			 

			v2f vert_old(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertext);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				//对纹理坐标进行变换，对应材质面板的Tiling和Offset调节项
				o.uv = v.texcoord.xy*_MainTex_ST.xy + _MainTex_ST.zw;
				//也可以使用内置函数  o.uv=TRANSFORM_TEX(v.texcoord,_MainTex); 计算过程一样
				return o;

			}

			v2f vert(a2v v) {
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



			fixed4 frag_old(v2f i) :SV_Target{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

				//使用tex2D做纹理采样，将采样结果和颜色属性_Color相乘作为反射率
				fixed3 albedo = tex2D(_MainTex,i.uv).rgb*_Color.rgb;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;

				fixed3 diffuse = _LightColor0.rgb*albedo*max(0,dot(worldNormal,worldLightDir));

				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				fixed3 specular = _LightColor0.rgb*albedo*pow(saturate(dot(halfDir,worldNormal)), _Gloss);

				return fixed4(ambient + diffuse + specular,1.0);
			}

			fixed4 frag(v2f i) :SV_TARGET{
				fixed3  tangentLightDir = normalize(i.lightDir);
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

		ENDCG

		}
    }
    FallBack "Diffuse"
}
