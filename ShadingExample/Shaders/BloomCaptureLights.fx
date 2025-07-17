#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


texture2D ScreenTexture;
float redThreshold = 0.9f;
float greenThreshold = 0.9f;
float blueThreshold = 0.9f;
sampler2D ColorMapSampler = sampler_state
{
    Texture = <ScreenTexture>;
};
struct PixelShaderInput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;//position on spritesheet
    float4 color : COLOR0;
};

//Extracts the pixels we want to blur, but considers luminance instead of average rgb
float4 ExtractLuminancePS(PixelShaderInput input) : COLOR0
{
    float4 color= tex2D(ColorMapSampler, input.texCoord) * input.color;

    if(color.r > redThreshold || color.g > blueThreshold || color.b > greenThreshold)
    {
        return color ;
    }
    else
    {
        return float4(0,0,0,0);
    }
}


technique Bloom
{   
    pass P0  
    {   
		PixelShader = compile ps_3_0 ExtractLuminancePS();
    }   
} 