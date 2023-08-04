// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SoundVisualisation"
{
	Properties{
		_RefDistortion("Refraction Distortion", Range(0,5)) = 0.5
		_Speed("Propagation Speed", Range(0, 1000)) = 1
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

			uniform float _Speed;
			uniform float _RefDistortion;
            int _SegmentCount;
			float _Segments[1000];
            
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

            float Remap(float value, float oldLow, float oldHigh, float newLow, float newHigh)
            {
	            return (value - oldLow) / (oldHigh - oldLow) * (newHigh - newLow) + newLow;
            }
            
			fixed4 frag(v2f i) : SV_Target
			{
				// define constants
				const float PI = 3.14159;
				const int frequency = 4;
				const float A = 1;
				const float B = PI;
				const float C = 0;

				float rt = frac(_Time[0] * _Speed) * 0.5;
				float totalRadius = Remap(
					sin(B * (rt-C)),
					0, 1, 0, 0.5 
				);

				// grab texture from behind the object
				fixed4 col = tex2Dproj(_GrabTexture, i.uv);

				float2 objectPos = i.objectPosition.xy;
				float distanceFromCenter = sqrt(dot(i.objectPosition.xy, i.objectPosition.xy));
				bool inCircle = distanceFromCenter < totalRadius;
				if (inCircle)
				{
					/*
					float t = length(relativePos) + _Time[0] * _Speed;
					float t = frac(_Time[1]);
					float x = A * sin(B*(t - C));
					float y = A * cos(B*(t - C));
					 */
					/*
					 * 
					x = Remap(x, -1, 1, 0, 1);
					y = Remap(y, -1, 1, 0, 1);
					 */
					
					float2 relativePos = objectPos / totalRadius;
					float distortion = 0.5*distanceFromCenter / totalRadius;
					float4 fragmentDisplacement = float4(0, 0, 0, 0);
					fragmentDisplacement.xy += relativePos * distortion;
					
					// float sphereIndex = floor(frequency * distanceFromCenter / totalRadius);
					col = tex2Dproj(_GrabTexture, i.uv + fragmentDisplacement);
					col.r = distortion;
				}
				return col;
			}
		    ENDCG
        }
    }
}
