Shader "Custom/ScanlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineMultiplier ("Scanline Multiplier", Float) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 scr_pos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.scr_pos = ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            float _ScanlineMultiplier;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float2 ps = i.scr_pos.xy * _ScreenParams.xy / i.scr_pos.w;
                uint scanline = (uint)ps.y % 2;
                if (scanline == 0) col *= _ScanlineMultiplier;
                return col;
            }
            ENDCG
        }
    }
}
