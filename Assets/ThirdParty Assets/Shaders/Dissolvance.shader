Shader "Custom/Dissolvance" {
	//Shader controls
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NoiseTexture("Noise Texutre", 2D) = "white" {}
		_DissolveState("Dissolve State", Range(0,1)) = 0
		[Toggle]_ToggleFade("Toggle Faded Edge", Range(0, 1)) = 0
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
		_EdgeSize("Edge Size", Range(0, 1)) = 0
	}

	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		Cull Off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows addshadow 

		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		sampler2D _NoiseTexture;
		half _DissolveState;
		fixed4 _EdgeColor;
		half _EdgeSize;
		float _ToggleFade;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			//Takes a noise image. And according to what the dissolve state is, it makes the darkest colors transparent
			half noise_value = tex2D(_NoiseTexture, IN.uv_MainTex).r;
			clip(noise_value - _DissolveState);

			//Colors the picture for the material after a selected color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			//Makes a dissolve edge
			half eS = _EdgeSize + _DissolveState;

			float thresh = eS;
			float useDissolve;
			//Decides whether or not if the edge should be faded into the main color
			if (_ToggleFade == 1) {
				useDissolve = thresh - noise_value;
				c = (1 - useDissolve) * c + useDissolve * _EdgeColor;
			}
			else {
				useDissolve = noise_value - thresh < 0;
				c = (1 - useDissolve) * c + useDissolve * _EdgeColor;
			}

			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}