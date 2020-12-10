// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Knife/Particle Channel Packed"
{
	Properties
	{
		_Rows("Rows", Float) = 4
		_Columns("Columns", Float) = 4
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		[Toggle(_MAINTEXSMOOTHSTEP_ON)] _MainTexSmoothstep("MainTexSmoothstep", Float) = 0
		_MainSoftnessMin("MainSoftnessMin", Range( 0 , 1)) = 0
		_MainSoftnessMax("MainSoftnessMax", Range( 0 , 1)) = 1
		_AlphaSoftness("AlphaSoftness", Range( 0 , 1)) = 0
		_DepthSoftness("DepthSoftness", Float) = 1
		[Toggle(_ALPHADISSOLVE_ON)] _AlphaDissolve("AlphaDissolve", Float) = 0
		[HDR]_Emission("Emission", Color) = (0,0,0,0)
		[Toggle(_EMISSIONDISSOLVE_ON)] _EmissionDissolve("EmissionDissolve", Float) = 0
		_EmissionTex("EmissionTex", 2D) = "white" {}
		_EmissionSoftness1("EmissionSoftness1", Range( 0 , 1)) = 0
		_EmissionSoftness2("EmissionSoftness2", Range( 0 , 1)) = 0
		[Toggle(_FINALALPHASMOOTHSTEP_ON)] _FinalAlphaSmoothstep("FinalAlphaSmoothstep", Float) = 0
		_FinalAlphaSmoothstepMin("FinalAlphaSmoothstepMin", Range( 0 , 1)) = 0
		_FinalAlphaSmoothstepMax("FinalAlphaSmoothstepMax", Range( 0 , 1)) = 1
		[Toggle(_EMISSIONALPHA_ON)] _EmissionAlpha("EmissionAlpha", Float) = 0
		[Toggle(_FINALEMISSIONSMOOTHSTEP_ON)] _FinalEmissionSmoothstep("FinalEmissionSmoothstep", Float) = 0
		_FinalEmissionSmoothstepMin("FinalEmissionSmoothstepMin", Range( 0 , 1)) = 0
		_FinalEmissionSmoothstepMax("FinalEmissionSmoothstepMax", Range( 0 , 1)) = 1
		[Toggle(_NORMALMAPENABLED_ON)] _NormalMapEnabled("Normal Map Enabled", Float) = 0
		_NormalMap("NormalMap", 2D) = "bump" {}
		_NormalScale("NormalScale", Float) = 0
		_EmissionSubValue("EmissionSubValue", Range( 0 , 1)) = 0
		[Toggle(_ALPHAEMISSIONDISSOLVESUB_ON)] _AlphaEmissionDissolveSub("Alpha Emission Dissolve Sub", Float) = 0
		_EmissionSpeed("EmissionSpeed", Vector) = (0,0,0,0)
		[Toggle(_ELIMINATEEMISSIONROTATION_ON)] _EliminateEmissionRotation("EliminateEmissionRotation", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Float) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 4.0
		#pragma shader_feature _NORMALMAPENABLED_ON
		#pragma shader_feature _EMISSIONALPHA_ON
		#pragma shader_feature _EMISSIONDISSOLVE_ON
		#pragma shader_feature _ELIMINATEEMISSIONROTATION_ON
		#pragma shader_feature _ALPHAEMISSIONDISSOLVESUB_ON
		#pragma shader_feature _ALPHADISSOLVE_ON
		#pragma shader_feature _MAINTEXSMOOTHSTEP_ON
		#pragma shader_feature _FINALEMISSIONSMOOTHSTEP_ON
		#pragma shader_feature _FINALALPHASMOOTHSTEP_ON
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float4 uv_tex4coord;
			float4 vertexColor : COLOR;
			float2 uv2_texcoord2;
			float4 screenPos;
		};

		uniform float _CullMode;
		uniform float _NormalScale;
		uniform sampler2D _NormalMap;
		uniform float _Columns;
		uniform float _Rows;
		uniform float4 _Color;
		uniform float4 _Emission;
		uniform float _EmissionSoftness1;
		uniform float _EmissionSoftness2;
		uniform sampler2D _EmissionTex;
		uniform float2 _EmissionSpeed;
		uniform float4 _EmissionTex_ST;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DepthSoftness;
		uniform float _AlphaSoftness;
		uniform sampler2D _MainTex;
		uniform float _MainSoftnessMin;
		uniform float _MainSoftnessMax;
		uniform float _EmissionSubValue;
		uniform float _FinalEmissionSmoothstepMin;
		uniform float _FinalEmissionSmoothstepMax;
		uniform float _FinalAlphaSmoothstepMin;
		uniform float _FinalAlphaSmoothstepMax;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float columns135 = _Columns;
			float rows136 = _Rows;
			float AnimFrame4 = round( i.uv_tex4coord.z );
			// *** BEGIN Flipbook UV Animation vars ***
			// Total tiles of Flipbook Texture
			float fbtotaltiles223 = ( columns135 * 2.0 ) * ( rows136 * 2.0 );
			// Offsets for cols and rows of Flipbook Texture
			float fbcolsoffset223 = 1.0f / ( columns135 * 2.0 );
			float fbrowsoffset223 = 1.0f / ( rows136 * 2.0 );
			// Speed of animation
			float fbspeed223 = _Time[ 1 ] * 0.0;
			// UV Tiling (col and row offset)
			float2 fbtiling223 = float2(fbcolsoffset223, fbrowsoffset223);
			// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
			// Calculate current tile linear index
			float fbcurrenttileindex223 = round( fmod( fbspeed223 + AnimFrame4, fbtotaltiles223) );
			fbcurrenttileindex223 += ( fbcurrenttileindex223 < 0) ? fbtotaltiles223 : 0;
			// Obtain Offset X coordinate from current tile linear index
			float fblinearindextox223 = round ( fmod ( fbcurrenttileindex223, ( columns135 * 2.0 ) ) );
			// Multiply Offset X by coloffset
			float fboffsetx223 = fblinearindextox223 * fbcolsoffset223;
			// Obtain Offset Y coordinate from current tile linear index
			float fblinearindextoy223 = round( fmod( ( fbcurrenttileindex223 - fblinearindextox223 ) / ( columns135 * 2.0 ), ( rows136 * 2.0 ) ) );
			// Reverse Y to get tiles from Top to Bottom
			fblinearindextoy223 = (int)(( rows136 * 2.0 )-1) - fblinearindextoy223;
			// Multiply Offset Y by rowoffset
			float fboffsety223 = fblinearindextoy223 * fbrowsoffset223;
			// UV Offset
			float2 fboffset223 = float2(fboffsetx223, fboffsety223);
			// Flipbook UV
			half2 fbuv223 = i.uv_texcoord * fbtiling223 + fboffset223;
			// *** END Flipbook UV Animation vars ***
			#ifdef _NORMALMAPENABLED_ON
				float3 staticSwitch221 = UnpackScaleNormal( tex2D( _NormalMap, fbuv223 ), _NormalScale );
			#else
				float3 staticSwitch221 = float3(0,0,1);
			#endif
			float3 normals206 = staticSwitch221;
			o.Normal = normals206;
			o.Albedo = (( (_Color).rgb * (i.vertexColor).rgb )).xyz;
			float4 temp_cast_0 = (_EmissionSoftness1).xxxx;
			float4 temp_cast_1 = (_EmissionSoftness2).xxxx;
			float2 uv0_EmissionTex = i.uv_texcoord * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
			float2 appendResult283 = (float2(i.uv2_texcoord2.y , i.uv2_texcoord2.y));
			float cos277 = cos( i.uv2_texcoord2.x );
			float sin277 = sin( i.uv2_texcoord2.x );
			float2 rotator277 = mul( ( uv0_EmissionTex + appendResult283 ) - float2( 0.5,0.5 ) , float2x2( cos277 , -sin277 , sin277 , cos277 )) + float2( 0.5,0.5 );
			#ifdef _ELIMINATEEMISSIONROTATION_ON
				float2 staticSwitch279 = rotator277;
			#else
				float2 staticSwitch279 = uv0_EmissionTex;
			#endif
			float2 panner238 = ( 1.0 * _Time.y * _EmissionSpeed + staticSwitch279);
			float4 smoothstepResult193 = smoothstep( temp_cast_0 , temp_cast_1 , tex2D( _EmissionTex, panner238 ));
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth142 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth142 = abs( ( screenDepth142 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthSoftness ) );
			float clampResult146 = clamp( distanceDepth142 , 0.0 , 1.0 );
			float depthFadeAlpha163 = clampResult146;
			float temp_output_18_0 = ( columns135 * rows136 );
			float ChannelFramesCount103 = temp_output_18_0;
			float fbtotaltiles98 = columns135 * rows136;
			float fbcolsoffset98 = 1.0f / columns135;
			float fbrowsoffset98 = 1.0f / rows136;
			float fbspeed98 = _Time[ 1 ] * 0.0;
			float2 fbtiling98 = float2(fbcolsoffset98, fbrowsoffset98);
			float fbcurrenttileindex98 = round( fmod( fbspeed98 + ( frac( ( AnimFrame4 / ChannelFramesCount103 ) ) * ChannelFramesCount103 ), fbtotaltiles98) );
			fbcurrenttileindex98 += ( fbcurrenttileindex98 < 0) ? fbtotaltiles98 : 0;
			float fblinearindextox98 = round ( fmod ( fbcurrenttileindex98, columns135 ) );
			float fboffsetx98 = fblinearindextox98 * fbcolsoffset98;
			float fblinearindextoy98 = round( fmod( ( fbcurrenttileindex98 - fblinearindextox98 ) / columns135, rows136 ) );
			fblinearindextoy98 = (int)(rows136-1) - fblinearindextoy98;
			float fboffsety98 = fblinearindextoy98 * fbrowsoffset98;
			float2 fboffset98 = float2(fboffsetx98, fboffsety98);
			half2 fbuv98 = (i.uv_tex4coord).xy * fbtiling98 + fboffset98;
			float4 tex2DNode1 = tex2D( _MainTex, fbuv98 );
			float4 temp_cast_2 = (_MainSoftnessMin).xxxx;
			float4 temp_cast_3 = (_MainSoftnessMax).xxxx;
			float4 smoothstepResult233 = smoothstep( temp_cast_2 , temp_cast_3 , tex2DNode1);
			#ifdef _MAINTEXSMOOTHSTEP_ON
				float4 staticSwitch236 = smoothstepResult233;
			#else
				float4 staticSwitch236 = tex2DNode1;
			#endif
			float4 break152 = staticSwitch236;
			float Frames126 = temp_output_18_0;
			float temp_output_133_0 = ( Frames126 - 1.0 );
			float smoothstepResult23 = smoothstep( temp_output_133_0 , temp_output_133_0 , AnimFrame4);
			float lerp156 = smoothstepResult23;
			float lerpResult20 = lerp( break152.r , break152.g , lerp156);
			float Frames243 = ( Frames126 * 2.0 );
			float temp_output_123_0 = ( Frames243 - 1.0 );
			float smoothstepResult24 = smoothstep( temp_output_123_0 , temp_output_123_0 , AnimFrame4);
			float lerp257 = smoothstepResult24;
			float lerpResult21 = lerp( lerpResult20 , break152.b , lerp257);
			float Frames344 = ( Frames126 * 3.0 );
			float temp_output_124_0 = ( Frames344 - 1.0 );
			float smoothstepResult25 = smoothstep( temp_output_124_0 , temp_output_124_0 , AnimFrame4);
			float lerp358 = smoothstepResult25;
			float lerpResult22 = lerp( lerpResult21 , break152.a , lerp358);
			float smoothstepResult173 = smoothstep( 0.0 , _AlphaSoftness , lerpResult22);
			float clampResult166 = clamp( ( ( depthFadeAlpha163 * smoothstepResult173 ) - ( 1.0 - i.vertexColor.a ) ) , 0.0 , 1.0 );
			#ifdef _ALPHADISSOLVE_ON
				float staticSwitch159 = ( _Color.a * clampResult166 );
			#else
				float staticSwitch159 = ( ( _Color.a * i.vertexColor.a ) * depthFadeAlpha163 * smoothstepResult173 );
			#endif
			float finalAlpha248 = staticSwitch159;
			#ifdef _ALPHAEMISSIONDISSOLVESUB_ON
				float staticSwitch246 = ( i.uv_tex4coord.w - ( finalAlpha248 * _EmissionSubValue ) );
			#else
				float staticSwitch246 = i.uv_tex4coord.w;
			#endif
			float4 temp_cast_4 = (staticSwitch246).xxxx;
			float4 clampResult197 = clamp( ( smoothstepResult193 - temp_cast_4 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			#ifdef _EMISSIONDISSOLVE_ON
				float4 staticSwitch177 = ( _Emission * clampResult197 );
			#else
				float4 staticSwitch177 = ( _Emission * i.uv_tex4coord.w );
			#endif
			float smoothstepResult258 = smoothstep( _FinalEmissionSmoothstepMin , _FinalEmissionSmoothstepMax , staticSwitch159);
			#ifdef _FINALEMISSIONSMOOTHSTEP_ON
				float staticSwitch276 = smoothstepResult258;
			#else
				float staticSwitch276 = staticSwitch159;
			#endif
			#ifdef _EMISSIONALPHA_ON
				float4 staticSwitch240 = ( staticSwitch276 * staticSwitch177 );
			#else
				float4 staticSwitch240 = staticSwitch177;
			#endif
			o.Emission = staticSwitch240.rgb;
			float smoothstepResult252 = smoothstep( _FinalAlphaSmoothstepMin , _FinalAlphaSmoothstepMax , staticSwitch159);
			#ifdef _FINALALPHASMOOTHSTEP_ON
				float staticSwitch275 = smoothstepResult252;
			#else
				float staticSwitch275 = finalAlpha248;
			#endif
			o.Alpha = staticSwitch275;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
-1885;7;1839;1004;-373.9534;-78.34583;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;134;-4202.213,708.7427;Inherit;False;Property;_Rows;Rows;0;0;Create;True;0;0;False;0;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-4219.035,567.627;Inherit;False;Property;_Columns;Columns;1;0;Create;True;0;0;False;0;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;135;-4024.213,606.7427;Inherit;False;columns;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;136;-3999.213,728.7427;Inherit;False;rows;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-3960.801,-110.4;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;138;-3742.213,663.7427;Inherit;False;136;rows;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-3715.213,570.7427;Inherit;False;135;columns;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-3476.581,586.8773;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RoundOpNode;122;-3697.72,3.856701;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;4;-3480.801,-33.39998;Inherit;False;AnimFrame;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;103;-3232.833,548.3247;Inherit;False;ChannelFramesCount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;-3037.313,135.0355;Inherit;False;103;ChannelFramesCount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;-3090.28,-45.1772;Inherit;False;4;AnimFrame;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;112;-2755.486,-146.6782;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;16;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;113;-2556.085,-48.1782;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-3001.968,777.5865;Inherit;False;Frames1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-2383.785,-75.5782;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;-2675.5,-272.1573;Inherit;False;136;rows;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;139;-2704.5,-358.1573;Inherit;False;135;columns;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;144;-3648.429,-169.1767;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-2673.201,806.7868;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;204;-2117.219,77.18805;Inherit;True;Property;_MainTex;MainTex;3;0;Create;True;0;0;False;0;None;de5a169e4d144e84ba887530ec866d75;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-3835.442,1586.618;Inherit;False;26;Frames1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;98;-2101.218,-238.2752;Inherit;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-2651.201,940.7868;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;-2517.201,815.7868;Inherit;False;Frames2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1843.427,-297.8819;Inherit;True;Property;_Tex;Tex;0;0;Create;True;0;0;False;0;-1;None;f5a3e0d69c865b5439c9bd0e50d9141a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;234;-1540.528,8.183472;Inherit;False;Property;_MainSoftnessMin;MainSoftnessMin;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;-3864.675,1877.818;Inherit;False;43;Frames2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;133;-3603.111,1565.098;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-2423.201,945.7868;Inherit;False;Frames3;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;235;-1476.528,82.18347;Inherit;False;Property;_MainSoftnessMax;MainSoftnessMax;6;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-3798.73,1416.279;Inherit;False;4;AnimFrame;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;23;-3449.742,1440.318;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;123;-3597.268,1868.176;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-3806.73,1721.279;Inherit;False;4;AnimFrame;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-3848.175,2168.319;Inherit;False;44;Frames3;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;233;-1323.528,-153.8165;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;-3742.73,2011.279;Inherit;False;4;AnimFrame;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;124;-3598.558,2111.958;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;24;-3394.442,1727.618;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;236;-1274.528,-246.8165;Inherit;False;Property;_MainTexSmoothstep;MainTexSmoothstep;4;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-3234.93,1501.176;Inherit;False;lerp1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;152;-1063.649,-19.47003;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-3172.53,1765.076;Inherit;False;lerp2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-1491.582,968.5979;Inherit;False;Property;_DepthSoftness;DepthSoftness;8;0;Create;True;0;0;False;0;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;25;-3408.442,2000.618;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-953.7399,194.5367;Inherit;False;56;lerp1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;20;-738.5215,4.716976;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-3093.23,2039.376;Inherit;False;lerp3;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;-781.7399,300.5367;Inherit;False;57;lerp2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;142;-1253.282,905.098;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;146;-1005.782,832.337;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;21;-519.5215,134.717;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;62;-667.7399,397.5367;Inherit;False;58;lerp3;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-475.1115,542.3913;Inherit;False;Property;_AlphaSoftness;AlphaSoftness;7;0;Create;True;0;0;False;0;0;0.578;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;22;-365.0283,290.8066;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;163;-795.183,903.2671;Inherit;False;depthFadeAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;164;-393.2097,14.70349;Inherit;False;163;depthFadeAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;173;-167.1115,354.3913;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;127;-1056.864,-476.2369;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;167;-324.7333,-161.2263;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;-60.55688,113.2626;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;165;78.87601,24.77582;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;125;-1041.864,-701.2369;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;-470.7218,-322.6705;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;166;257.876,23.77582;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;278;-2461.589,1584.963;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;172;388.4871,-86.409;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;55.12355,-269.4049;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;283;-2045.836,1514.259;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;237;-2277.438,1271.754;Inherit;False;0;192;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;281;-1878.836,1405.259;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;159;484.6583,-221.2462;Inherit;False;Property;_AlphaDissolve;AlphaDissolve;9;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;248;775.607,-53.26995;Inherit;False;finalAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;277;-1604.589,1582.963;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;279;-1448.589,1399.963;Inherit;False;Property;_EliminateEmissionRotation;EliminateEmissionRotation;28;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;250;-672.8932,1136.73;Inherit;False;Property;_EmissionSubValue;EmissionSubValue;25;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;249;-690.8932,1056.73;Inherit;False;248;finalAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;271;-1254.033,1573.312;Inherit;False;Property;_EmissionSpeed;EmissionSpeed;27;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;238;-948.0377,1401.354;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;-368.8932,1057.73;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;170;-481.7773,864.3997;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;247;-134.3784,1051.624;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;-445.8678,1503.202;Inherit;False;Property;_EmissionSoftness2;EmissionSoftness2;14;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;192;-485.8678,1182.202;Inherit;True;Property;_EmissionTex;EmissionTex;12;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;195;-485.8678,1399.202;Inherit;False;Property;_EmissionSoftness1;EmissionSoftness1;13;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;-2453.211,1961.536;Inherit;False;135;columns;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;246;45.62158,934.624;Inherit;False;Property;_AlphaEmissionDissolveSub;Alpha Emission Dissolve Sub;26;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;193;-87.8678,1201.202;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;225;-2447.702,2119.007;Inherit;False;136;rows;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-2163.839,1965.316;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;228;-2080.336,2303.065;Inherit;False;4;AnimFrame;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;196;379.1322,1038.202;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;227;-2156.839,2122.316;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;232;-2247.079,1790.732;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;168;-248.219,627.3657;Inherit;False;Property;_Emission;Emission;10;1;[HDR];Create;True;0;0;False;0;0,0,0,0;170.1817,63.80706,13.64665,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;257;156.8456,465.3385;Inherit;False;Property;_FinalEmissionSmoothstepMax;FinalEmissionSmoothstepMax;21;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;197;526.1322,1016.202;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;230;-1519.603,2324.986;Inherit;False;Property;_NormalScale;NormalScale;24;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;256;179.8456,361.3385;Inherit;False;Property;_FinalEmissionSmoothstepMin;FinalEmissionSmoothstepMin;20;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;223;-1626.185,2094.693;Inherit;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SmoothstepOpNode;258;526.8457,368.3385;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;274;-1006.884,1899.545;Inherit;False;Constant;_Vector0;Vector 0;26;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;222;-1314.529,2101.159;Inherit;True;Property;_NormalMap;NormalMap;23;0;Create;True;0;0;False;0;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;229.2227,626.3997;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;190;385.2937,786.7416;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;253;186.5071,163.13;Inherit;False;Property;_FinalAlphaSmoothstepMin;FinalAlphaSmoothstepMin;16;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;177;473.4174,561.438;Inherit;False;Property;_EmissionDissolve;EmissionDissolve;11;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;255;163.5071,267.13;Inherit;False;Property;_FinalAlphaSmoothstepMax;FinalAlphaSmoothstepMax;17;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;276;805.6426,295.1938;Inherit;False;Property;_FinalEmissionSmoothstep;FinalEmissionSmoothstep;19;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;154;-459.6545,-651.055;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;221;-834.5293,1966.159;Inherit;False;Property;_NormalMapEnabled;Normal Map Enabled;22;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;155;-409.6545,-467.055;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SmoothstepOpNode;252;533.5071,170.13;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;206;-544.756,1888.111;Inherit;False;normals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-99.86377,-485.2369;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;1123.483,539.1605;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;129;157.1362,-411.2369;Inherit;False;True;True;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;207;1229.142,-49.15683;Inherit;False;206;normals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;284;1486.953,685.3458;Inherit;False;Property;_CullMode;Cull Mode;29;1;[Enum];Create;True;0;1;UnityEngine.Rendering.CullMode;True;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;240;1225.512,360.524;Inherit;False;Property;_EmissionAlpha;EmissionAlpha;18;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;273;-1204.884,1869.545;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;209;-1417.255,1893.461;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;275;1180.943,71.19385;Inherit;False;Property;_FinalAlphaSmoothstep;FinalAlphaSmoothstep;15;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1610.346,-77.11003;Float;False;True;-1;4;ASEMaterialInspector;0;0;Standard;Knife/Particle Channel Packed;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;284;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;135;0;12;0
WireConnection;136;0;134;0
WireConnection;18;0;137;0
WireConnection;18;1;138;0
WireConnection;122;0;3;3
WireConnection;4;0;122;0
WireConnection;103;0;18;0
WireConnection;112;0;104;0
WireConnection;112;1;121;0
WireConnection;113;0;112;0
WireConnection;26;0;18;0
WireConnection;114;0;113;0
WireConnection;114;1;121;0
WireConnection;144;0;3;0
WireConnection;40;0;26;0
WireConnection;98;0;144;0
WireConnection;98;1;139;0
WireConnection;98;2;140;0
WireConnection;98;4;114;0
WireConnection;41;0;26;0
WireConnection;43;0;40;0
WireConnection;1;0;204;0
WireConnection;1;1;98;0
WireConnection;133;0;27;0
WireConnection;44;0;41;0
WireConnection;23;0;52;0
WireConnection;23;1;133;0
WireConnection;23;2;133;0
WireConnection;123;0;47;0
WireConnection;233;0;1;0
WireConnection;233;1;234;0
WireConnection;233;2;235;0
WireConnection;124;0;49;0
WireConnection;24;0;53;0
WireConnection;24;1;123;0
WireConnection;24;2;123;0
WireConnection;236;1;1;0
WireConnection;236;0;233;0
WireConnection;56;0;23;0
WireConnection;152;0;236;0
WireConnection;57;0;24;0
WireConnection;25;0;54;0
WireConnection;25;1;124;0
WireConnection;25;2;124;0
WireConnection;20;0;152;0
WireConnection;20;1;152;1
WireConnection;20;2;60;0
WireConnection;58;0;25;0
WireConnection;142;0;143;0
WireConnection;146;0;142;0
WireConnection;21;0;20;0
WireConnection;21;1;152;2
WireConnection;21;2;61;0
WireConnection;22;0;21;0
WireConnection;22;1;152;3
WireConnection;22;2;62;0
WireConnection;163;0;146;0
WireConnection;173;0;22;0
WireConnection;173;2;174;0
WireConnection;167;0;127;4
WireConnection;162;0;164;0
WireConnection;162;1;173;0
WireConnection;165;0;162;0
WireConnection;165;1;167;0
WireConnection;158;0;125;4
WireConnection;158;1;127;4
WireConnection;166;0;165;0
WireConnection;172;0;125;4
WireConnection;172;1;166;0
WireConnection;131;0;158;0
WireConnection;131;1;164;0
WireConnection;131;2;173;0
WireConnection;283;0;278;2
WireConnection;283;1;278;2
WireConnection;281;0;237;0
WireConnection;281;1;283;0
WireConnection;159;1;131;0
WireConnection;159;0;172;0
WireConnection;248;0;159;0
WireConnection;277;0;281;0
WireConnection;277;2;278;1
WireConnection;279;1;237;0
WireConnection;279;0;277;0
WireConnection;238;0;279;0
WireConnection;238;2;271;0
WireConnection;251;0;249;0
WireConnection;251;1;250;0
WireConnection;247;0;170;4
WireConnection;247;1;251;0
WireConnection;192;1;238;0
WireConnection;246;1;170;4
WireConnection;246;0;247;0
WireConnection;193;0;192;0
WireConnection;193;1;195;0
WireConnection;193;2;194;0
WireConnection;226;0;224;0
WireConnection;196;0;193;0
WireConnection;196;1;246;0
WireConnection;227;0;225;0
WireConnection;197;0;196;0
WireConnection;223;0;232;0
WireConnection;223;1;226;0
WireConnection;223;2;227;0
WireConnection;223;4;228;0
WireConnection;258;0;159;0
WireConnection;258;1;256;0
WireConnection;258;2;257;0
WireConnection;222;1;223;0
WireConnection;222;5;230;0
WireConnection;171;0;168;0
WireConnection;171;1;170;4
WireConnection;190;0;168;0
WireConnection;190;1;197;0
WireConnection;177;1;171;0
WireConnection;177;0;190;0
WireConnection;276;1;159;0
WireConnection;276;0;258;0
WireConnection;154;0;125;0
WireConnection;221;1;274;0
WireConnection;221;0;222;0
WireConnection;155;0;127;0
WireConnection;252;0;159;0
WireConnection;252;1;253;0
WireConnection;252;2;255;0
WireConnection;206;0;221;0
WireConnection;126;0;154;0
WireConnection;126;1;155;0
WireConnection;241;0;276;0
WireConnection;241;1;177;0
WireConnection;129;0;126;0
WireConnection;240;1;177;0
WireConnection;240;0;241;0
WireConnection;275;1;248;0
WireConnection;275;0;252;0
WireConnection;0;0;129;0
WireConnection;0;1;207;0
WireConnection;0;2;240;0
WireConnection;0;9;275;0
ASEEND*/
//CHKSM=8872C16061A211BA1E19684507C587F6DAB7157F