Shader "Unlit/Lightning"
{
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
            #pragma geometry geom
			#pragma fragment frag
            #pragma target 5.0
			
			#include "UnityCG.cginc"

            struct v2g
            {
                float id : ANY;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Lines
            {
                float2 start;
                float2 end;
                float thickness;
                int isEdge;
            };

            #include "./CreateLine.cginc"

            float4 _Color;
            StructuredBuffer<Lines> _Lines;
            int _Length;

            v2g vert (uint id : SV_VertexID, uint inst : SV_InstanceID)
			{
                v2g o;

                o.id = id;

				return o;
			}
			
            [maxvertexcount(128)]
            void geom(point v2g input[1], inout TriangleStream<g2f> outStream)
            {
                v2g d = input[0];

                Lines l = _Lines[d.id];
    
                float3 start = float3(l.start.x, 0.0, l.start.y);
                float3 end = float3(l.end.x, 0.0, l.end.y);

                if (l.isEdge == 0)
                {
                    AppendRoundedLine(start, end, float3(0.0, 1.0, 0.0), l.thickness, outStream);
                }
                else
                {
                    AppendRoundedLine(start, end, float3(0.0, 1.0, 0.0), l.thickness, outStream);
                }
            }

			float4 frag (g2f i) : SV_Target
			{
                return _Color;
			}
			ENDCG
		}
	}
}
