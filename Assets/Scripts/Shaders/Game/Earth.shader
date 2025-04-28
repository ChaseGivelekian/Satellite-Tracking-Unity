Shader "Custom/Earth"
{
    Properties
    {
        _DayTex ("Day Texture", 2D) = "white" {}
        _NightTex ("Night Texture", 2D) = "black" {}
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

        struct Input
        {
            float2 uv_DayTex;
            float3 worldNormal;
            float3 viewDir;
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

            // Get normalized world normal
            float3 normal = normalize(IN.worldNormal);

            // Calculate dot product between normal and light direction
            // This gives us a value from -1 (facing away from sun) to 1 (facing directly at sun)
            float sunDot = dot(dirToSun, normal);

            // Create a smooth transition from night to day
            float dayFactor = saturate((sunDot + 0.1) * _TransitionSharpness);

            // Sample both textures
            fixed4 dayColor = tex2D(_DayTex, IN.uv_DayTex) * _Color;
            fixed4 nightColor = tex2D(_NightTex, IN.uv_DayTex) * _Color;

            // Blend between night and day based on the light direction
            o.Albedo = lerp(nightColor.rgb, dayColor.rgb, dayFactor);

            // Night texture emission (only shows when in shadow)
            o.Emission = nightColor.rgb * _EmissionStrength * (1.0 - dayFactor);

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