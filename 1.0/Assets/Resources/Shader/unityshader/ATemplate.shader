Shader "UnityShader/Template"
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

			v2f vert(a2v v)
			{

			}

			fixed4 frag(v2f i) :SV_Target{

			}

		ENDCG

		}
    }
    FallBack "Diffuse"
}
