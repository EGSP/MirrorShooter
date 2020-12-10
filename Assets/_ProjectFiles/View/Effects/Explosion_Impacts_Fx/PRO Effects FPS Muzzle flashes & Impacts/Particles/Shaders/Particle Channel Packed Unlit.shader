// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Knife/Particle Channel Packed Unlit"
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
		_DepthSoftness("DepthSoftness", Float) = 1
		[Toggle(_ALPHADISSOLVE_ON)] _AlphaDissolve("AlphaDissolve", Float) = 0
		[HDR]_Emission("Emission", Color) = (0,0,0,0)
		[Toggle(_EMISSIONDISSOLVE_ON)] _EmissionDissolve("EmissionDissolve", Float) = 0
		_EmissionTex("EmissionTex", 2D) = "white" {}
		_EmissionSpeed("EmissionSpeed", Vector) = (0,0,0,0)
		_EmissionSoftness1("EmissionSoftness1", Range( 0 , 1)) = 0
		_EmissionSoftness2("EmissionSoftness2", Range( 0 , 1)) = 0
		[Toggle(_FINALALPHASMOOTHSTEP_ON)] _FinalAlphaSmoothstep("FinalAlphaSmoothstep", Float) = 0
		_FinalAlphaSmoothstepMin("FinalAlphaSmoothstepMin", Range( 0 , 1)) = 0
		_FinalAlphaSmoothstepMax("FinalAlphaSmoothstepMax", Range( 0 , 1)) = 1
		[Toggle(_EMISSIONALPHA_ON)] _EmissionAlpha("EmissionAlpha", Float) = 0
		[Toggle(_FINALEMISSIONSMOOTHSTEP_ON)] _FinalEmissionSmoothstep("FinalEmissionSmoothstep", Float) = 0
		_FinalEmissionSmoothstepMin("FinalEmissionSmoothstepMin", Range( 0 , 1)) = 0
		_FinalEmissionSmoothstepMax("FinalEmissionSmoothstepMax", Range( 0 , 1)) = 1
		_EmissionSubValue("EmissionSubValue", Range( 0 , 1)) = 0
		[Toggle(_ALPHAEMISSIONDISSOLVESUB_ON)] _AlphaEmissionDissolveSub("Alpha Emission Dissolve Sub", Float) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		
		
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
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature _EMISSIONALPHA_ON
			#pragma shader_feature _EMISSIONDISSOLVE_ON
			#pragma shader_feature _ALPHAEMISSIONDISSOLVESUB_ON
			#pragma shader_feature _ALPHADISSOLVE_ON
			#pragma shader_feature _MAINTEXSMOOTHSTEP_ON
			#pragma shader_feature _FINALEMISSIONSMOOTHSTEP_ON
			#pragma shader_feature _FINALALPHASMOOTHSTEP_ON


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
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
			};

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
			uniform sampler2D _MainTex;
			uniform float _Columns;
			uniform float _Rows;
			uniform float _MainSoftnessMin;
			uniform float _MainSoftnessMax;
			uniform float _EmissionSubValue;
			uniform float _FinalEmissionSmoothstepMin;
			uniform float _FinalEmissionSmoothstepMax;
			uniform float _FinalAlphaSmoothstepMin;
			uniform float _FinalAlphaSmoothstepMax;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_color = v.color;
				o.ase_texcoord1 = v.ase_texcoord;
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
				float4 uv0170 = i.ase_texcoord1;
				uv0170.xy = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float4 temp_cast_1 = (_EmissionSoftness1).xxxx;
				float4 temp_cast_2 = (_EmissionSoftness2).xxxx;
				float2 uv0_EmissionTex = i.ase_texcoord1.xy * _EmissionTex_ST.xy + _EmissionTex_ST.zw;
				float2 panner238 = ( 1.0 * _Time.y * _EmissionSpeed + uv0_EmissionTex);
				float4 smoothstepResult193 = smoothstep( temp_cast_1 , temp_cast_2 , tex2D( _EmissionTex, panner238 ));
				float4 screenPos = i.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth142 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
				float distanceDepth142 = abs( ( screenDepth142 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthSoftness ) );
				float clampResult146 = clamp( distanceDepth142 , 0.0 , 1.0 );
				float depthFadeAlpha163 = clampResult146;
				float4 uv03 = i.ase_texcoord1;
				uv03.xy = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float columns135 = _Columns;
				float rows136 = _Rows;
				float AnimFrame4 = round( uv03.z );
				float temp_output_18_0 = ( columns135 * rows136 );
				float ChannelFramesCount103 = temp_output_18_0;
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles98 = columns135 * rows136;
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset98 = 1.0f / columns135;
				float fbrowsoffset98 = 1.0f / rows136;
				// Speed of animation
				float fbspeed98 = _Time[ 1 ] * 0.0;
				// UV Tiling (col and row offset)
				float2 fbtiling98 = float2(fbcolsoffset98, fbrowsoffset98);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index
				float fbcurrenttileindex98 = round( fmod( fbspeed98 + ( frac( ( AnimFrame4 / ChannelFramesCount103 ) ) * ChannelFramesCount103 ), fbtotaltiles98) );
				fbcurrenttileindex98 += ( fbcurrenttileindex98 < 0) ? fbtotaltiles98 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox98 = round ( fmod ( fbcurrenttileindex98, columns135 ) );
				// Multiply Offset X by coloffset
				float fboffsetx98 = fblinearindextox98 * fbcolsoffset98;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy98 = round( fmod( ( fbcurrenttileindex98 - fblinearindextox98 ) / columns135, rows136 ) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy98 = (int)(rows136-1) - fblinearindextoy98;
				// Multiply Offset Y by rowoffset
				float fboffsety98 = fblinearindextoy98 * fbrowsoffset98;
				// UV Offset
				float2 fboffset98 = float2(fboffsetx98, fboffsety98);
				// Flipbook UV
				half2 fbuv98 = (uv03).xy * fbtiling98 + fboffset98;
				// *** END Flipbook UV Animation vars ***
				float4 tex2DNode1 = tex2D( _MainTex, fbuv98 );
				float4 temp_cast_3 = (_MainSoftnessMin).xxxx;
				float4 temp_cast_4 = (_MainSoftnessMax).xxxx;
				float4 smoothstepResult233 = smoothstep( temp_cast_3 , temp_cast_4 , tex2DNode1);
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
				float clampResult166 = clamp( ( ( depthFadeAlpha163 * lerpResult22 ) - ( 1.0 - i.ase_color.a ) ) , 0.0 , 1.0 );
				#ifdef _ALPHADISSOLVE_ON
				float staticSwitch159 = ( _Color.a * clampResult166 );
				#else
				float staticSwitch159 = ( ( _Color.a * i.ase_color.a ) * depthFadeAlpha163 * lerpResult22 );
				#endif
				float finalAlpha248 = staticSwitch159;
				#ifdef _ALPHAEMISSIONDISSOLVESUB_ON
				float staticSwitch246 = ( uv0170.w - ( finalAlpha248 * _EmissionSubValue ) );
				#else
				float staticSwitch246 = uv0170.w;
				#endif
				float4 temp_cast_5 = (staticSwitch246).xxxx;
				float4 clampResult197 = clamp( ( smoothstepResult193 - temp_cast_5 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
				#ifdef _EMISSIONDISSOLVE_ON
				float4 staticSwitch177 = ( _Emission * clampResult197 );
				#else
				float4 staticSwitch177 = ( _Emission * uv0170.w );
				#endif
				float smoothstepResult258 = smoothstep( _FinalEmissionSmoothstepMin , _FinalEmissionSmoothstepMax , staticSwitch159);
				#ifdef _FINALEMISSIONSMOOTHSTEP_ON
				float staticSwitch278 = smoothstepResult258;
				#else
				float staticSwitch278 = staticSwitch159;
				#endif
				#ifdef _EMISSIONALPHA_ON
				float4 staticSwitch240 = ( staticSwitch278 * staticSwitch177 );
				#else
				float4 staticSwitch240 = staticSwitch177;
				#endif
				float smoothstepResult252 = smoothstep( _FinalAlphaSmoothstepMin , _FinalAlphaSmoothstepMax , staticSwitch159);
				#ifdef _FINALALPHASMOOTHSTEP_ON
				float staticSwitch277 = smoothstepResult252;
				#else
				float staticSwitch277 = finalAlpha248;
				#endif
				float4 appendResult273 = (float4((staticSwitch240).rgb , staticSwitch277));
				
				
				finalColor = ( float4( (( (_Color).rgb * (i.ase_color).rgb )).xyz , 0.0 ) + appendResult273 );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18000
-1885;7;1839;1004;1353.614;661.5672;1.726254;True;False
Node;AmplifyShaderEditor.RangedFloatNode;12;-4219.035,567.627;Inherit;False;Property;_Columns;Columns;1;0;Create;True;0;0;False;0;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;134;-4202.213,708.7427;Inherit;False;Property;_Rows;Rows;0;0;Create;True;0;0;False;0;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;136;-3999.213,728.7427;Inherit;False;rows;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;135;-4024.213,606.7427;Inherit;False;columns;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-3715.213,570.7427;Inherit;False;135;columns;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;-3742.213,663.7427;Inherit;False;136;rows;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-3960.801,-110.4;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RoundOpNode;122;-3697.72,3.856701;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-3476.581,586.8773;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;103;-3232.833,548.3247;Inherit;False;ChannelFramesCount;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;4;-3480.801,-33.39998;Inherit;False;AnimFrame;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;-3090.28,-45.1772;Inherit;False;4;AnimFrame;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;-3037.313,135.0355;Inherit;False;103;ChannelFramesCount;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;112;-2755.486,-146.6782;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;16;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;113;-2556.085,-48.1782;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-3001.968,777.5865;Inherit;False;Frames1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;144;-3648.429,-169.1767;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-2383.785,-75.5782;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;140;-2675.5,-272.1573;Inherit;False;136;rows;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;139;-2704.5,-358.1573;Inherit;False;135;columns;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-2673.201,806.7868;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;204;-2117.219,77.18805;Inherit;True;Property;_MainTex;MainTex;3;0;Create;True;0;0;False;0;None;f5a3e0d69c865b5439c9bd0e50d9141a;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;-2517.201,815.7868;Inherit;False;Frames2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;98;-2101.218,-238.2752;Inherit;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;27;-3835.442,1586.618;Inherit;False;26;Frames1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-2651.201,940.7868;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-3798.73,1416.279;Inherit;False;4;AnimFrame;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;133;-3603.111,1565.098;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1843.427,-297.8819;Inherit;True;Property;_Tex;Tex;0;0;Create;True;0;0;False;0;-1;None;f5a3e0d69c865b5439c9bd0e50d9141a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;234;-1768.528,8.183472;Inherit;False;Property;_MainSoftnessMin;MainSoftnessMin;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;-3864.675,1877.818;Inherit;False;43;Frames2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-2423.201,945.7868;Inherit;False;Frames3;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;235;-1773.528,119.1835;Inherit;False;Property;_MainSoftnessMax;MainSoftnessMax;6;0;Create;True;0;0;False;0;1;0.611;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-3848.175,2168.319;Inherit;False;44;Frames3;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-3806.73,1721.279;Inherit;False;4;AnimFrame;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;233;-1412.528,-148.8165;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;23;-3449.742,1440.318;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;123;-3597.268,1868.176;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;24;-3394.442,1727.618;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-3234.93,1501.176;Inherit;False;lerp1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-1491.582,968.5979;Inherit;False;Property;_DepthSoftness;DepthSoftness;8;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;124;-3598.558,2111.958;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;-3742.73,2011.279;Inherit;False;4;AnimFrame;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;236;-1231.147,-248.5517;Inherit;False;Property;_MainTexSmoothstep;MainTexSmoothstep;4;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;142;-1253.282,905.098;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;152;-1063.649,-19.47003;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;60;-953.7399,194.5367;Inherit;False;56;lerp1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-3172.53,1765.076;Inherit;False;lerp2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;25;-3408.442,2000.618;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;20;-738.5215,4.716976;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;146;-1005.782,832.337;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;-781.7399,300.5367;Inherit;False;57;lerp2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-3093.23,2039.376;Inherit;False;lerp3;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;21;-519.5215,134.717;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;163;-795.183,903.2671;Inherit;False;depthFadeAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;62;-667.7399,397.5367;Inherit;False;58;lerp3;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;164;-393.2097,14.70349;Inherit;False;163;depthFadeAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;127;-1056.864,-476.2369;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;22;-365.0283,290.8066;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;-60.55688,113.2626;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;167;-324.7333,-161.2263;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;165;78.87601,24.77582;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;125;-1041.864,-701.2369;Inherit;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;166;257.876,23.77582;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;-470.7218,-322.6705;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;55.12355,-269.4049;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;172;388.4871,-86.409;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;159;458.6583,-204.2462;Inherit;False;Property;_AlphaDissolve;AlphaDissolve;9;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;248;747.5269,-106.4725;Inherit;False;finalAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;237;-1384.438,1345.754;Inherit;False;0;192;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;250;-672.8932,1136.73;Inherit;False;Property;_EmissionSubValue;EmissionSubValue;23;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;249;-690.8932,1056.73;Inherit;False;248;finalAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;271;-1254.033,1573.312;Inherit;False;Property;_EmissionSpeed;EmissionSpeed;13;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;170;-481.7773,864.3997;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;238;-948.0377,1401.354;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;251;-368.8932,1057.73;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;-445.8678,1503.202;Inherit;False;Property;_EmissionSoftness2;EmissionSoftness2;15;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;192;-485.8678,1182.202;Inherit;True;Property;_EmissionTex;EmissionTex;12;0;Create;True;0;0;False;0;-1;None;2140d5caeca76404cadd35cc48f45f10;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;195;-485.8678,1399.202;Inherit;False;Property;_EmissionSoftness1;EmissionSoftness1;14;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;247;-134.3784,1051.624;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;246;45.62158,934.624;Inherit;False;Property;_AlphaEmissionDissolveSub;Alpha Emission Dissolve Sub;24;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;193;-87.8678,1201.202;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;196;379.1322,1038.202;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;197;526.1322,1016.202;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;256;349.4456,327.7385;Inherit;False;Property;_FinalEmissionSmoothstepMin;FinalEmissionSmoothstepMin;21;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;168;-248.219,627.3657;Inherit;False;Property;_Emission;Emission;10;1;[HDR];Create;True;0;0;False;0;0,0,0,0;7.377211,2.858186,0.6952345,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;257;326.4456,431.7385;Inherit;False;Property;_FinalEmissionSmoothstepMax;FinalEmissionSmoothstepMax;22;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;190;385.2937,786.7416;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;258;696.4457,334.7385;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;171;229.2227,626.3997;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;177;473.4174,561.438;Inherit;False;Property;_EmissionDissolve;EmissionDissolve;11;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;278;855.9118,241.1099;Inherit;False;Property;_FinalEmissionSmoothstep;FinalEmissionSmoothstep;20;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;253;522.1071,72.73;Inherit;False;Property;_FinalAlphaSmoothstepMin;FinalAlphaSmoothstepMin;17;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;255;499.1071,176.73;Inherit;False;Property;_FinalAlphaSmoothstepMax;FinalAlphaSmoothstepMax;18;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;791.9829,579.4604;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;240;891.412,403.424;Inherit;False;Property;_EmissionAlpha;EmissionAlpha;19;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;252;815.1071,83.73;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;154;-459.6545,-651.055;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;155;-409.6545,-467.055;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;274;1144.743,347.8459;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;277;945.9118,-18.89011;Inherit;False;Property;_FinalAlphaSmoothstep;FinalAlphaSmoothstep;16;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-99.86377,-485.2369;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;273;1250.477,68.60117;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;129;157.1362,-411.2369;Inherit;False;True;True;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;199;-656.2374,1252.588;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;173;-167.1115,354.3913;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-475.1115,542.3913;Inherit;False;Property;_AlphaSoftness;AlphaSoftness;7;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;275;1343.683,-40.13361;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;276;752.0867,-498.3344;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;198;-908.2374,1207.588;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;272;1479.946,-44.81003;Float;False;True;-1;2;ASEMaterialInspector;100;1;Knife/Particle Channel Packed Unlit;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;False;0;False;-1;0;False;-1;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
WireConnection;136;0;134;0
WireConnection;135;0;12;0
WireConnection;122;0;3;3
WireConnection;18;0;137;0
WireConnection;18;1;138;0
WireConnection;103;0;18;0
WireConnection;4;0;122;0
WireConnection;112;0;104;0
WireConnection;112;1;121;0
WireConnection;113;0;112;0
WireConnection;26;0;18;0
WireConnection;144;0;3;0
WireConnection;114;0;113;0
WireConnection;114;1;121;0
WireConnection;40;0;26;0
WireConnection;43;0;40;0
WireConnection;98;0;144;0
WireConnection;98;1;139;0
WireConnection;98;2;140;0
WireConnection;98;4;114;0
WireConnection;41;0;26;0
WireConnection;133;0;27;0
WireConnection;1;0;204;0
WireConnection;1;1;98;0
WireConnection;44;0;41;0
WireConnection;233;0;1;0
WireConnection;233;1;234;0
WireConnection;233;2;235;0
WireConnection;23;0;52;0
WireConnection;23;1;133;0
WireConnection;23;2;133;0
WireConnection;123;0;47;0
WireConnection;24;0;53;0
WireConnection;24;1;123;0
WireConnection;24;2;123;0
WireConnection;56;0;23;0
WireConnection;124;0;49;0
WireConnection;236;1;1;0
WireConnection;236;0;233;0
WireConnection;142;0;143;0
WireConnection;152;0;236;0
WireConnection;57;0;24;0
WireConnection;25;0;54;0
WireConnection;25;1;124;0
WireConnection;25;2;124;0
WireConnection;20;0;152;0
WireConnection;20;1;152;1
WireConnection;20;2;60;0
WireConnection;146;0;142;0
WireConnection;58;0;25;0
WireConnection;21;0;20;0
WireConnection;21;1;152;2
WireConnection;21;2;61;0
WireConnection;163;0;146;0
WireConnection;22;0;21;0
WireConnection;22;1;152;3
WireConnection;22;2;62;0
WireConnection;162;0;164;0
WireConnection;162;1;22;0
WireConnection;167;0;127;4
WireConnection;165;0;162;0
WireConnection;165;1;167;0
WireConnection;166;0;165;0
WireConnection;158;0;125;4
WireConnection;158;1;127;4
WireConnection;131;0;158;0
WireConnection;131;1;164;0
WireConnection;131;2;22;0
WireConnection;172;0;125;4
WireConnection;172;1;166;0
WireConnection;159;1;131;0
WireConnection;159;0;172;0
WireConnection;248;0;159;0
WireConnection;238;0;237;0
WireConnection;238;2;271;0
WireConnection;251;0;249;0
WireConnection;251;1;250;0
WireConnection;192;1;238;0
WireConnection;247;0;170;4
WireConnection;247;1;251;0
WireConnection;246;1;170;4
WireConnection;246;0;247;0
WireConnection;193;0;192;0
WireConnection;193;1;195;0
WireConnection;193;2;194;0
WireConnection;196;0;193;0
WireConnection;196;1;246;0
WireConnection;197;0;196;0
WireConnection;190;0;168;0
WireConnection;190;1;197;0
WireConnection;258;0;159;0
WireConnection;258;1;256;0
WireConnection;258;2;257;0
WireConnection;171;0;168;0
WireConnection;171;1;170;4
WireConnection;177;1;171;0
WireConnection;177;0;190;0
WireConnection;278;1;159;0
WireConnection;278;0;258;0
WireConnection;241;0;278;0
WireConnection;241;1;177;0
WireConnection;240;1;177;0
WireConnection;240;0;241;0
WireConnection;252;0;159;0
WireConnection;252;1;253;0
WireConnection;252;2;255;0
WireConnection;154;0;125;0
WireConnection;155;0;127;0
WireConnection;274;0;240;0
WireConnection;277;1;248;0
WireConnection;277;0;252;0
WireConnection;126;0;154;0
WireConnection;126;1;155;0
WireConnection;273;0;274;0
WireConnection;273;3;277;0
WireConnection;129;0;126;0
WireConnection;199;0;198;1
WireConnection;199;1;198;3
WireConnection;173;0;22;0
WireConnection;173;2;174;0
WireConnection;275;0;129;0
WireConnection;275;1;273;0
WireConnection;276;0;168;0
WireConnection;276;1;22;0
WireConnection;272;0;275;0
ASEEND*/
//CHKSM=1235608C025165868BC9DB564E70D20768A10055