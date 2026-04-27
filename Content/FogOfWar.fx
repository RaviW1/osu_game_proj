#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float2 PlayerScreenPos;
float InnerRadius;
float OuterRadius;

sampler2D SpriteTexture : register(s0);

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(
    float4 position : POSITION,
    float4 color    : COLOR0,
    float2 texCoord : TEXCOORD0)
{
    VertexShaderOutput output;
    output.Position = position;
    output.Color    = color;
    output.TexCoord = texCoord;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float4 scene = tex2D(SpriteTexture, input.TexCoord) * input.Color;

    float2 delta   = input.TexCoord - PlayerScreenPos;
    float  dist    = length(delta);
    float  fog     = smoothstep(InnerRadius, OuterRadius, dist);

    // Darken edges, full color center
    float3 result = scene.rgb * (1.0 - fog * 0.85);
    return float4(result, scene.a);
}

technique FogOfWar
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader  = compile PS_SHADERMODEL MainPS();
    }
}
