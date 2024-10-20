#if OPENGL
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float lightIntensity = 0.5;

//	This is the sampler for the render target texture.
Texture2D RenderTargetTexture;
sampler2D RenderTargetSampler = sampler_state
{
	Texture = <RenderTargetTexture>;
};
Texture2D MaskTextureLights;
sampler2D MaskSamplerLights= sampler_state
{
	Texture = <MaskTextureLights>;
};
struct PixelShaderInput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;//position on spritesheet
    float4 color : COLOR0;//color of the pixel of the current rendertarget 
	//(it's what you cleaned the graphics device to after setting rendertarget)
};

float4 MainPS(PixelShaderInput input) : COLOR0
{
    float4 pixelColor = tex2D(RenderTargetSampler, input.texCoord);
	float4 lightColor = tex2D(MaskSamplerLights, input.texCoord);

	return pixelColor  + (lightColor * lightIntensity);

}

technique LightDrawing
{
	pass P0
	{
		PixelShader = compile ps_3_0 MainPS();
	}
};