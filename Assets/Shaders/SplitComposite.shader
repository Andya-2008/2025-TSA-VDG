Shader "Hidden/SplitComposite"
{
    Properties
    {
        _BallTex ("Ball", 2D) = "black" {}
        _PacmanTex ("Pacman", 2D) = "black" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _BallTex;
            sampler2D _PacmanTex;

            float2 _LineA;
            float2 _LineB;
            float _BlendWidth;
            float2 _ScreenSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float cross2d(float2 a, float2 b)
            {
                return a.x * b.y - a.y * b.x;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenPos = i.uv;

                float2 ab = _LineB - _LineA;
                float dist = cross2d(ab, screenPos - _LineA) / length(ab);

                float t = dist > 0 ? 1.0 : 0.0;

                fixed4 ball = tex2D(_BallTex, i.uv);
                fixed4 pac  = tex2D(_PacmanTex, i.uv);
                
                return lerp(ball, pac, t);
            }
            ENDCG
        }
    }
}