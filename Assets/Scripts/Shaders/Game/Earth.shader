Shader "Custom/Earth"
{
    Properties
    {
        _DayTex ("Day Texture", 2D) = "white" {}
        _NightTex ("Night Texture", 2D) = "black" {}
        _CloudTex ("Cloud Texture", 2D) = "black" {} // Added cloud texture
        _CloudStrength ("Cloud Strength", Range(0,1)) = 0.5 // Control cloud opacity
        _CloudSpeed ("Cloud Speed", Range(0,1)) = 0.1 // Control cloud movement speed
        _BumpMap ("Land Normal Map", 2D) = "bump" {} // Normal map for land
        _OceanBumpMap ("Ocean Normal Map", 2D) = "bump" {} // Added ocean normal map
        _BumpScale ("Land Bump Scale", Range(0,2)) = 1.0 // Control for land bump intensity
        _OceanBumpScale ("Ocean Bump Scale", Range(0,2)) = 1.0 // Control for ocean bump intensity
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.1
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionStrength ("Night Emission Strength", Range(0,3)) = 1.0
        _TransitionSharpness ("Day/Night Transition Sharpness", Range(1,10)) = 3.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _DayTex;
        sampler2D _NightTex;
        sampler2D _CloudTex;
        sampler2D _BumpMap;
        sampler2D _OceanBumpMap;
        half _BumpScale;
        half _OceanBumpScale;
        half _CloudStrength;
        half _CloudSpeed;

        struct Input
        {
            float2 uv_DayTex;
            float2 uv_BumpMap;
            float3 worldNormal;
            float3 viewDir;
            INTERNAL_DATA // Required for WorldReflectionVector to work with normal maps
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _EmissionStrength;
        float _TransitionSharpness;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Get normalized direction to the light source
            float3 dirToSun = normalize(_WorldSpaceLightPos0.xyz);

            // Sample base textures first
            fixed4 dayColor = tex2D(_DayTex, IN.uv_DayTex) * _Color;
            fixed4 nightColor = tex2D(_NightTex, IN.uv_DayTex) * _Color;

            // Sample cloud texture with animation
            float2 cloudUV = IN.uv_DayTex;
            cloudUV.x = frac(cloudUV.x + _Time.y * _CloudSpeed); // Animate clouds horizontally with wrapping
            fixed4 cloudColor = tex2D(_CloudTex, cloudUV);

            // Sample both normal maps
            fixed3 landNormal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            fixed3 oceanNormal = UnpackNormal(tex2D(_OceanBumpMap, IN.uv_BumpMap));

            // Apply scales to the normal maps
            landNormal.xy *= _BumpScale;
            oceanNormal.xy *= _OceanBumpScale;

            // This works because where one map has no detail (is flat), it won't affect the other map's details
            fixed3 combinedNormal;
            combinedNormal.xy = landNormal.xy + oceanNormal.xy;
            combinedNormal.z = 1.0;
            combinedNormal = normalize(combinedNormal);

            o.Normal = combinedNormal;

            // Get world normal (now affected by the combined normal map)
            float3 normal = WorldNormalVector(IN, o.Normal);
            normal = normalize(normal);

            // Calculate dot product between normal and light direction
            float sunDot = dot(dirToSun, normal);

            // Create a smooth transition from night to day
            float dayFactor = saturate((sunDot + 0.1) * _TransitionSharpness);

            // Blend between night and day based on the light direction
            o.Albedo = lerp(nightColor.rgb, dayColor.rgb, dayFactor);

            // Add clouds with more visibility on the day side
            float cloudAlpha = cloudColor.a * _CloudStrength * (0.3 + 0.7 * dayFactor);
            o.Albedo = lerp(o.Albedo, cloudColor.rgb, cloudAlpha);

            // Night texture emission (only shows when in shadow)
            o.Emission = nightColor.rgb * _EmissionStrength * (1.0 - dayFactor) * (1.0 - cloudAlpha * 0.5);

            // Reduce specular at grazing angles (where horizon is visible)
            float viewDot = dot(normal, normalize(IN.viewDir));
            float grazingFactor = 1.0 - saturate(viewDot);

            // Reduce smoothness at grazing angles
            o.Smoothness = _Glossiness * (1.0 - pow(grazingFactor, 3));
            o.Metallic = _Metallic * (1.0 - pow(grazingFactor, 2));
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}