Shader "Custom/GlitchShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DisplacementTex ("Displacement Texture", 2D) = "white" {}
        _DisplaceStrength ("Displacement Strength", Range (0,1)) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DisplacementTex;
            float _DisplaceStrength;

            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float4 frag(v2f_img i) : COLOR
            {
                float2 displacementVector = random(float2(sin(_Time.x / 5000), floor(i.uv.y * 32) / 32) / 10);
                float4 col = tex2D(_MainTex, i.uv * 1 + ((displacementVector + random(float2(_Time.x, i.uv.y)) / 3) / 10) * _DisplaceStrength);
                if (_DisplaceStrength > 0) col = col * float4(col.x + (0 + (1 - _DisplaceStrength)), (0 + (1 - _DisplaceStrength)), (0 + (1 - _DisplaceStrength)), 1);
                col = col + float4(random(float2(sin(_Time.x / 5000), floor(i.uv.y * 32) / 32) / 5) * _DisplaceStrength / 5, 0, 0, 0);
                return col;
            }
            ENDCG
        }
    }
}
