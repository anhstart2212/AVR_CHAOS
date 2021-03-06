Shader "TextMeshPro/Bitmap" {
	Properties {
		_MainTex ("Font Atlas", 2D) = "white" {}
		_FaceTex ("Font Texture", 2D) = "white" {}
		_FaceColor ("Text Color", Vector) = (1,1,1,1)
		_VertexOffsetX ("Vertex OffsetX", Float) = 0
		_VertexOffsetY ("Vertex OffsetY", Float) = 0
		_MaskSoftnessX ("Mask SoftnessX", Float) = 0
		_MaskSoftnessY ("Mask SoftnessY", Float) = 0
		_ClipRect ("Clip Rect", Vector) = (-32767,-32767,32767,32767)
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
	}
	SubShader {
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ColorMask 0 -1
			ZClip Off
			ZWrite Off
			Cull Off
			Stencil {
				ReadMask 0
				WriteMask 0
				Comp Disabled
				Pass Keep
				Fail Keep
				ZFail Keep
			}
			Fog {
				Mode Off
			}
			GpuProgramID 14523
			Program "vp" {
				SubProgram "d3d9 " {
					"!!vs_3_0
					
					//
					// Generated by Microsoft (R) HLSL Shader Compiler 10.1
					//
					// Parameters:
					//
					//   float4 _ClipRect;
					//   float4 _FaceColor;
					//   float4 _FaceTex_ST;
					//   float _MaskSoftnessX;
					//   float _MaskSoftnessY;
					//   float4 _ScreenParams;
					//   float _VertexOffsetX;
					//   float _VertexOffsetY;
					//   row_major float4x4 glstate_matrix_mvp;
					//   row_major float4x4 glstate_matrix_projection;
					//
					//
					// Registers:
					//
					//   Name                      Reg   Size
					//   ------------------------- ----- ----
					//   glstate_matrix_mvp        c0       4
					//   glstate_matrix_projection c4       2
					//   _ScreenParams             c6       1
					//   _FaceTex_ST               c7       1
					//   _FaceColor                c8       1
					//   _VertexOffsetX            c9       1
					//   _VertexOffsetY            c10      1
					//   _ClipRect                 c11      1
					//   _MaskSoftnessX            c12      1
					//   _MaskSoftnessY            c13      1
					//
					
					    vs_3_0
					    def c14, 0.5, 0.000244140625, 4096, 0.001953125
					    def c15, -2e+010, 2e+010, 2, 0.25
					    dcl_position v0
					    dcl_color v1
					    dcl_texcoord v2
					    dcl_texcoord1 v3
					    dcl_position o0
					    dcl_color o1
					    dcl_texcoord o2.xy
					    dcl_texcoord1 o3.xy
					    dcl_texcoord2 o4
					    add r0.x, c9.x, v0.x
					    add r0.y, c10.x, v0.y
					    mul r0.z, c14.x, v0.w
					    rcp r1.x, c6.x
					    rcp r1.y, c6.y
					    mad r0.xy, r0.z, r1, r0
					    mov r0.zw, v0
					    dp4 r3.z, c2, r0
					    dp4 r1.x, c0, r0
					    dp4 r1.y, c1, r0
					    dp4 r0.z, c3, r0
					    rcp r0.w, r0.z
					    mul r1.xy, r0.w, r1
					    mov r2.xw, c14
					    mul r1.zw, r2.x, c6.xyxy
					    mad r1.xy, r1, r1.zwzw, c14.x
					    frc r2.xy, r1
					    add r1.xy, r1, -r2
					    rcp r2.x, r1.z
					    rcp r2.y, r1.w
					    mul r1.xy, r1, r2
					    mul r3.xy, r0.z, r1
					    mul o1, c8, v1
					    mul r0.w, c14.y, v3.x
					    frc r1.x, r0.w
					    add r1.x, r0.w, -r1.x
					    mad r1.y, r1.x, -c14.z, v3.x
					    mul r1.xy, r1, c7
					    mad o3.xy, r1, r2.w, c7.zwzw
					    mov r1.xw, c15
					    max r2, r1.x, c11
					    min r2, r2, c15.y
					    mad r0.xy, r0, c15.z, -r2
					    add o4.xy, -r2.zwzw, r0
					    mov r0.xy, c6
					    mul r0.x, r0.x, c4.x
					    rcp r1.x, r0_abs.x
					    mul r0.x, r0.y, c5.y
					    rcp r1.y, r0_abs.x
					    mul r0.x, r1.w, c12.x
					    mul r0.y, r1.w, c13.x
					    mad r0.xy, r0.z, r1, r0
					    mov r3.w, r0.z
					    rcp r0.z, r0.x
					    rcp r0.w, r0.y
					    mul o4.zw, r0, c15.w
					    mov o2.xy, v2
					    mad o0.xy, r3.w, c255, r3
					    mov o0.zw, r3
					
					// approximately 49 instruction slots used"
				}
				SubProgram "d3d11 " {
					"!!vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[2];
						vec4 _FaceTex_ST;
						vec4 _FaceColor;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 _ClipRect;
						float _MaskSoftnessX;
						float _MaskSoftnessY;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[6];
						vec4 _ScreenParams;
						vec4 unused_1_2[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 glstate_matrix_mvp;
						vec4 unused_2_1[18];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[13];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_COLOR0;
					in  vec2 in_TEXCOORD0;
					in  vec2 in_TEXCOORD1;
					out vec4 vs_COLOR0;
					out vec2 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					vec2 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec2 u_xlat6;
					void main()
					{
					    u_xlat0.x = in_POSITION0.w * 0.5;
					    u_xlat0.xy = u_xlat0.xx / _ScreenParams.xy;
					    u_xlat6.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat0.xy = u_xlat0.xy + u_xlat6.xy;
					    u_xlat1 = u_xlat0.yyyy * glstate_matrix_mvp[1];
					    u_xlat1 = glstate_matrix_mvp[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat1 = glstate_matrix_mvp[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat6.xy = u_xlat1.xy / u_xlat1.ww;
					    u_xlat1.xy = _ScreenParams.xy * vec2(0.5, 0.5);
					    u_xlat6.xy = u_xlat6.xy * u_xlat1.xy;
					    u_xlat6.xy = roundEven(u_xlat6.xy);
					    u_xlat6.xy = u_xlat6.xy / u_xlat1.xy;
					    gl_Position.xy = u_xlat1.ww * u_xlat6.xy;
					    gl_Position.zw = u_xlat1.zw;
					    vs_COLOR0 = in_COLOR0 * _FaceColor;
					    u_xlat6.x = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat6.x = floor(u_xlat6.x);
					    u_xlat6.y = (-u_xlat6.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat6.xy = u_xlat6.xy * _FaceTex_ST.xy;
					    vs_TEXCOORD1.xy = u_xlat6.xy * vec2(0.001953125, 0.001953125) + _FaceTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
					    u_xlat2 = max(_ClipRect, vec4(-2e+10, -2e+10, -2e+10, -2e+10));
					    u_xlat2 = min(u_xlat2, vec4(2e+10, 2e+10, 2e+10, 2e+10));
					    u_xlat0.xy = u_xlat0.xy * vec2(2.0, 2.0) + (-u_xlat2.xy);
					    vs_TEXCOORD2.xy = (-u_xlat2.zw) + u_xlat0.xy;
					    u_xlat6.x = _ScreenParams.x * glstate_matrix_projection[0].x;
					    u_xlat6.y = _ScreenParams.y * glstate_matrix_projection[1].y;
					    u_xlat0.xy = u_xlat1.ww / abs(u_xlat6.xy);
					    u_xlat0.xy = vec2(_MaskSoftnessX, _MaskSoftnessY) * vec2(0.25, 0.25) + u_xlat0.xy;
					    vs_TEXCOORD2.zw = vec2(0.25, 0.25) / u_xlat0.xy;
					    return;
					}"
				}
				SubProgram "d3d11_9x " {
					"!!vs_4_0_level_9_1
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[2];
						vec4 _FaceTex_ST;
						vec4 _FaceColor;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 _ClipRect;
						float _MaskSoftnessX;
						float _MaskSoftnessY;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[6];
						vec4 _ScreenParams;
						vec4 unused_1_2[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 glstate_matrix_mvp;
						vec4 unused_2_1[18];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[13];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_COLOR0;
					in  vec2 in_TEXCOORD0;
					in  vec2 in_TEXCOORD1;
					out vec4 vs_COLOR0;
					out vec2 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					vec2 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec2 u_xlat6;
					void main()
					{
					    u_xlat0.x = in_POSITION0.w * 0.5;
					    u_xlat0.xy = u_xlat0.xx / _ScreenParams.xy;
					    u_xlat6.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat0.xy = u_xlat0.xy + u_xlat6.xy;
					    u_xlat1 = u_xlat0.yyyy * glstate_matrix_mvp[1];
					    u_xlat1 = glstate_matrix_mvp[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat1 = glstate_matrix_mvp[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat6.xy = u_xlat1.xy / u_xlat1.ww;
					    u_xlat1.xy = _ScreenParams.xy * vec2(0.5, 0.5);
					    u_xlat6.xy = u_xlat6.xy * u_xlat1.xy;
					    u_xlat6.xy = roundEven(u_xlat6.xy);
					    u_xlat6.xy = u_xlat6.xy / u_xlat1.xy;
					    gl_Position.xy = u_xlat1.ww * u_xlat6.xy;
					    gl_Position.zw = u_xlat1.zw;
					    vs_COLOR0 = in_COLOR0 * _FaceColor;
					    u_xlat6.x = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat6.x = floor(u_xlat6.x);
					    u_xlat6.y = (-u_xlat6.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat6.xy = u_xlat6.xy * _FaceTex_ST.xy;
					    vs_TEXCOORD1.xy = u_xlat6.xy * vec2(0.001953125, 0.001953125) + _FaceTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
					    u_xlat2 = max(_ClipRect, vec4(-2e+10, -2e+10, -2e+10, -2e+10));
					    u_xlat2 = min(u_xlat2, vec4(2e+10, 2e+10, 2e+10, 2e+10));
					    u_xlat0.xy = u_xlat0.xy * vec2(2.0, 2.0) + (-u_xlat2.xy);
					    vs_TEXCOORD2.xy = (-u_xlat2.zw) + u_xlat0.xy;
					    u_xlat6.x = _ScreenParams.x * glstate_matrix_projection[0].x;
					    u_xlat6.y = _ScreenParams.y * glstate_matrix_projection[1].y;
					    u_xlat0.xy = u_xlat1.ww / abs(u_xlat6.xy);
					    u_xlat0.xy = vec2(_MaskSoftnessX, _MaskSoftnessY) * vec2(0.25, 0.25) + u_xlat0.xy;
					    vs_TEXCOORD2.zw = vec2(0.25, 0.25) / u_xlat0.xy;
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d9 " {
					"!!ps_3_0
					
					//
					// Generated by Microsoft (R) HLSL Shader Compiler 10.1
					//
					// Parameters:
					//
					//   float4 _ClipRect;
					//   sampler2D _FaceTex;
					//   sampler2D _MainTex;
					//
					//
					// Registers:
					//
					//   Name         Reg   Size
					//   ------------ ----- ----
					//   _ClipRect    c0       1
					//   _MainTex     s0       1
					//   _FaceTex     s1       1
					//
					
					    ps_3_0
					    dcl_color_pp v0
					    dcl_texcoord v1.xy
					    dcl_texcoord1 v2.xy
					    dcl_texcoord2 v3
					    dcl_2d s0
					    dcl_2d s1
					    add r0.xy, -c0, c0.zwzw
					    add r0.xy, r0, -v3_abs
					    mul_sat_pp r0.xy, r0, v3.zwzw
					    mul_pp r0.x, r0.y, r0.x
					    texld r1, v2, s1
					    mul_pp r1.xyz, r1, v0
					    texld_pp r2, v1, s0
					    mul_pp r1.w, r2.w, v0.w
					    mul_pp oC0, r0.x, r1
					
					// approximately 9 instruction slots used (2 texture, 7 arithmetic)"
				}
				SubProgram "d3d11 " {
					"!!ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[5];
						vec4 _ClipRect;
						vec4 unused_0_2;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					in  vec4 vs_COLOR0;
					in  vec2 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec2 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat10_1;
					vec4 u_xlat10_2;
					void main()
					{
					    u_xlat0.xy = (-_ClipRect.xy) + _ClipRect.zw;
					    u_xlat0.xy = u_xlat0.xy + -abs(vs_TEXCOORD2.xy);
					    u_xlat0.xy = u_xlat0.xy * vs_TEXCOORD2.zw;
					    u_xlat0.xy = clamp(u_xlat0.xy, 0.0, 1.0);
					    u_xlat0.x = u_xlat0.y * u_xlat0.x;
					    u_xlat10_1 = texture(_FaceTex, vs_TEXCOORD1.xy);
					    u_xlat1.xyz = u_xlat10_1.xyz * vs_COLOR0.xyz;
					    u_xlat10_2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat1.w = u_xlat10_2.w * vs_COLOR0.w;
					    SV_Target0 = u_xlat0.xxxx * u_xlat1;
					    return;
					}"
				}
				SubProgram "d3d11_9x " {
					"!!ps_4_0_level_9_1
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[5];
						vec4 _ClipRect;
						vec4 unused_0_2;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					in  vec4 vs_COLOR0;
					in  vec2 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec2 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat10_1;
					vec4 u_xlat10_2;
					void main()
					{
					    u_xlat0.xy = (-_ClipRect.xy) + _ClipRect.zw;
					    u_xlat0.xy = u_xlat0.xy + -abs(vs_TEXCOORD2.xy);
					    u_xlat0.xy = u_xlat0.xy * vs_TEXCOORD2.zw;
					    u_xlat0.xy = clamp(u_xlat0.xy, 0.0, 1.0);
					    u_xlat0.x = u_xlat0.y * u_xlat0.x;
					    u_xlat10_1 = texture(_FaceTex, vs_TEXCOORD1.xy);
					    u_xlat1.xyz = u_xlat10_1.xyz * vs_COLOR0.xyz;
					    u_xlat10_2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat1.w = u_xlat10_2.w * vs_COLOR0.w;
					    SV_Target0 = u_xlat0.xxxx * u_xlat1;
					    return;
					}"
				}
			}
		}
	}
	CustomEditor "TMPro.EditorUtilities.TMP_BitmapShaderGUI"
}