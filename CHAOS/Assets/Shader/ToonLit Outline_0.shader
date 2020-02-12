Shader "Toon/Lit Outline" {
	Properties {
		_Color ("Main Color", Vector) = (0.5,0.5,0.5,1)
		_OutlineColor ("Outline Color", Vector) = (0,0,0,1)
		_Outline ("Outline width", Range(0.002, 0.03)) = 0.005
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		UsePass "Toon/Lit/FORWARD"
		UsePass "Toon/Basic Outline/OUTLINE"
	}
	Fallback "Toon/Lit"
}