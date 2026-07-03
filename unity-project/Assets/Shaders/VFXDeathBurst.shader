Shader "Custom/VFX/DeathBurst"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint Color", Color) = (1, 0.45, 0.15, 1)
        _RingIntensity("Ring Intensity", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        LOD 100

        Pass
        {
            ZWrite Off
            Blend One One // Additive blending for glow effect

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _RingIntensity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Distance from UV center for ring/shockwave effect
                half2 uvCenter = i.texcoord - 0.5;
                float distFromCenter = length(uvCenter);

                // Ring mask: stronger at edges, weaker in center
                float ringMask = smoothstep(0.15, 0.5, distFromCenter);
                ringMask = lerp(1.0, ringMask, _RingIntensity);

                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                fixed4 col = texColor * _Color * i.color;
                col.rgb *= ringMask;

                return col;
            }

            ENDCG
        }
    }
}