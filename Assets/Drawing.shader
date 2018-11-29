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
		
		float2 unpackNormal(float2 n)
		{
			return normalize(2 * (n - 0.5));
		}
		float2 packNormal(float2 n)
		{
			return saturate((n + 0.5) / 2);
		}
		half2  radianDirection(half radian)
		{
			return half2(cos(radian), sin(radian));
		}
		half2  blurDirection(int index, half length)
		{
			length = (index / length) * 6.28;
			return radianDirection(length);
		}
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
		float  getLength(float2 cp, float2 pc, float r)
		{
			return 1 - saturate(distance(pc, cp) / r);
		}

		fixed4 frag_draw (v2f i) : SV_Target
		{
			float2 pc = i.uv * _ScreenParams.xy;

			float4 prevImage = tex2D(_MainTex, i.uv);

			float cl = getLength(_CursorPos, pc, _BrushSize);
			float ca = step(0, cl) * _BrushDraw;

			if (ca < 0.01)
				return prevImage;

			float2 n = sampleHeightMap(_MainTex, i.uv, float2(0.01, 0.01 * (_ScreenParams.x / _ScreenParams.y)));

			return float4(n, max(cl, prevImage.b), 1) * ca;
		}

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