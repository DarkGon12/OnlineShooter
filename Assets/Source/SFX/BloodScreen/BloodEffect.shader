Shader "Custom/BloodEffect"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BloodTex ("Blood Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _BloodTex;
        half _Intensity;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 blood = tex2D(_BloodTex, IN.uv_MainTex) * _Intensity;
            o.Albedo = c.rgb + blood.rgb * (1.0 - c.a);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
