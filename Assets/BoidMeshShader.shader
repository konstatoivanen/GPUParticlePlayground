Shader "Hidden/BoidMeshShader" 
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

			v2f vert(uint id : SV_VertexID, uint i : SV_InstanceID) 
			{
				v2f o;

				float2 coord = 0;

				[branch] switch (id) 
				{
					case 0: coord = float2(0, 1);	break;
					case 1: coord = float2(1, 1);	break;
					case 2: coord = float2(1, 0);	break;

					case 3: coord = float2(1, 0);   break;
					case 4: coord = float2(0, 0);	break;
					case 5: coord = float2(0, 1);	break;
				};

				coord -= 0.5;
				coord *= 2;

				Boid b = BoidBuffer[i];

				float2 scale = 0.01;
				scale.x     /= _ScreenParams.x / _ScreenParams.y;

				float2 pos = b.pos;

				pos /= _ScreenParams.xy * 0.5;
				pos -= 1;
				pos += coord * scale;

				o.pos   = float4(pos, 0, 1);
				o.col   = b.col;
				o.coord = coord;

				return o;
			}

			float4 frag(v2f i) : COLOR
			{ 
				float l = length(i.coord);

				clip(1 - l);
				
				return i.col;
			}

			ENDCG
		}
	}
	FallBack Off
}