#ifndef CREATE_LINE_INCLUDED
#define CREATE_LINE_INCLUDED

#define CIRCLE_STEP_COUNT 12

void AppendLine(float3 from, float3 to, float3 up, float thickness, inout TriangleStream<g2f> outStream)
{
    float3 dir = normalize(to - from);
    float3 normal = normalize(cross(dir, up));
    
    thickness *= 0.5;
    float3 p0 = from - normal * thickness;
    float3 p1 = from + normal * thickness;
    float3 p2 = to - normal * thickness;
    float3 p3 = to + normal * thickness;

    g2f o;

    o.pos = mul(UNITY_MATRIX_VP, float4(p1, 1.0));
    o.uv = float2(1.0, 0.0);
    outStream.Append(o);

    o.pos = mul(UNITY_MATRIX_VP, float4(p3, 1.0));
    o.uv = float2(1.0, 1.0);
    outStream.Append(o);

    o.pos = mul(UNITY_MATRIX_VP, float4(p0, 1.0));
    o.uv = float2(0.0, 0.0);
    outStream.Append(o);

    o.pos = mul(UNITY_MATRIX_VP, float4(p2, 1.0));
    o.uv = float2(0.0, 1.0);
    outStream.Append(o);

    outStream.RestartStrip();
}

float3 RodriguesRotation(float3 v, float3 n, float theta)
{
    return mul(float3x3(
        float3(
            cos(theta) + n.x * n.x * (1.0 - cos(theta)),
            n.x * n.y * (1.0 - cos(theta)) - n.z * sin(theta),
            n.x * n.z * (1.0 - cos(theta)) + n.y * sin(theta)
        ),
        float3(
            n.y * n.x * (1.0 - cos(theta)) + n.z * sin(theta),
            cos(theta) + n.y * n.y * (1.0 - cos(theta)),
            n.y * n.z * (1.0 - cos(theta)) - n.x * sin(theta)
        ),
        float3(
            n.z * n.x * (1.0 - cos(theta)) - n.y * sin(theta),
            n.z * n.y * (1.0 - cos(theta)) + n.x * sin(theta),
            cos(theta) + n.z * n.z * (1.0 - cos(theta))
        )
    ), v);
}

void AppendHalfCircle(float3 center, float3 forward, float3 up, float radius, inout TriangleStream<g2f> outStream)
{
    float step = UNITY_PI / CIRCLE_STEP_COUNT;
    float3 normal = normalize(cross(forward, up));
    float3 p0 = normal * radius;

    g2f co;
    co.pos = mul(UNITY_MATRIX_VP, float4(center, 1.0));
    co.uv = 0;

    g2f o;
    for (int i = 1; i <= CIRCLE_STEP_COUNT; i++)
    {
        float3 p1 = RodriguesRotation(p0, up, step);

        o.pos = mul(UNITY_MATRIX_VP, float4(p0 + center, 1.0));
        o.uv = 0;

        outStream.Append(co);
        outStream.Append(o);

        o.pos = mul(UNITY_MATRIX_VP, float4(p1 + center, 1.0));

        outStream.Append(o);
        outStream.RestartStrip();

        p0 = p1;
    }
}

void AppendRoundedLine(float3 from, float3 to, float3 up, float thickness, inout TriangleStream<g2f> outStream)
{
    AppendLine(from, to, up, thickness, outStream);
    AppendHalfCircle(from, normalize(from - to), up, thickness * 0.5, outStream);
    AppendHalfCircle(to, normalize(to - from), up, thickness * 0.5, outStream);
}

#endif
