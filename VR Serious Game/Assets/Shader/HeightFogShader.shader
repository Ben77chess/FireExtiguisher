// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'

Shader "Hidden/HeightFogShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.scrPos = ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float4x4 _CameraToWorldMatrix;

			float4 _FogColor;
			half _FogBottomStart;
			half _FogTopEnd;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos));
				float linear01Depth = Linear01Depth(depth);
				float eyeDepth = LinearEyeDepth(depth);
				
				//Get the viewPos
				float4 ndcPosFarPlane = float4(i.uv.x * 2.0 - 1.0, i.uv.y * 2.0 - 1.0, depth, 1.0);
				float4 viewPos = mul(unity_CameraInvProjection, ndcPosFarPlane);
				viewPos.xyz = viewPos.xyz / viewPos.w;
				//viewPos = viewPos * linear01Depth;
				float4 cameraPos = float4(viewPos.xy, -viewPos.z, viewPos.w);
				float4 worldPos = mul(unity_CameraToWorld, cameraPos); //unity_CameraToWorld
				float worldPosY = worldPos.y;

				//Fog
				float rangePara = (worldPosY - _FogBottomStart) / (_FogTopEnd - _FogBottomStart);
				rangePara = saturate(rangePara);
				float fogCol = _FogColor * rangePara;
				col = lerp(col, fogCol, rangePara);
				//float fogPara = 1 - exp(-pow((linear01Depth * 0.5f + 0.5f) * _FogIntensity, 2));
				//fogPara = (_FogTopEnd - viewPos.y) / (_FogTopEnd - _FogBottomStart);
				//fogPara = saturate(_FogIntensity * fogPara) * (1.0f - linear01Depth);
				//col = float4(worldPos.x, worldPos.x, worldPos.x, 1);

                return col;
            }
            ENDCG
        }
    }
}
