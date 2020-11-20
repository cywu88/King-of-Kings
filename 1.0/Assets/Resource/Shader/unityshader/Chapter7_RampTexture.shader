Shader "UnityShader/Chapter7_RampTexture"
{
	Properties
	{
		_Color("Color",Color) = (1,1,1,1)
		_RampTex("RampTex",2D) = "white"{}
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
			sampler2D _RampTex;
			float4 _RampTex_ST;
			fixed4 _Specular;
			float _Gloss;

			
			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;
				fixed4 texcoord : TEXCOORD0;
			};
			struct v2f {
				float4 pos:SV_POSITION;
				float3 worldNormal:TEXCOORD0;
				float3 worldPos:TEXCOORD1;
				float2 uv:TEXCOORD2;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.texcoord.xy*_RampTex_ST.xy + _RampTex_ST.zw;

				return o;
			}


			fixed4 frag(v2f i) :SV_Target{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed halfLambert = 0.5*dot(worldNormal, worldLightDir) + 0.5;
			
				fixed3 diffuseColor = tex2D(_RampTex, fixed2(halfLambert, halfLambert))*_Color.rgb;
				
				fixed3 halfDir = normalize(worldLightDir + worldViewDir);
				fixed3 specular = _LightColor0.rgb*_Specular.rgb*pow(max(0, dot(halfDir, worldNormal)), _Gloss);

				return fixed4(ambient + diffuse + specular, 1.0); 
			}

		ENDCG

		}
    }
    FallBack "Diffuse"
}
