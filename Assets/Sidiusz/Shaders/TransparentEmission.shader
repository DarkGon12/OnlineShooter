Shader "Custom/IntersectionFadeShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (0,0,0,0)
        _FadeDistance ("Fade Distance", Range(0.01, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

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
                float4 worldPos : TEXCOORD1;
                float depth : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _EmissionColor;
            float _FadeDistance;

            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.depth = o.vertex.z / o.vertex.w;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.vertex.xy / i.vertex.w));
                float distance = abs(depth - i.depth);
                float alpha = saturate(distance / _FadeDistance);

                float4 texColor = tex2D(_MainTex, i.uv) * _Color;
                texColor.a *= alpha;

                float4 emission = _EmissionColor;
                emission.rgb *= emission.a;

                return texColor + emission;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
