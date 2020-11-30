// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Knife/Distortion"
{
	Properties
	{
		_NormalMap("NormalMap", 2D) = "bump" {}
		_NormalMap2("NormalMap2", 2D) = "bump" {}
		_DistortionAmount2("DistortionAmount2", Float) = 1
		_DistortionAmount("DistortionAmount", Float) = 1
		_AlphaMask("AlphaMask", 2D) = "white" {}
		[Toggle(_TWONORMALS_ON)] _TwoNormals("TwoNormals", Float) = 0
		_DistortionSpeed2("DistortionSpeed2", Vector) = (0,0,0,0)
		_DistortionSpeed("DistortionSpeed", Vector) = (0,0,0,0)
		[Toggle(_DEBUG_ON)] _Debug("Debug", Float) = 0
		[Toggle(_SCREENSPACEUV_ON)] _ScreenSpaceUV("ScreenSpaceUV", Float) = 0
		_Tiling2("Tiling2", Float) = 1
		_Tiling1("Tiling1", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 4.0
		#pragma shader_feature _DEBUG_ON
		#pragma shader_feature _SCREENSPACEUV_ON
		#pragma shader_feature _TWONORMALS_ON
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit alpha:fade keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _DistortionAmount;
		uniform sampler2D _AlphaMask;
		uniform float4 _AlphaMask_ST;
		uniform sampler2D _NormalMap;
		uniform float2 _DistortionSpeed;
		uniform float4 _NormalMap_ST;
		uniform float _Tiling1;
		uniform float _DistortionAmount2;
		uniform sampler2D _NormalMap2;
		uniform float2 _DistortionSpeed2;
		uniform float4 _NormalMap2_ST;
		uniform float _Tiling2;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float2 uv_AlphaMask = i.uv_texcoord * _AlphaMask_ST.xy + _AlphaMask_ST.zw;
			float4 tex2DNode9 = tex2D( _AlphaMask, uv_AlphaMask );
			float2 uv0_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 temp_output_28_0 = (ase_screenPosNorm).xy;
			#ifdef _SCREENSPACEUV_ON
				float2 staticSwitch25 = temp_output_28_0;
			#else
				float2 staticSwitch25 = uv0_NormalMap;
			#endif
			float2 panner16 = ( 1.0 * _Time.y * _DistortionSpeed + ( staticSwitch25 * _Tiling1 ));
			float2 uv0_NormalMap2 = i.uv_texcoord * _NormalMap2_ST.xy + _NormalMap2_ST.zw;
			#ifdef _SCREENSPACEUV_ON
				float2 staticSwitch26 = temp_output_28_0;
			#else
				float2 staticSwitch26 = uv0_NormalMap2;
			#endif
			float2 panner17 = ( 1.0 * _Time.y * _DistortionSpeed2 + ( staticSwitch26 * _Tiling2 ));
			#ifdef _TWONORMALS_ON
				float3 staticSwitch10 = UnpackScaleNormal( tex2D( _NormalMap2, panner17 ), ( _DistortionAmount2 * tex2DNode9.r * i.vertexColor.a ) );
			#else
				float3 staticSwitch10 = float3( 0,0,0 );
			#endif
			float3 normalizeResult13 = normalize( ( UnpackScaleNormal( tex2D( _NormalMap, panner16 ), ( _DistortionAmount * i.vertexColor.a * tex2DNode9.r ) ) + staticSwitch10 ) );
			float4 screenColor6 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + float4( normalizeResult13 , 0.0 ) ).xy);
			#ifdef _DEBUG_ON
				float4 staticSwitch24 = tex2DNode9;
			#else
				float4 staticSwitch24 = screenColor6;
			#endif
			o.Emission = staticSwitch24.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
-1885;7;1839;1004;1444.986;341.9863;1;True;False
Node;AmplifyShaderEditor.ScreenPosInputsNode;27;-3758.998,294.2185;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-3449.231,705.7425;Inherit;False;0;11;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;28;-3531.998,283.2185;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;26;-3053.998,511.2185;Inherit;False;Property;_ScreenSpaceUV;ScreenSpaceUV;9;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Reference;25;False;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2894.666,682.6489;Inherit;False;Property;_Tiling2;Tiling2;10;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-3601.431,-203.3576;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-2716.047,561.0468;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-3039.998,-4.781494;Inherit;False;Property;_Tiling1;Tiling1;11;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-2400.933,528.5466;Inherit;False;Property;_DistortionAmount2;DistortionAmount2;2;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-2628.5,288.5;Inherit;True;Property;_AlphaMask;AlphaMask;4;0;Create;True;0;0;False;0;-1;None;031ee2bac6a3f7b469382bf25cdd5256;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;25;-3224.172,-201.2526;Inherit;False;Property;_ScreenSpaceUV;ScreenSpaceUV;9;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;3;-2647.5,41.5;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;22;-2331.931,842.5424;Inherit;False;Property;_DistortionSpeed2;DistortionSpeed2;6;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;2;-2594.5,-106.5;Inherit;False;Property;_DistortionAmount;DistortionAmount;3;0;Create;True;0;0;False;0;1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-2033.933,433.5466;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;17;-2113.931,599.5424;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-2863.998,-197.7815;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;20;-2415.931,-209.4576;Inherit;False;Property;_DistortionSpeed;DistortionSpeed;7;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;16;-2109.931,-340.4576;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-2191.5,50.5;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-1770.933,393.5466;Inherit;True;Property;_NormalMap2;NormalMap2;1;0;Create;True;0;0;False;0;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;10;-1341.933,341.5466;Inherit;False;Property;_TwoNormals;TwoNormals;5;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;1;-1814.5,8.5;Inherit;True;Property;_NormalMap;NormalMap;0;0;Create;True;0;0;False;0;-1;None;3e642b290e1041c45bbd75a4ab51cba7;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-1160.933,46.54663;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;13;-987.9326,82.54663;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GrabScreenPosition;5;-1006.5,-265.5;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-693.5,-74.5;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenColorNode;6;-354.2642,-37.43468;Inherit;False;Global;_GrabScreen0;Grab Screen 0;2;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;24;-195.0104,185.1603;Inherit;False;Property;_Debug;Debug;8;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;45,-27;Float;False;True;-1;4;ASEMaterialInspector;0;0;Unlit;Knife/Distortion;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;27;0
WireConnection;26;1;23;0
WireConnection;26;0;28;0
WireConnection;32;0;26;0
WireConnection;32;1;31;0
WireConnection;25;1;21;0
WireConnection;25;0;28;0
WireConnection;14;0;15;0
WireConnection;14;1;9;1
WireConnection;14;2;3;4
WireConnection;17;0;32;0
WireConnection;17;2;22;0
WireConnection;30;0;25;0
WireConnection;30;1;29;0
WireConnection;16;0;30;0
WireConnection;16;2;20;0
WireConnection;4;0;2;0
WireConnection;4;1;3;4
WireConnection;4;2;9;1
WireConnection;11;1;17;0
WireConnection;11;5;14;0
WireConnection;10;0;11;0
WireConnection;1;1;16;0
WireConnection;1;5;4;0
WireConnection;12;0;1;0
WireConnection;12;1;10;0
WireConnection;13;0;12;0
WireConnection;8;0;5;0
WireConnection;8;1;13;0
WireConnection;6;0;8;0
WireConnection;24;1;6;0
WireConnection;24;0;9;0
WireConnection;0;2;24;0
ASEEND*/
//CHKSM=BA8F2FCDBD8BF727D0A401A587520BA7A66261BA