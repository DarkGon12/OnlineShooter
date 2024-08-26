Shader "Custom/Skybox/Procedural Clouds Skybox"
{
    Properties
    {
        _SkyColorHorizon ("Sky Color Horizon", Color) = (0.4, 0.6, 0.9, 1)
        _SkyColorZenith ("Sky Color Zenith", Color) = (0.2, 0.4, 0.8, 1)
        _CloudColor1 ("Cloud Color 1", Color) = (1, 1, 1, 1)
        _CloudColor2 ("Cloud Color 2", Color) = (0.8, 0.8, 0.8, 1)
        _CloudSpeed ("Cloud Speed", Float) = 0.1
        _CloudScale1 ("Cloud Scale 1", Float) = 3
        _CloudScale2 ("Cloud Scale 2", Float) = 1
        _CloudDensity ("Cloud Density", Range(0, 1)) = 0.35
        _CloudOpacity ("Cloud Opacity", Range(0, 1)) = 1
        _CloudHeight1 ("Cloud Height 1", Range(0, 2)) = 1.5
        _CloudHeight2 ("Cloud Height 2", Range(0, 2)) = 2
        _WindDirection ("Wind Direction", Vector) = (1, 0, 0, 0)
        _EraseNoiseScale ("Erase Noise Scale", Float) = 2.4
        _EraseNoiseStrength ("Erase Noise Strength", Range(0, 1)) = 0.45
        _EraseNoiseBlur ("Erase Noise Blur", Range(0, 1)) = 0.25
        _CloudFadeDistance ("Cloud Fade Distance", Range(0, 1)) = 1
    }
    
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            fixed4 _SkyColorHorizon;
            fixed4 _SkyColorZenith;
            fixed4 _CloudColor1;
            fixed4 _CloudColor2;
            float _CloudSpeed;
            float _CloudScale1;
            float _CloudScale2;
            float _CloudDensity;
            float _CloudOpacity;
            float _CloudHeight1;
            float _CloudHeight2;
            float4 _WindDirection;
            float _EraseNoiseScale;
            float _EraseNoiseStrength;
            float _EraseNoiseBlur;
            float _CloudFadeDistance;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            float noise(float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);
                float n = p.x + p.y * 157.0 + 113.0 * p.z;
                return lerp(lerp(lerp(frac(sin(n + 0.0) * 43758.5453),
                                     frac(sin(n + 1.0) * 43758.5453), f.x),
                                lerp(frac(sin(n + 157.0) * 43758.5453),
                                     frac(sin(n + 158.0) * 43758.5453), f.x), f.y),
                           lerp(lerp(frac(sin(n + 113.0) * 43758.5453),
                                     frac(sin(n + 114.0) * 43758.5453), f.x),
                                lerp(frac(sin(n + 270.0) * 43758.5453),
                                     frac(sin(n + 271.0) * 43758.5453), f.x), f.y), f.z);
            }

            float cloudLayer(float3 p, float scale)
            {
                float n = noise(p * scale);
                n += 0.5 * noise(p * scale * 2);
                n += 0.25 * noise(p * scale * 4);
                return saturate(n - (1 - _CloudDensity));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.texcoord);
                
                float skyGradient = saturate(dir.y);
                fixed4 skyColor = lerp(_SkyColorHorizon, _SkyColorZenith, skyGradient);
                
                float3 cloudPlaneNormal = float3(0, 1, 0);
                float t1 = (_CloudHeight1 - dot(float3(0,0,0), cloudPlaneNormal)) / dot(dir, cloudPlaneNormal);
                float t2 = (_CloudHeight2 - dot(float3(0,0,0), cloudPlaneNormal)) / dot(dir, cloudPlaneNormal);
                float3 cloudPos1 = float3(0,0,0) + t1 * dir;
                float3 cloudPos2 = float3(0,0,0) + t2 * dir;
                
                float cloudMask = step(0, dir.y);
                
                float3 windOffset1 = _WindDirection.xyz * _Time.y * _CloudSpeed;
                float3 windOffset2 = _WindDirection.xyz * _Time.y * _CloudSpeed * 0.5;
                float3 p1 = cloudPos1 * _CloudScale1 + windOffset1;
                float3 p2 = cloudPos2 * _CloudScale2 + windOffset2;
                float3 p3 = cloudPos1 * _EraseNoiseScale + windOffset1 * 0.7;
                float3 p4 = cloudPos2 * _EraseNoiseScale + windOffset2 * 0.7;
                
                float c1 = cloudLayer(p1, _CloudScale1);
                float c2 = cloudLayer(p2, _CloudScale2);
                
                float eraseNoise1 = noise(p3);
                float blurredEraseNoise1 = smoothstep(_EraseNoiseStrength - _EraseNoiseBlur, _EraseNoiseStrength + _EraseNoiseBlur, eraseNoise1);
                c1 *= 1 - blurredEraseNoise1;

                float eraseNoise2 = noise(p4);
                float blurredEraseNoise2 = smoothstep(_EraseNoiseStrength - _EraseNoiseBlur, _EraseNoiseStrength + _EraseNoiseBlur, eraseNoise2);
                c2 *= 1 - blurredEraseNoise2;
                
                float clouds = saturate(c1 + c2 * 0.5) * cloudMask * _CloudOpacity;
                
                float fadeDistance = 1 - saturate((length(cloudPos1.xz) - _CloudFadeDistance) / (10 - _CloudFadeDistance));
                clouds *= fadeDistance;
                
                fixed4 cloudColor = lerp(_CloudColor2, _CloudColor1, clouds);
                
                return lerp(skyColor, cloudColor, clouds);
            }
            ENDCG
        }
    }
}