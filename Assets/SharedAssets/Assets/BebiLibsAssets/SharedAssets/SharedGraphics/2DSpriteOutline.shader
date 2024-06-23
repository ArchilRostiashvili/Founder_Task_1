Shader "2DSpriteOutline"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_Tint("Tint", Color) = (1,0,0,1)
		_OutlineColor("OutlineColor", Color) = (1,0.7945905,0,1)
		_OutlineWidth("OutlineWidth", Range( 0 , 0.1)) = 0.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		
		#pragma prefer_hlslcc gles
		#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal vulkan 

		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"

		ENDHLSL

		
		Pass
		{
			Name "Unlit"
			

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			
			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Tint;
			float4 _MainTex_ST;
			float4 _OutlineColor;
			float _OutlineWidth;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float2 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			// #if ETC1_EXTERNAL_ALPHA
			// 	TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
			// 	float _EnableAlphaTexture;
			// #endif

			#if ETC1_EXTERNAL_ALPHA
				sampler2D _AlphaTex;
				float _EnableExternalAlpha;
			#endif

			float4 _RendererColor;

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				
				o.texCoord0 = TRANSFORM_TEX(v.uv0, _MainTex);
				o.color = v.color;
				o.clipPos = UnityObjectToClipPos(v.vertex);

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_MainTex = IN.texCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode17 = tex2D( _MainTex, uv_MainTex );
				float2 appendResult8 = (float2(_OutlineWidth , 0.0));
				float2 texCoord13 = IN.texCoord0.xy * float2( 1,1 ) + appendResult8;
				float2 appendResult11 = (float2(( _OutlineWidth * -1.0 ) , 0.0));
				float2 texCoord12 = IN.texCoord0.xy * float2( 1,1 ) + appendResult11;
				float2 appendResult10 = (float2(0.0 , _OutlineWidth));
				float2 texCoord14 = IN.texCoord0.xy * float2( 1,1 ) + appendResult10;
				float2 appendResult9 = (float2(0.0 , ( _OutlineWidth * -1.0 )));
				float2 texCoord7 = IN.texCoord0.xy * float2( 1,1 ) + appendResult9;
				float2 appendResult37 = (float2(_OutlineWidth , _OutlineWidth));
				float2 texCoord42 = IN.texCoord0.xy * float2( 1,1 ) + ( appendResult37 * float2( 0.7,0.7 ) );
				float2 appendResult36 = (float2(_OutlineWidth , _OutlineWidth));
				float2 texCoord39 = IN.texCoord0.xy * float2( 1,1 ) + ( appendResult36 * float2( -0.7,-0.7 ) );
				float2 appendResult38 = (float2(_OutlineWidth , _OutlineWidth));
				float2 texCoord40 = IN.texCoord0.xy * float2( 1,1 ) + ( appendResult38 * float2( 0.7,-0.7 ) );
				float2 appendResult35 = (float2(_OutlineWidth , _OutlineWidth));
				float2 texCoord41 = IN.texCoord0.xy * float2( 1,1 ) + ( appendResult35 * float2( -0.7,0.7 ) );
				float clampResult25 = clamp( ( tex2D( _MainTex, texCoord13 ).a + tex2D( _MainTex, texCoord12 ).a + tex2D( _MainTex, texCoord14 ).a + tex2D( _MainTex, texCoord7 ).a + tex2D( _MainTex, texCoord42 ).a + tex2D( _MainTex, texCoord39 ).a + tex2D( _MainTex, texCoord40 ).a + tex2D( _MainTex, texCoord41 ).a ) , 0.0 , 1.0 );
				float temp_output_29_0 = ( clampResult25 - tex2DNode17.a );
				float4 appendResult59 = (float4(temp_output_29_0 , temp_output_29_0 , temp_output_29_0 , clampResult25));
				float4 lerpResult60 = lerp( ( _Tint * tex2DNode17 ) , _OutlineColor , appendResult59);
				
				float4 Color = lerpResult60;

				// #if ETC1_EXTERNAL_ALPHA
				// 	float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
				// 	Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				// #endif
				#if ETC1_EXTERNAL_ALPHA
					fixed4 alpha = tex2D (_AlphaTex,  IN.texCoord0.xy);
					Color.a = lerp (Color.a, alpha.r, _EnableExternalAlpha);
				#endif

				Color *= IN.color;

				return Color;
			}

			ENDHLSL
		}
	}
	Fallback "Sprites/Default"	
}
