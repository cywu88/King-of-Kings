Shader "UnityShader/Chapter7_MaskTexture"
{
	Properties
	{
		_Color("Color",Color) = (1,1,1,1)
		_MainTex("MainTex",2D) = "white"{}
		_BumpTex("BumpTex",2D) = "bump"{}
		_BumpScale("BumpScale",Float) = 1.0
		_SpecularMask("SpecularMask",2D) = "white"{}
		_SpecularMaskScale("SpecularMaskScale",Float) = 1.0
		_Specular("Specular",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(8.0,256)) = 20.0
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
			float4 _MainTex_ST;     //主纹理，法线纹理，遮罩纹理的纹理坐标用一个变量来存储
			sampler2D _BumpTex;
			float _BumpScale;
			sampler2D _SpecularMask;
			float _SpecularMaskScale;
			fixed4 _Specular;
			float _Gloss;

			
			struct a2v {
				float4 vertex:POSITION;
				float3 normal:NORMAL;
				float4 tangent:TANGENT;
				float4 texcoord:TEXCOORD0;
			};

			struct v2f {
				float4 pos:SV_POSITION;
				float2 uv:TEXCOORD0;
				float3 lightDir:TEXCOORD1;
				float3 viewDir:TEXCOORD2;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord*_MainTex_ST.xy + _MainTex_ST.zw;

				float3 binormal = cross(normalize(v.normal), normalize(v.tangent))*v.tangent.w;
				float3x3 rotation = float3x3(v.tangent.xyz, binormal, v.normal);

				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;

				return o;
			}


			fixed4 frag(v2f i) :SV_Target{
				fixed3 tangentLightDir = normalize(i.lightDir);
				fixed3 tangentViewDir = normalize(i.viewDir);

				fixed3 tangentNormal = UnpackNormal(tex2D(_BumpTex,i.uv));
				tangentNormal.xy *= _BumpScale;
				tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy,tangentNormal.xy)));

				fixed3 albedo = tex2D(_MainTex,i.uv)*_Color.rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;
				fixed3 diffuse = _LightColor0.rgb*albedo*max(0,dot(tangentLightDir,tangentNormal));

				fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
				fixed3 specularMask = tex2D(_SpecularMask, i.uv).r*_SpecularMaskScale;//使用其中一个通道r来影响高光的光照效果
				fixed3 specular = _LightColor0.rgb*_Specular.rgb*pow(max(0,dot(halfDir,tangentNormal)),_Gloss)*specularMask;

				return fixed4(ambient + diffuse + specular,1.0);
			}

		ENDCG

		}
    }
    FallBack "Diffuse"
}
