Shader "Custom/UICanvasBlur"
{
    Properties
    {
        _BlurAmount ("Blur Amount", Range(0, 20)) = 1
        _Iterations ("Iterations", Range(1, 8)) = 4
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent+100" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0

        #include "UnityCG.cginc"
        #include "UnityUI.cginc"

        struct appdata_t
        {
            float4 vertex   : POSITION;
            float4 color    : COLOR;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4 vertex   : SV_POSITION;
            fixed4 color    : COLOR;
            float4 worldPosition : TEXCOORD0;
            float4 uvgrab : TEXCOORD1;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        float4 _ClipRect;
        float _BlurAmount;
        int _Iterations;

        v2f vert(appdata_t IN)
        {
            v2f OUT;
            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            OUT.worldPosition = IN.vertex;
            OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
            OUT.uvgrab = ComputeGrabScreenPos(OUT.vertex);
            OUT.color = IN.color;
            return OUT;
        }

        ENDCG

        // Horizontal blur pass
        GrabPass
        {
            "_HorizontalBlur"
        }

        Pass
        {
            CGPROGRAM
            sampler2D _HorizontalBlur;

            half4 frag(v2f IN) : SV_Target
            {
                float4 uvgrab = IN.uvgrab;
                half4 color = 0;
                float weight = 1.0 / (2 * _Iterations + 1);

                for(int i = -_Iterations; i <= _Iterations; i++)
                {
                    float offset = _BlurAmount * _GrabTexture_TexelSize.x * i;
                    color += tex2Dproj(_HorizontalBlur, uvgrab + float4(offset, 0, 0, 0)) * weight;
                }

                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                color *= IN.color;
                return color;
            }
            ENDCG
        }

        // Vertical blur pass
        GrabPass
        {
            "_VerticalBlur"
        }

        Pass
        {
            CGPROGRAM
            sampler2D _VerticalBlur;

            half4 frag(v2f IN) : SV_Target
            {
                float4 uvgrab = IN.uvgrab;
                half4 color = 0;
                float weight = 1.0 / (2 * _Iterations + 1);

                for(int i = -_Iterations; i <= _Iterations; i++)
                {
                    float offset = _BlurAmount * _GrabTexture_TexelSize.y * i;
                    color += tex2Dproj(_VerticalBlur, uvgrab + float4(0, offset, 0, 0)) * weight;
                }

                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                color *= IN.color;
                return color;
            }
            ENDCG
        }
    }
}