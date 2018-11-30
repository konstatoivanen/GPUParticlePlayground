Shader "Hidden/Drawing"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off 
		ZWrite Off 
		ZTest Always
		
		CGINCLUDE

		uniform sampler2D _MainTex;
		uniform sampler2D _DrawBuffer;
		uniform float	  _BrushSize;
		uniform int		  _BrushDraw;
		uniform float2	  _CursorPos;

		struct v2f
		{
			float2 uv		: TEXCOORD0;
			float4 vertex	: SV_POSITION;
		};

		v2f vert (float4 vertex : POSITION, float2 uv : TEXCOORD0)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(vertex);
			o.uv     = uv;
			return o;
		}
		
		//Unpacks a 0-1 space normal into -1 - 1 space
		float2 unpackNormal(float2 n)
		{
			return normalize(2 * (n - 0.5));
		}
		//Packs a -1 - 1 normal into 0 - 1 space
		float2 packNormal(float2 n)
		{
			return saturate((n + 0.5) / 2);
		}
		//Converts a radian to a 2d direction
		half2  radianDirection(half radian)
		{
			return half2(cos(radian), sin(radian));
		}
		//Returns a direction that is rotated by interpolating between 0 - length by the interpolant of index.
		half2  blurDirection(int index, half length)
		{
			length = (index / length) * 6.28;
			return radianDirection(length);
		}

		//Samples a 0-1 height map into a -1 - 1 normal
		float2 sampleHeightMap(sampler2D source, float2 uv, float2 offs)
		{
			float c = tex2D(source, uv).b;
			float v = 0;

			float2 n = 0;
			float2 d = 0;

			[unroll]
			for (int i = 0; i < 16; ++i)
			{
				d = blurDirection(i, 16);
				v = tex2D(source, uv + d * offs).b;
				v = v - c;
				n += d * v;
			}

			return (normalize(n) + 1) / 2;
		}

		//Returns the distance to brush edge from current pixel coordinate in pixels
		float  signedDistance(float2 cp, float2 pc, float r)
		{
			return r - distance(pc, cp);
		}

		//Main Drawing method
		fixed4 frag_draw (v2f i) : SV_Target
		{
			float2 pc = i.uv * _ScreenParams.xy;

			float4 prevImage = tex2D(_MainTex, i.uv);

			float cl = signedDistance(_CursorPos, pc, _BrushSize) * _BrushDraw;
			float2 n = sampleHeightMap(_MainTex, i.uv, float2(0.01, 32 / _ScreenParams.x));

			return float4(n, max(cl, prevImage.b), 1);
		}

		//Copy method that clips normals outside of brush radius
		fixed4 frag_copy (v2f i) : SV_Target
		{
			float4 col = tex2D(_MainTex, i.uv);

			col.xy *= step(0.01, col.z);

			col.a = 1;

			return col;
		}

		ENDCG

		Pass
		{
			CGPROGRAM
			#pragma  vertex vert
			#pragma  fragment frag_draw
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			#pragma  vertex vert
			#pragma  fragment frag_copy
			ENDCG
		}
	}
}