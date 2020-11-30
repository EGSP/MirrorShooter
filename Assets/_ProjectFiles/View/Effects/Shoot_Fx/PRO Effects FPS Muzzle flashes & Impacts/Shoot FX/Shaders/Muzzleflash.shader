// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Knife/MuzzleFlash"
{
	Properties
	{
		_Noise("Noise", 2D) = "white" {}
		_Noise1("Noise1", 2D) = "white" {}
		_Alpha("Alpha", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (1,1,1,1)
		[HDR]_Color1("Color 1", Color) = (1,1,1,1)
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_NoiseSoftness1("NoiseSoftness1", Range( 0 , 1)) = 0
		_NoiseSoftness2("NoiseSoftness2", Range( 0 , 1)) = 0
		_NoiseSpeed1("NoiseSpeed1", Vector) = (0,1,0,0)
		_NoiseSpeed("NoiseSpeed", Vector) = (0,1,0,0)
		_DepthFade("DepthFade", Float) = 0
		_AlphaSoftness("AlphaSoftness", Range( 0 , 1)) = 1
		[Normal]_Distortion("Distortion", 2D) = "bump" {}
		_DistortionAmount("DistortionAmount", Range( 0 , 1)) = 0
		_DistortionDiff("DistortionDiff", Float) = 0
		_DistortionSpeed1("DistortionSpeed1", Vector) = (0,0,0,0)
		_DistortionSpeed2("DistortionSpeed2", Vector) = (0,0,0,0)
		_CenterFadeSize("CenterFadeSize", Range( -1 , 1)) = 0
		_CenterNoiseFadeSize("CenterNoiseFadeSize", Range( -1 , 1)) = 0
		_CenterNoiseFadeSoftness("CenterNoiseFadeSoftness", Range( 0 , 1)) = 0
		_CenterFadeSoftness("CenterFadeSoftness", Range( 0 , 1)) = 0
		_DissolveSoftness("DissolveSoftness", Range( 0 , 1)) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#include "UnityStandardUtils.cginc"
			#define ASE_NEEDS_FRAG_COLOR


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
			};

			uniform float4 _Color0;
			uniform float4 _Color1;
			uniform float _NoiseSoftness1;
			uniform float _NoiseSoftness2;
			uniform sampler2D _Noise1;
			uniform float2 _NoiseSpeed1;
			uniform float4 _Noise1_ST;
			uniform sampler2D _Noise;
			uniform float2 _NoiseSpeed;
			uniform float4 _Noise_ST;
			uniform float _CenterNoiseFadeSize;
			uniform float _CenterNoiseFadeSoftness;
			uniform float _AlphaSoftness;
			uniform sampler2D _Alpha;
			uniform float4 _Alpha_ST;
			uniform float _DistortionAmount;
			uniform float _CenterFadeSize;
			uniform float _CenterFadeSoftness;
			uniform sampler2D _Distortion;
			uniform float2 _DistortionSpeed1;
			uniform float4 _Distortion_ST;
			uniform float2 _DistortionSpeed2;
			uniform float _DistortionDiff;
			uniform float _Opacity;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _DepthFade;
			uniform float _DissolveSoftness;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_texcoord1 = v.ase_texcoord;
				o.ase_color = v.color;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
#endif
				float2 uv0_Noise1 = i.ase_texcoord1 * _Noise1_ST.xy + _Noise1_ST.zw;
				float2 panner80 = ( 1.0 * _Time.y * _NoiseSpeed1 + uv0_Noise1);
				float2 uv0_Noise = i.ase_texcoord1.xy * _Noise_ST.xy + _Noise_ST.zw;
				float2 panner24 = ( 1.0 * _Time.y * _NoiseSpeed + uv0_Noise);
				float2 uv0180 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( -0.5,-0.5 );
				float smoothstepResult178 = smoothstep( _CenterNoiseFadeSize , ( _CenterNoiseFadeSize + _CenterNoiseFadeSoftness ) , length( ( uv0180 * float2( 2,2 ) ) ));
				float CenterNoiseFade179 = smoothstepResult178;
				float lerpResult173 = lerp( 0.0 , ( ( tex2D( _Noise1, panner80 ).r + tex2D( _Noise, panner24 ).r ) / 2.0 ) , CenterNoiseFade179);
				float smoothstepResult11 = smoothstep( _NoiseSoftness1 , _NoiseSoftness2 , lerpResult173);
				float4 lerpResult9 = lerp( _Color0 , _Color1 , smoothstepResult11);
				float2 uv0_Alpha = i.ase_texcoord1.xy * _Alpha_ST.xy + _Alpha_ST.zw;
				float2 uv0116 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( -0.5,-0.5 );
				float smoothstepResult120 = smoothstep( _CenterFadeSize , ( _CenterFadeSize + _CenterFadeSoftness ) , length( ( uv0116 * float2( 2,2 ) ) ));
				float CenterFade126 = smoothstepResult120;
				float DistortionAmount115 = ( _DistortionAmount * CenterFade126 );
				float2 uv0_Distortion = i.ase_texcoord1.xy * _Distortion_ST.xy + _Distortion_ST.zw;
				float2 panner107 = ( 1.0 * _Time.y * _DistortionSpeed1 + uv0_Distortion);
				float2 panner108 = ( 1.0 * _Time.y * _DistortionSpeed2 + ( uv0_Distortion * _DistortionDiff ));
				float2 DistortionOffset113 = ( (UnpackScaleNormal( tex2D( _Distortion, panner107 ), DistortionAmount115 )).xy + (UnpackScaleNormal( tex2D( _Distortion, panner108 ), DistortionAmount115 )).xy );
				float smoothstepResult69 = smoothstep( 0.0 , _AlphaSoftness , tex2D( _Alpha, ( uv0_Alpha + DistortionOffset113 ) ).r);
				float4 screenPos = i.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth50 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth50 = abs( ( screenDepth50 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
				float clampResult52 = clamp( distanceDepth50 , 0.0 , 1.0 );
				float smoothstepResult58 = smoothstep( _NoiseSoftness1 , _NoiseSoftness2 , lerpResult173);
				float clampResult86 = clamp( ( smoothstepResult69 - smoothstepResult58 ) , 0.0 , 1.0 );
				float4 temp_output_13_0 = ( lerpResult9 * ( smoothstepResult69 * _Opacity * clampResult52 * clampResult86 ) * i.ase_color );
				float4 uv0130 = i.ase_texcoord1;
				uv0130.xy = i.ase_texcoord1.xy * float2( 1,1 ) + float2( -0.5,-0.5 );
				float temp_output_143_0 = ( 1.0 - ( length( (uv0130).xy ) * 2.0 ) );
				float DissolveHide156 = uv0130.w;
				float smoothstepResult150 = smoothstep( 0.0 , _DissolveSoftness , ( temp_output_143_0 + DissolveHide156 ));
				float DissolveShow139 = uv0130.z;
				float smoothstepResult154 = smoothstep( 0.0 , _DissolveSoftness , ( temp_output_143_0 + DissolveShow139 ));
				float clampResult148 = clamp( ( smoothstepResult150 + ( 1.0 - smoothstepResult154 ) ) , 0.0 , 1.0 );
				float FinalDissolve146 = clampResult148;
				float clampResult134 = clamp( ( (temp_output_13_0).a - FinalDissolve146 ) , 0.0 , 1.0 );
				float4 appendResult132 = (float4((temp_output_13_0).rgb , clampResult134));
				
				
				finalColor = appendResult132;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18000
-1885;7;1839;1004;2429.05;884.9624;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;116;-5685.754,1356.721;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;122;-5408.754,1707.721;Inherit;False;Property;_CenterFadeSize;CenterFadeSize;17;0;Create;True;0;0;False;0;0;-0.12;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;121;-5213.754,1926.721;Inherit;False;Property;_CenterFadeSoftness;CenterFadeSoftness;20;0;Create;True;0;0;False;0;0;0.813;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-5381.754,1439.721;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;2,2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;123;-4925.754,1744.721;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;117;-5038.754,1394.721;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;120;-4764.654,1474.621;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;126;-4600.664,1484.385;Inherit;False;CenterFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-5240.932,1101.866;Inherit;False;Property;_DistortionAmount;DistortionAmount;13;0;Create;True;0;0;False;0;0;0.11;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;-4837.264,1282.885;Inherit;False;126;CenterFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;104;-4343.932,548.8658;Inherit;False;0;96;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-4641.754,1187.721;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-4226.932,781.8658;Inherit;False;Property;_DistortionDiff;DistortionDiff;14;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-3951.932,738.8658;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;110;-4033.932,621.8658;Inherit;False;Property;_DistortionSpeed1;DistortionSpeed1;15;0;Create;True;0;0;False;0;0,0;0.05,0.05;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;115;-4423.754,1160.721;Inherit;False;DistortionAmount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;109;-4073.932,920.8658;Inherit;False;Property;_DistortionSpeed2;DistortionSpeed2;16;0;Create;True;0;0;False;0;0,0;-0.05,-0.05;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;125;-3691.754,677.7214;Inherit;False;115;DistortionAmount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;107;-3686.932,466.8658;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;108;-3728.932,827.8658;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;180;-5612.182,2187.34;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;96;-3368.932,502.8658;Inherit;True;Property;_Distortion;Distortion;12;1;[Normal];Create;True;0;0;False;0;-1;None;3e642b290e1041c45bbd75a4ab51cba7;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;103;-3334.932,765.8658;Inherit;True;Property;_TextureSample0;Texture Sample 0;12;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Instance;96;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-5308.182,2270.34;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;2,2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-5335.182,2538.34;Inherit;False;Property;_CenterNoiseFadeSize;CenterNoiseFadeSize;18;0;Create;True;0;0;False;0;0;0.3;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-5140.182,2757.34;Inherit;False;Property;_CenterNoiseFadeSoftness;CenterNoiseFadeSoftness;19;0;Create;True;0;0;False;0;0;0.237;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;77;-2841.709,-816.8359;Float;False;Property;_NoiseSpeed1;NoiseSpeed1;8;0;Create;True;0;0;False;0;0,1;0,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;25;-2847.432,-213.9333;Float;False;Property;_NoiseSpeed;NoiseSpeed;9;0;Create;True;0;0;False;0;0,1;0,-0.4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;130;-1196.339,933.6655;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;73;-3452.31,-984.9358;Inherit;False;0;81;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;102;-3071.932,540.8658;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;111;-2994.932,773.8658;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;38;-3349.032,-465.033;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;177;-4852.182,2575.34;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;181;-4965.182,2225.34;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;24;-2556.232,-321.8333;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;-2788.932,643.8658;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;138;-882.3936,843.4243;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;80;-2550.51,-924.736;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;178;-4691.082,2305.24;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;137;-613.3936,865.4243;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;81;-2320.446,-796.1949;Inherit;True;Property;_Noise1;Noise1;1;0;Create;True;0;0;False;0;-1;None;2140d5caeca76404cadd35cc48f45f10;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;-2661.928,655.6635;Inherit;False;DistortionOffset;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-2323.168,-487.2923;Inherit;True;Property;_Noise;Noise;0;0;Create;True;0;0;False;0;-1;None;2140d5caeca76404cadd35cc48f45f10;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-1923.311,-624.6215;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;179;-4527.092,2315.004;Inherit;False;CenterNoiseFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;99;-2024.932,101.8658;Inherit;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;114;-1911.048,366.9264;Inherit;False;113;DistortionOffset;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;-406.3936,879.4243;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;139;-921.3936,1054.424;Inherit;False;DissolveShow;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;172;-1772.186,-295.2176;Inherit;False;179;CenterNoiseFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;66.50624,1308.307;Inherit;False;139;DissolveShow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;84;-1654.311,-623.6215;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;-1684.932,181.8658;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;143;-5.142544,886.0292;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;-951.3989,1145.664;Inherit;False;DissolveHide;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-1590.989,-149.8729;Float;False;Property;_NoiseSoftness1;NoiseSoftness1;6;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-1306.498,383.3688;Float;False;Property;_AlphaSoftness;AlphaSoftness;11;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;173;-1301.186,-471.2176;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1612.519,1.120804;Float;False;Property;_NoiseSoftness2;NoiseSoftness2;7;0;Create;True;0;0;False;0;0;0.426;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;258.9907,1011.378;Inherit;False;156;DissolveHide;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-1326,152;Inherit;True;Property;_Alpha;Alpha;2;0;Create;True;0;0;False;0;-1;None;a26f58b2a629a9c40bc2c333660368e6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;153;314.6746,1191.202;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;166.2071,1478.778;Inherit;False;Property;_DissolveSoftness;DissolveSoftness;21;0;Create;True;0;0;False;0;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1072.198,657.0425;Float;False;Property;_DepthFade;DepthFade;10;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;69;-979.498,177.3688;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;58;-995.1234,-104.3162;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;142;483.6064,874.4243;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;154;703.9354,1419.033;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;162;965.053,1430.26;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;150;720.0172,1137.201;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;55;-641.1234,13.68384;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;50;-884.198,589.0425;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;170;1115.807,1158.209;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;86;-470.7563,69.42346;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;11;-979.5187,-352.8792;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-1147.8,-830.3;Float;False;Property;_Color0;Color 0;3;1;[HDR];Create;True;0;0;False;0;1,1,1,1;9.189589,3.045738,1.34376,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-1153.8,-674.3;Float;False;Property;_Color1;Color 1;4;1;[HDR];Create;True;0;0;False;0;1,1,1,1;55.06544,12.68523,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;52;-576.198,573.0425;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-836,407;Float;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;9;-647.9,-640.7999;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-192,324;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;148;1206.606,922.4243;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;14;-113.519,112.1208;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;106.481,-47.87921;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;146;1363.606,871.4243;Inherit;False;FinalDissolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;131;245.6064,198.4243;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;147;149.6064,434.4243;Inherit;True;146;FinalDissolve;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;133;491.6064,229.4243;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;129;309.661,-2.334534;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;134;636.8465,194.4243;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;132;591.6064,35.42426;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;968.9449,-102.2239;Float;False;True;-1;2;ASEMaterialInspector;0;1;Knife/MuzzleFlash;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;119;0;116;0
WireConnection;123;0;122;0
WireConnection;123;1;121;0
WireConnection;117;0;119;0
WireConnection;120;0;117;0
WireConnection;120;1;122;0
WireConnection;120;2;123;0
WireConnection;126;0;120;0
WireConnection;124;0;97;0
WireConnection;124;1;128;0
WireConnection;106;0;104;0
WireConnection;106;1;105;0
WireConnection;115;0;124;0
WireConnection;107;0;104;0
WireConnection;107;2;110;0
WireConnection;108;0;106;0
WireConnection;108;2;109;0
WireConnection;96;1;107;0
WireConnection;96;5;125;0
WireConnection;103;1;108;0
WireConnection;103;5;125;0
WireConnection;175;0;180;0
WireConnection;102;0;96;0
WireConnection;111;0;103;0
WireConnection;177;0;176;0
WireConnection;177;1;174;0
WireConnection;181;0;175;0
WireConnection;24;0;38;0
WireConnection;24;2;25;0
WireConnection;112;0;102;0
WireConnection;112;1;111;0
WireConnection;138;0;130;0
WireConnection;80;0;73;0
WireConnection;80;2;77;0
WireConnection;178;0;181;0
WireConnection;178;1;176;0
WireConnection;178;2;177;0
WireConnection;137;0;138;0
WireConnection;81;1;80;0
WireConnection;113;0;112;0
WireConnection;2;1;24;0
WireConnection;83;0;81;1
WireConnection;83;1;2;1
WireConnection;179;0;178;0
WireConnection;140;0;137;0
WireConnection;139;0;130;3
WireConnection;84;0;83;0
WireConnection;100;0;99;0
WireConnection;100;1;114;0
WireConnection;143;0;140;0
WireConnection;156;0;130;4
WireConnection;173;1;84;0
WireConnection;173;2;172;0
WireConnection;3;1;100;0
WireConnection;153;0;143;0
WireConnection;153;1;144;0
WireConnection;69;0;3;1
WireConnection;69;2;67;0
WireConnection;58;0;173;0
WireConnection;58;1;85;0
WireConnection;58;2;12;0
WireConnection;142;0;143;0
WireConnection;142;1;157;0
WireConnection;154;0;153;0
WireConnection;154;2;152;0
WireConnection;162;0;154;0
WireConnection;150;0;142;0
WireConnection;150;2;152;0
WireConnection;55;0;69;0
WireConnection;55;1;58;0
WireConnection;50;0;51;0
WireConnection;170;0;150;0
WireConnection;170;1;162;0
WireConnection;86;0;55;0
WireConnection;11;0;173;0
WireConnection;11;1;85;0
WireConnection;11;2;12;0
WireConnection;52;0;50;0
WireConnection;9;0;4;0
WireConnection;9;1;5;0
WireConnection;9;2;11;0
WireConnection;8;0;69;0
WireConnection;8;1;7;0
WireConnection;8;2;52;0
WireConnection;8;3;86;0
WireConnection;148;0;170;0
WireConnection;13;0;9;0
WireConnection;13;1;8;0
WireConnection;13;2;14;0
WireConnection;146;0;148;0
WireConnection;131;0;13;0
WireConnection;133;0;131;0
WireConnection;133;1;147;0
WireConnection;129;0;13;0
WireConnection;134;0;133;0
WireConnection;132;0;129;0
WireConnection;132;3;134;0
WireConnection;1;0;132;0
ASEEND*/
//CHKSM=5682DBB5AB1573C84C13160A95C1E5F36827643F