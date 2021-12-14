Shader "Custom/Terrain"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GridTex("Grid Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #pragma multi_compile _ GRID_ON

        sampler2D _MainTex;
        sampler2D _GridTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 color: COLOR;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            
            fixed4 grid = 1;
#if defined(GRID_ON)
            float2 gridUV = IN.worldPos.xz;

            //Forward distance between adjacent cell centers is 15
            //twice that to move two straight cells up
            gridUV.y *= 1 / (2 * 15.0);
            // Inner radius of cells is 5*sqrt(3).
            // 4 times this is needed to move two cells to the right
            gridUV.x  *= 1 / (4 * 8.66025404);
            gridUV.y += 0.5;
            gridUV.x += 0.5;
            grid = tex2D(_GridTex, gridUV);
#endif
            o.Albedo = c.rgb * grid * IN.color;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
