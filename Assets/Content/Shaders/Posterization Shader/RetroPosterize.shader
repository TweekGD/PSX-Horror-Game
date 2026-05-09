Shader "Fullscreen/RetroPosterize"
{
    Properties
    {
        _PosterizeLevels("Levels", Float) = 4.0
        _DitherStrength("Dither Strength", Range(0, 1)) = 0.05
        _PixelSize("Pixel Size", Int) = 2
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            Name "PosterizePass"
            ZWrite Off Cull Off ZTest Always

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _PosterizeLevels;
            float _DitherStrength;
            int   _PixelSize;

            static const float4x4 bayerMatrix = float4x4(
                 0.0,  8.0,  2.0, 10.0,
                12.0,  4.0, 14.0,  6.0,
                 3.0, 11.0,  1.0,  9.0,
                15.0,  7.0, 13.0,  5.0
            ) / 16.0;

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = input.texcoord;

                if (_PixelSize > 1)
                {
                    float2 res = _ScreenParams.xy / _PixelSize;
                    uv = floor(uv * res) / res;
                }

                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                float2 pixelPos = uv * _ScreenParams.xy / (_PixelSize > 0 ? _PixelSize : 1);
                uint2  ditherUV = uint2(fmod(pixelPos.x, 4), fmod(pixelPos.y, 4));
                float  dither   = bayerMatrix[ditherUV.x][ditherUV.y] - 0.5;
                color.rgb += dither * _DitherStrength;

                color.rgb = floor(color.rgb * _PosterizeLevels) / _PosterizeLevels;

                return color;
            }
            ENDHLSL
        }
    }
}