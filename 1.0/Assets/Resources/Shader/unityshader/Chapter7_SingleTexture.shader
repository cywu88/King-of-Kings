// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UnityShader/Chapter7_SingleTexture"
{
	Properties
	{
		_Color("Color",color) = (1,1,1,1)
		_MainTex("Main Tex", 2D) = "white"{}
		_Specular("Specular", color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(8.0, 256)) = 20
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
			//纹理类型的属性需要再声明一个float4类型的变量，命名格式为 纹理名_ST 
			//其中，ST是缩放和偏移的缩写 纹理名_ST.xy存储的是缩放值，纹理名_ST.zw存储的是偏移值
			//对应材质面板的纹理属性的Tiling和Offset调节项
			float4 _MainTex_ST;
			fixed4 _Specular;
			float   _Gloss;
			
			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;

				//使用texcoord变量存储模型的第一组纹理坐标
				float4 texcoord:TEXCOORD0;
			};

			struct v2f {
				float4 pos:SV_POSITION;
				float3 worldNormal:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float2 uv:TEXCOORD2;
			};

			v2f vert(a2v v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertext);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				//对纹理坐标进行变换，对应材质面板的Tiling和Offset调节项
				o.uv = v.texcoord.xy*_MainTex_ST.xy + _MainTex_ST.zw;
				//也可以使用内置函数  o.uv=TRANSFORM_TEX(v.texcoord,_MainTex); 计算过程一样
				  
				return o;

			}

			fixed4 frag(v2f i) :SV_Target{
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

 

		ENDCG

		}
    }
    FallBack "Diffuse"
}
