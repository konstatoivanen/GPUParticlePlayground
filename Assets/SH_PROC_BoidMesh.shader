Shader "Hidden/BoidMesh" 
{
	SubShader 
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			CGPROGRAM 
			#pragma  target 5.0
			#pragma  vertex vert
			#pragma  fragment frag

			struct Boid 
			{
				float2 pos;
				float2 dir;
				float4 col;
			};

			StructuredBuffer<Boid> BoidBuffer;

			struct v2f
			{
				float4 pos		: SV_POSITION;
				float4 col		: COLOR0;
				float2 coord	: TEXCOORD0;
			};

			//returns a 2d rotation matrix from a direction
			float2x2 getRotationMatrix(float2 dir)
			{
				dir = normalize(dir);

				//Calculate Signed angle of the direction relative to 2d up vector (0,1)
				float a = -acos(clamp(dir.y, -1, 1)) *sign( - dir.x);
				float s;
				float c;

				sincos(a, s, c);

				return float2x2(c, -s, s, c);
			}
			//returns a matrix that transforms a screen space coordinate to clip space
			float3x3 getClipSpaceMatrix()
			{
				return float3x3(1 / (_ScreenParams.x * 0.5), 0, -1, 0, 1 / (_ScreenParams.y * 0.5), -1, 0, 0, 1);
			}

			v2f vert(uint id : SV_VertexID, uint i : SV_InstanceID) 
			{
				v2f o;

				float2 coord = 0;

				//Switch for determining vertex positions from vertex id
				[branch] switch (id) 
				{
					case 0: coord = float2(-1,  1);	break;
					case 1: coord = float2( 1,  1);	break;
					case 2: coord = float2( 1, -1);	break;

					case 3: coord = float2( 1, -1); break;
					case 4: coord = float2(-1, -1);	break;
					case 5: coord = float2(-1,  1);	break;
				};

				Boid b = BoidBuffer[i];

				//Aspect ratio corrected scale
				float2 clipScale = float2(0.01 / (_ScreenParams.x / _ScreenParams.y), 0.01);
				//Local space scale of the boid with streching along the up vector based on current speed
				//1 - e^-x never reaches 1 so it is useful for speed based streching as the boids don't have a cap to their speed.
				float2 localScale = float2(1, 1 + (1 - pow(2.718, -0.002 * length(b.dir))) * 4);

				//lets use matrices for the sake of matrices even though this would probably be cheaper without
				//calculate transformationData
				float3x3 clipMatrix = getClipSpaceMatrix();
				float2x2 rotMatrix	= getRotationMatrix(b.dir);

				//Transfrom screen space position to clip space with rotation
				o.pos   = float4(mul(clipMatrix, float3(b.pos,1)).xy + mul(coord * localScale, rotMatrix) * clipScale, 0, 1);
				o.col   = b.col;
				o.coord = coord;
				return o;
			}

			float4 frag(v2f i) : COLOR
			{ 
				//return if current pixel is beyond circle radius
				clip(1 - length(i.coord.xy));
				return i.col;
			}

			ENDCG
		}
	}
	FallBack Off
}