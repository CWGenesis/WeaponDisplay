sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
//sampler uImage3 : register(s3);

float4 allcolor;
float4x4 uTransform;
float uTime;
//float uSaturation;

struct VSInput {
	float2 Pos : POSITION0;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

struct PSInput {
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs((c.xxx + K.xyz - floor(c.xxx + K.xyz)) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float4 PixelShaderFunction(PSInput input) : COLOR0 {
	float3 coord = input.Texcoord;
	float y = uTime + coord.x;
	float4 c1 = tex2D(uImage1, float2(coord.x, coord.y));
	float4 c3 = tex2D(uImage2, float2(y, coord.y));
	c1 *= c3;
	float4 c = tex2D(uImage0, float2(c1.r, 0));
    /*int k = 9;
    float4 color[9] = { tex2D(uImage0, float2(0.5, 0.5)), tex2D(uImage0, float2(0.2, 0.8)), tex2D(uImage0, float2(0.9, 0.1)), tex2D(uImage0, float2(0.9, 0.5)), tex2D(uImage0, float2(0.5, 0.1)), tex2D(uImage0, float2(0.5, 0.65)), tex2D(uImage0, float2(0.65, 0.5)), tex2D(uImage0, float2(0.5, 0.35)), tex2D(uImage0, float2(0.35, 0.5)) };
    float4 allcolor = (0, 0, 0, 0);
	for (int n = 0; n < 9; n++)
    {
        if (!any(color[n]))
            k--;
		else
            allcolor += color[n];
    }
    allcolor = allcolor / k;
    //饱和度,通不过编译
    int parameter = 0;
    if (allcolor.r < allcolor.g)
    {
        parameter = 2;
        if (allcolor.g < allcolor.b)
        {
            parameter = 3;
        }
    }
    else
    {
        parameter = 1;
        if (allcolor.r < allcolor.b)
        {
            parameter = 3;
        }
    }
    if (parameter == 1)
    {
        if (allcolor.g < allcolor.b)
        {
            float median = (1 - allcolor.g / allcolor.r) * allcolor.b;
            float k = 1 - uSaturation;
            allcolor = float4(allcolor.r, allcolor.r * k, (allcolor.r - median) * k + median, allcolor.a);
        }
        else
        {
            float median = (1 - allcolor.b / allcolor.r) * allcolor.g;
            float k = 1 - uSaturation;
            allcolor = float4(allcolor.r, (allcolor.r - median) * k + median, allcolor.r * k, allcolor.a);
        }
    }
    else if (parameter == 2)
    {
        if (allcolor.r < allcolor.b)
        {
            float1 median = (1 - allcolor.r / allcolor.g) * allcolor.b;
            allcolor = (allcolor.g * (1 - uSaturation),allcolor.g , (allcolor.g - median) * (1 - uSaturation) + median, allcolor.a);
        }
        else
        {
            float1 median = (1 - allcolor.b / allcolor.g) * allcolor.r;
            allcolor = ((allcolor.g - median) * (1 - uSaturation) + median, allcolor.a), allcolor.g,allcolor.g * (1 - uSaturation) ;
        }
    }
    else if (parameter == 3)
    {
        if (allcolor.r < allcolor.g)
        {
            float1 median = (1 - allcolor.r / allcolor.b) * allcolor.g;
            allcolor = (allcolor.b * (1 - uSaturation),(allcolor.b - median) * (1 - uSaturation) + median ,allcolor.b , allcolor.a);
        }
        else
        {
            float1 median = (1 - allcolor.g / allcolor.b) * allcolor.r;
            allcolor = ((allcolor.b - median) * (1 - uSaturation) + median, allcolor.b * (1 - uSaturation), allcolor.b, allcolor.a);
        }
    }
    /*float max = allcolor.r;
    int pMax = 1;
    float min = allcolor.g;
    int pMin = 2;
    float median = allcolor.b;
    int pMedian = 3;
    if (allcolor.r < allcolor.g)
    {
        parameter = allcolor.g;
        min = allcolor.r;
        pMin = 1;
    }
    else
    {
        parameter = allcolor.r;
        //min = allcolor.g;
    }
    if (parameter < allcolor.b)
    {
        max = allcolor.b;
        pMax = 3;
        median = parameter;
        pMedian = 6 - pMax - pMin;
    }
    else 
    {
        max = parameter;
        if (allcolor.b < min)
        {
            median = min;
            min = allcolor.b;
        }
        else
        {
            median = allcolor.b;
        }
    }*/
    if (c.r < 0.1)
		return float4(0, 0, 0, 0);
	return 2 * c * allcolor * coord.z;
}

PSInput VertexShaderFunction(VSInput input)  {
	PSInput output;
	output.Color = input.Color;
	output.Texcoord = input.Texcoord;
	output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
	return output;
}


technique Technique1 {
	pass ColorBar {
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}