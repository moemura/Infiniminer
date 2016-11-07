struct VertexToPixel
{
    float4 Position   	: POSITION;
    float2 TextureCoords: TEXCOORD0;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};

//------- XNA-to-HLSL variables --------
uniform extern float4x4 xView : VIEW;
uniform extern float4x4 xWorld : WORLD;
uniform extern float4x4 xProjection : PROJECTION;
uniform extern float xTime;
uniform extern Texture xTexture;

//------- Texture Samplers --------

sampler2D NoiseSampler = sampler_state {
	Texture = <xTexture>;
	MagFilter = POINT;
	MinFilter = ANISOTROPIC;
	MipFilter = NONE;
	AddressU = WRAP;
	AddressV = WRAP;
};

//------- Technique: Textured --------

VertexToPixel TexturedVS( float4 inPos : POSITION, float2 inTexCoords: TEXCOORD0)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection);	
	Output.TextureCoords = inTexCoords;
	    
	return Output;    
}

PixelToFrame TexturedPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;
	
	float2 direction = float2(0.5, 1.0);		
	float2 uv = PSIn.TextureCoords / 20;
	float t = xTime / 8000;
	float4 perlin = 0;
    perlin += tex2D(NoiseSampler, 4 * (uv + 8 * t * direction)) / 2;
    perlin += tex2D(NoiseSampler, 8 * (uv + 4 * t * direction)) / 4;
    perlin += tex2D(NoiseSampler, 16 * (uv + 2 * t * direction)) / 8;
    perlin += tex2D(NoiseSampler, 32 * (uv + 1 * t * direction)) / 16;
	Output.Color = 1 - pow(1-perlin,2);

	//Output.Color = tex2D(NoiseSampler, PSIn.TextureCoords);
	return Output;
}

technique Skyplane
{
	pass Pass0
	{   
		VertexShader = compile vs_3_0 TexturedVS();
		PixelShader  = compile ps_3_0 TexturedPS();
	}
}
