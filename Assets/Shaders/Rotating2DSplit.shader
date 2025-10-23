Shader "Custom/Rotating2DSplit"
{
    Properties {
        _MainTexA ("Texture A", 2D) = "white" {}
        _MainTexB ("Texture B", 2D) = "white" {}
        _Angle ("Split Angle", Range(0,360)) = 0
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Feather ("Feather", Range(0,0.2)) = 0.02
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass {
            ZWrite Off
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTexA, _MainTexB;
            float _Angle;
            float4 _Center;
            float _Feather;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 center = _Center.xy;
                float2 dir = i.uv - center;

                float rad = radians(_Angle);
                float2x2 rot = float2x2(cos(rad), -sin(rad), sin(rad), cos(rad));
                float2 rotated = mul(rot, dir);

                float edge = rotated.x;
                float t = smoothstep(-_Feather, _Feather, edge);

                fixed4 a = tex2D(_MainTexA, i.uv);
                fixed4 b = tex2D(_MainTexB, i.uv);
                return lerp(a, b, t);
            }
            ENDCG
        }
    }
}
