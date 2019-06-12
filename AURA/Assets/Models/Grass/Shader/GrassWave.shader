Shader "GrassWave" {
	Properties{
		_Color("Main Color r:ampl g:speed b:time", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.5

		_WaveCycle("Wave Cycle", Range(0.0,5.0)) = 1.0
		_WaveAmount("Wave Amount", Range(0,0.1)) = 0.02
	}

		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200
		Lighting Off
		Cull Off


		CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert

		sampler2D _MainTex;
	float4 _Color;
	float _WaveCycle;
	float _WaveAmount;

	struct Input {
		float2 uv_MainTex;
		fixed4 color : COLOR;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = _Color * tex2D(_MainTex, float2(IN.uv_MainTex));
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}

	void vert(inout appdata_full v) {
		float4 p = v.vertex;

		float sy = p.y;
		p.x += sin((_Time.y * _WaveCycle)) *  _WaveAmount * sy;
		p.z += cos((_Time.y * _WaveCycle)) *  _WaveAmount * sy;

		v.vertex = p;
	}

	ENDCG
	}
}