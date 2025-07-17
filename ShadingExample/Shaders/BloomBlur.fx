texture2D ScreenTexture;
float2 res = float2(1920,1080);
sampler2D ScreenTextureSampler = sampler_state
{
    Texture = <ScreenTexture>;
    magfilter = LINEAR; 
    minfilter = LINEAR; 
    mipfilter= LINEAR; 
    AddressU = clamp; 
    AddressV = clamp;
};
struct PixelShaderInput
{
    float2 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;//position on spritesheet
    float4 color : COLOR0;
};
//Just an average of 4 values.
float4 Box4(float4 p0, float4 p1, float4 p2, float4 p3)
{
	return (p0 + p1 + p2 + p3) * 0.25f;
}
//Downsample to the next mip, blur in the process
float4 DownsamplePS(PixelShaderInput input) : COLOR0
{
    float2 inverseRes = float2(1.0/res.x,1.0/res.y);

    float2 offset = float2(2 * inverseRes.x, 2 * inverseRes.y);
    
    float4 c0 = tex2D(ScreenTextureSampler, input.texCoord + float2(-2, -2) * offset );
    float4 c1 = tex2D(ScreenTextureSampler, input.texCoord + float2(0,-2)*offset );
    float4 c2 = tex2D(ScreenTextureSampler, input.texCoord + float2(2, -2) * offset );
    float4 c3 = tex2D(ScreenTextureSampler, input.texCoord + float2(-1, -1) * offset );
    float4 c4 = tex2D(ScreenTextureSampler, input.texCoord + float2(1, -1) * offset );
    float4 c5 = tex2D(ScreenTextureSampler, input.texCoord + float2(-2, 0) * offset );
    float4 c6 = tex2D(ScreenTextureSampler, input.texCoord );
    float4 c7 = tex2D(ScreenTextureSampler, input.texCoord + float2(2, 0) * offset );
    float4 c8 = tex2D(ScreenTextureSampler, input.texCoord + float2(-1, 1) * offset );
    float4 c9 = tex2D(ScreenTextureSampler, input.texCoord + float2(1, 1) * offset );
    float4 c10 = tex2D(ScreenTextureSampler, input.texCoord + float2(-2, 2) * offset );
    float4 c11 = tex2D(ScreenTextureSampler, input.texCoord + float2(0, 2) * offset );
    float4 c12 = tex2D(ScreenTextureSampler, input.texCoord + float2(2, 2) * offset );

    return Box4(c0, c1, c5, c6) * 0.125f +
    Box4(c1, c2, c6, c7) * 0.125f +
    Box4(c5, c6, c10, c11) * 0.125f +
    Box4(c6, c7, c11, c12) * 0.125f +
    Box4(c3, c4, c8, c9) * 0.5f;


}


technique Blur
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 DownsamplePS();  
    }
}