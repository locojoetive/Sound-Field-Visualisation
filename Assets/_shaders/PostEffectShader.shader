// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PostEffectShader"
{
	Properties{
		_RefDistortion("Refraction Distortion", Range(0,128)) = 15.0
		_Speed("Propagation Speed", Range(0, 1)) = 0.5
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
			uniform float _RefDistortion;
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
				float4 objectPosition: TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f output;
				output.vertex = UnityObjectToClipPos(v.vertex);
				output.objectPosition = v.vertex;
				output.uv = ComputeGrabScreenPos(output.vertex);
				return output;
            }
			fixed4 frag(v2f i) : SV_Target
			{
				// define constants
				const float PI = 3.14159;
				int frequency = 1;
				float totalRadius = 0.5;

				// define most outer circle
				float2 objectPos = i.objectPosition.xy;
				float distanceFromCenter = sqrt(dot(i.objectPosition.xy, i.objectPosition.xy));
				float phaseDistanceBetweenSpheres = 0;
				bool inCircle = distanceFromCenter < totalRadius;
				
				// grab texture from behind the object
				fixed4 col = tex2Dproj(_GrabTexture, i.uv);
				if (inCircle) {
					float4 fragmentDisplacement = float4(0, 0, 0, 0);
					float phaseDisplacement = frac(_Time[1] * _Speed) * totalRadius / frequency;
					float radius = 0;
					for (int sphereIndex = 0; sphereIndex <= frequency; sphereIndex++) {
						radius = sphereIndex * totalRadius / frequency + phaseDisplacement;
						inCircle = distanceFromCenter < radius;
						if (inCircle) {
							float percentage = distanceFromCenter / radius;
							float t = _Time[0];
							float amplitude = percentage * _RefDistortion;
							float B = 2 * PI / (totalRadius / frequency);
							float C = phaseDisplacement;
							float sine = sin(B*(t - C)) / 100;
							if (objectPos.x >= 0) {
								fragmentDisplacement.x = amplitude * sine;
							}
							else {
								fragmentDisplacement.x = -amplitude * sine;
							}
							if (objectPos.y >= 0) {
								fragmentDisplacement.y = amplitude * sine;
							}
							else {
								fragmentDisplacement.y = -amplitude * sine;
							}

							col = tex2Dproj(_GrabTexture, i.uv + fragmentDisplacement);
							col.r = 1 - percentage;
							break;
						}
					}
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
