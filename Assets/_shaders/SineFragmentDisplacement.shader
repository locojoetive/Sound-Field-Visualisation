Shader "Unlit/SineFragmentDisplacement"
{
	// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

	// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

	Shader "Custom/SineFrahgmentDisplacement"
	{
		Properties{
			_RefPow("Refraction Distortion", Range(0,128)) = 15.0
			_Frequency("Amount of Circles", Int) = 2
			_Speed("Propagation Speed", Float) = 3
			_A("A", Float) = 1
			_B("B", Float) = 1
			_C("C", Float) = 1
			_D("D", Float) = 1
			_MaxPercentage("Amount of Color Displacement", Range(0, 1)) = 1
		}

		SubShader
		{
			Tags {"Queue" = "Transparent"}

			GrabPass {
				"_GrabTexture"
			}

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				uniform int _Frequency;
				uniform float _Speed;
				uniform float _MaxPercentage;
				uniform float _A;
				uniform float _B;
				uniform float _C;
				uniform float _D;

				sampler2D _GrabTexture;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float4 uv : TEXCOORD0;
					float4 objectPos: TEXCOORD3;
				};

				v2f vert(appdata v)
				{
					v2f output;
					output.vertex = UnityObjectToClipPos(v.vertex);
					output.objectPos = v.vertex;
					output.uv = ComputeGrabScreenPos(output.vertex);
					return output;
				}
				fixed4 frag(v2f i) : SV_Target
				{
					// define constants
					const float PI = 3.14159;
					float totalRadius = 0.5;
					int frequency = _Frequency;


					// define most outer circle
					float2 objectPos = mul(unity_WorldToObject, i.vertex).xy;
					float distanceFromCenter = sqrt(dot(i.objectPos.xy, i.objectPos.xy));
					float phaseDistanceBetweenSpheres = totalRadius / frequency;
					bool inCircle = distanceFromCenter < totalRadius;

					// grab texture from behind the object
					fixed4 col = tex2Dproj(_GrabTexture, i.uv);

					if (inCircle) {
						float4 displacedFragment = float4(0, 0, 0, 0);
						float A = _A;
						float B = _B;
						float C = _C;
						float D = _D;
						float x = i.vertex.x;
						float y = i.vertex.y;

						if (objectPos.x >= 0) {
							displacedFragment.x = A * sin(B*(x + C)) + D;
						}
						else {
							displacedFragment.x = -A * sin(B*(x + C)) - D;
						}
						if (objectPos.y >= 0) {
							displacedFragment.y = A * sin(B*(y + C)) + D;
						}
						else {
							displacedFragment.y = -A * sin(B*(y + C)) - D;
						}
						col = tex2Dproj(_GrabTexture, i.uv + displacedFragment);
						/*

						float phaseDisplacement = frac(_Time[1]/_Speed) * totalRadius/frequency;
						float radius = totalRadius;
						float4 displacedFragment = float4(0, 0, 0, 0);
						for (int sphereIndex = 0; sphereIndex <= frequency; sphereIndex++) {
							radius = sphereIndex * totalRadius / frequency + phaseDisplacement;
							inCircle = distanceFromCenter < radius;
							if (inCircle) {
								float percentage = distanceFromCenter / radius;

								break;
							}
						}
						*/

					}
					return col;

					//relevant for spheres
					/*
					float phi = acos(distanceFromCenter / totalRadius);
					float distanceFromSurface = sin(phi) / totalRadius;
					*/


					/* //things to keep in mind
					float4 sphereNormal = normalize(float4(i.objectPos.x, i.objectPos.y, 0, 1) + float4(0, 0, distanceFromSurface, 0));
					*/
				}
            ENDCG
        }
    }
}
