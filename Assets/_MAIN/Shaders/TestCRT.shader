Shader "UI/TransparentStatic_Overlay"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _Intensity ("Noise Intensity", Range(0,1)) = 0.3
        _Speed ("Noise Speed", Range(0,10)) = 3.0
        _Alpha ("Overlay Alpha", Range(0,1)) = 0.3
        _EdgeSoftness ("Edge Softness", Range(0,1)) = 0.4
    }

    SubShader
    {
        Tags 
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanvasModulateColor"="True"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _Intensity;
            float _Speed;
            float _Alpha;
            float _EdgeSoftness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = _Time.y * _Speed;

                // generate static
                float n1 = random(i.uv * 100 + t);
                float n2 = random(i.uv * 200 - t * 1.3);
                float n3 = random(i.uv * 50 + t * 0.7);
                float noise = (n1 + n2 + n3) / 3.0;

                // only use noise for alpha variation
                float alphaNoise = abs(noise - 0.5) * 2.0 * _Intensity;

                // vignette edges
                float2 centeredUV = i.uv * 2.0 - 1.0;
                float dist = length(centeredUV);
                float vignette = smoothstep(1.0, 1.0 - _EdgeSoftness, dist);
                vignette = 1.0 - vignette;

                // combine everything
                float finalAlpha = _Alpha * alphaNoise * vignette;

                // color is transparent, we just use alpha
                fixed4 col = _Color;
                col.a = finalAlpha;

                return col;
            }
            ENDCG
        }
    }
}
