#if OPENGL
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif
//! this is what you _spriteBatch.Draw() to the rendertarget in between _spriteBatch.Begin() and _spriteBatch.End()
float playerShadeRadius = 320;//!set in monogame
float2 playerPos = float2(100, 100);//!set in monogame
Texture2D RenderTargetTexture;
//	This is the sampler for the render target texture.
sampler2D RenderTargetSampler = sampler_state
{
	Texture = <RenderTargetTexture>;
};

struct PixelShaderInput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;//position on spritesheet
    float4 color : COLOR0;//color of the pixel of the current rendertarget
};

float4 MainPS(PixelShaderInput input) : COLOR0
{
    float4 pixelColor = tex2D(RenderTargetSampler, input.texCoord);
    float2 inputPos = float2(input.position.x, input.position.y);
    //calcualte distance of point from player, apply fade accoding to distance from player
    float2 dist = distance(playerPos, inputPos);
    float fadeVal = (1-(dist.x / playerShadeRadius )) + (1-(dist.y / playerShadeRadius )) * 0.5;

    if(fadeVal <= 1){ 
        pixelColor.r = fadeVal + pixelColor.r ;
        pixelColor.g = fadeVal + pixelColor.g;
        pixelColor.b = fadeVal + pixelColor.b;
        pixelColor.a = fadeVal + pixelColor.a;
    }else{
        pixelColor.a = 1;
    }


    if(pixelColor.a > 0){
        return float4(0, 0, 0, 1 - pixelColor.a);

    }else{
        return float4(0, 0, 0, 1);
    }
}

technique LightDrawing
{
	pass P0
	{
		PixelShader = compile ps_3_0 MainPS();
	}
};